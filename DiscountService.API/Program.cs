using DiscountService.API.Services;
using DiscountService.Application.Interfaces;
using DiscountService.Application.MappingProfile;
using DiscountService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

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

#region gRPC Pipeline
app.MapGrpcService<GRPCDiscountService>();
#endregion

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
