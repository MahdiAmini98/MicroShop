using MicroShop.ApiGateway.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. تنظیمات Logging (برای مشاهده درخواست‌ها)
// ==========================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();


// ==========================================
// 1.1 Load کردن فایل
// ==========================================
builder.Configuration
    .AddJsonFile(
        "reverse-proxy.json",
        optional: false,
        reloadOnChange: true);

// ==========================================
// 1.2 خواندن SecurityHeaders از appsettings.json   
// ==========================================
builder.Services.AddSecurityHeaders(builder.Configuration);


// ==========================================
// 1.2 خواندن uthForwarding از appsettings.json   
// ==========================================

builder.Services.AddAuthForwarding(builder.Configuration);

// ==========================================
// 2. تنظیمات Swagger برای API Gateway
// ==========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MicroShop API Gateway",
        Version = "v1",
        Description = "API Gateway for MicroShop Microservices Architecture",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "MicroShop Team",
            Email = "support@microshop.com"
        }
    });

    // اضافه کردن JWT Authentication به Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ==========================================
// 2.1. تنظیمات  برای ExceptionHandling
// ==========================================

builder.Services.AddGlobalExceptionHandling();




// ==========================================
// 3. تنظیمات YARP Reverse Proxy
// ==========================================
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")).AddAuthForwardingTransforms();

// ==========================================
// 4. تنظیمات Authentication (JWT)
// ==========================================
var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-32-character-or-longer-secret-key-here-please-change-me";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MicroShopGateway";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "MicroShopServices";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // برای جلوگیری از خطای HTTPS در محیط توسعه
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization();


// ==========================================
// 5. تنظیمات CORS (برای دسترسی MVC و Admin)
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

// ==========================================
// 6. پیکربندی Pipeline (ترتیب مهم است!)
// ==========================================

// Swagger (فقط در محیط توسعه)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroShop API Gateway v1");
        c.RoutePrefix = "swagger";
    });
}


app.UseExceptionHandler();

app.UseSecurityHeaders();

// Middlewareهای عمومی
app.UseHttpsRedirection();
app.UseCors("AllowAll");
// Authentication & Authorization (قبل از ReverseProxy)
app.UseAuthentication();
app.UseAuthorization();

// لاگ‌گیری درخواست‌ها (اختیاری)
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});

// ⚠️ فقط برای تست - در تولید حذف شود
app.MapPost("/generate-token", (HttpContext context) =>
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("microshop-super-secret-key-minimum-32-characters-long-here"));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "MicroShopGateway",
        audience: "MicroShopServices",
        claims: new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-123"),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.Role, "Admin")
        },
        expires: DateTime.Now.AddHours(1),
        signingCredentials: credentials
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = tokenString });
});



// YARP Reverse Proxy (در انتها)
app.MapReverseProxy();

app.Run();