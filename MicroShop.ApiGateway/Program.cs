using MicroShop.ApiGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Configuration & Logging
// ==========================================
builder.ConfigureLogging();
builder.LoadCustomConfigurationFiles();

// ==========================================
// 2. Core Services
// ==========================================
builder.Services.AddStrongConfiguration(builder.Configuration);
builder.Services.AddCustomCors(builder.Configuration);          
builder.Services.AddSwaggerAggregation(builder.Configuration);
builder.Services.AddRedisInfrastructure(builder.Configuration);
builder.Services.AddDistributedRateLimiting();
builder.Services.AddGlobalExceptionHandling();

// ==========================================
// 3. Authentication & Authorization
// ==========================================
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();

// ==========================================
// 4. YARP Reverse Proxy
// ==========================================
builder.Services.AddReverseProxyWithTransforms(builder.Configuration);

var app = builder.Build();

// ==========================================
// Pipeline Configuration
// ==========================================
app.ConfigurePipeline();

app.Run();