using DiscountService.API.Services;
using DiscountService.Application.Interfaces;
using DiscountService.Application.MappingProfile;
using DiscountService.Infrastructure.Context;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
#region Kestrel Configuration
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP/2 only — برای gRPC
    options.ListenLocalhost(7134, o =>
    {
        o.Protocols = HttpProtocols.Http2;
        o.UseHttps(); 
    });
    // HTTP/1.1 — اگه REST endpoint هم داری
    options.ListenLocalhost(5135, o => o.Protocols = HttpProtocols.Http1);
});
#endregion

#region DbContext
var connectionString = builder.Configuration.GetConnectionString("DiscountConnection");
builder.Services.AddDbContext<DiscountDataBaseContext>(p => p.UseSqlServer(connectionString));
#endregion

#region Services
builder.Services.AddTransient<IDiscountService, DiscountService.Application.Services.DiscountService>();
#endregion

#region AutoMapper
builder.Services.AddAutoMapper(typeof(DiscountMappingProfile));

#endregion

var app = builder.Build();

app.UseGrpcWeb();

#region gRPC Pipeline
app.MapGrpcService<GRPCDiscountService>().EnableGrpcWeb();
#endregion

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
