using MicroShop.ApiGateway.Configurations;
using System.Security.Claims;
using Yarp.ReverseProxy.Transforms;

namespace MicroShop.ApiGateway.Transforms;

/// <summary>
/// یه Transform واحد که هم token و هم claims رو handle می‌کنه
/// 
/// مشکلات نسخه قبل که اینجا حل شده:
/// - دو callback جداگانه → یه کلاس واحد
/// - TryAddWithoutValidation بدون Remove → Remove قبل از Add
/// - role های تکراری → جمع‌آوری در List و comma-separated
/// - ExcludedClaims آرایه → HashSet برای O(1) lookup
/// </summary>
public sealed class ClaimsToHeadersTransform : RequestTransform
{
    private readonly AuthForwardingOptions _authOptions;
    private readonly ClaimsMappingOptions _mappingOptions;

    // HashSet برای O(1) lookup — یه بار ساخته می‌شه
    private readonly HashSet<string> _excludedClaimsSet;
    private readonly HashSet<string> _roleClaimTypesSet;

    public ClaimsToHeadersTransform(
        AuthForwardingOptions authOptions,
        ClaimsMappingOptions mappingOptions)
    {
        _authOptions = authOptions;
        _mappingOptions = mappingOptions;

        // یه بار در constructor — نه per-request
        _excludedClaimsSet = new HashSet<string>(
            mappingOptions.ExcludedClaims,
            StringComparer.OrdinalIgnoreCase);

        _roleClaimTypesSet = new HashSet<string>(
            mappingOptions.RoleClaimTypes,
            StringComparer.OrdinalIgnoreCase);
    }

    public override ValueTask ApplyAsync(RequestTransformContext context)
    {
        if (!_authOptions.Enabled)
            return ValueTask.CompletedTask;

        var httpContext = context.HttpContext;
        var proxyHeaders = context.ProxyRequest.Headers;

        // همیشه Gateway header اضافه می‌شه
        SetHeader(proxyHeaders, _authOptions.GatewayHeader, SanitizeHeaderValue(_authOptions.GatewayHeaderValue));

        // ── User authenticate نشده ────────────────────────────
        if (httpContext.User.Identity?.IsAuthenticated != true)
        {
            SetHeader(proxyHeaders, _authOptions.IsAuthenticatedHeader, "false");
            return ValueTask.CompletedTask;
        }

        // ── User authenticate شده ─────────────────────────────
        SetHeader(proxyHeaders, _authOptions.IsAuthenticatedHeader, "true");

        // Bearer Token Forward
        if (_authOptions.ForwardBearerToken)
            ForwardToken(context, httpContext);

        // Claims → Headers
        if (_mappingOptions.Enabled)
            ForwardClaims(proxyHeaders, httpContext.User);

        return ValueTask.CompletedTask;
    }

    private void ForwardToken(RequestTransformContext context, HttpContext httpContext)
    {
        var authHeader = httpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader))
            return;

        // token رو parse کن — فقط مقدار بعد از "Bearer "
        var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authHeader["Bearer ".Length..].Trim()
            : authHeader;

        if (_authOptions.UseCustomHeader)
        {
            // به جای Authorization، در header دلخواه بذار
            SetHeader(headers: context.ProxyRequest.Headers,
                      name: _authOptions.CustomHeaderName,
                      value: token);
        }
        else
        {
            // استاندارد Authorization header
            context.ProxyRequest.Headers.Remove("Authorization");
            context.ProxyRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    private void ForwardClaims(
        System.Net.Http.Headers.HttpRequestHeaders proxyHeaders,
        ClaimsPrincipal user)
    {
        // track کن کدام headerها set شدن — جلوی تکراری رو می‌گیره
        var processedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var roleValues = new List<string>();

        foreach (var claim in user.Claims)
        {
            // Excluded claims رو skip کن
            if (_excludedClaimsSet.Contains(claim.Type))
                continue;

            // Role claims رو جمع‌آوری کن
            if (_roleClaimTypesSet.Contains(claim.Type))
            {
                roleValues.Add(claim.Value);
                continue;
            }

            // Regular mapping
            if (!_mappingOptions.Mappings.TryGetValue(claim.Type, out var headerName))
                continue;

            // اولین مقدار برنده — تکراری نمی‌نویسیم
            if (!processedHeaders.Add(headerName))
                continue;

            SetHeader(proxyHeaders, headerName, SanitizeHeaderValue(claim.Value));
        }

        // Roles — comma-separated در یه header
        if (roleValues.Count > 0)
        {
            SetHeader(
                proxyHeaders,
                _mappingOptions.RolesHeaderName,
                string.Join(",", roleValues.Distinct(StringComparer.OrdinalIgnoreCase)));
        }
    }

    private static void SetHeader(
        System.Net.Http.Headers.HttpRequestHeaders headers,
        string name,
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        headers.Remove(name);
        headers.TryAddWithoutValidation(name, value);
    }

    private static string SanitizeHeaderValue(string value)
    {
        return value
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty);
    }
}