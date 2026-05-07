using BasketService.Application.Handlers;
using BasketService.Application.Interfaces;
using BasketService.Application.MappingProfile;
using BasketService.Application.Services;
using BasketService.Domain.Repository;
using BasketService.Infrastructure.Context;
using BasketService.Infrastructure.gRPC;
using Common.EventBus.Constants;
using Common.EventBus.Extensions;
using Common.EventBus.Messages.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


#region DbContext
var connectionString = builder.Configuration.GetConnectionString("BasketConnection");
builder.Services.AddDbContext<BasketDataBaseContext>(p => p.UseSqlServer(connectionString));
#endregion

#region Repository
builder.Services.AddTransient<IDiscountRepository, DiscountGrpcClient>();

#endregion

#region Services
builder.Services.AddTransient<IBasketService, BasketService.Application.Services.BasketService>();
builder.Services.AddTransient<IDiscountService, BasketService.Application.Services.DiscountService>();
builder.Services.AddTransient<IProductService, ProductService>();

#endregion

#region AutoMapper
builder.Services.AddAutoMapper(typeof(BasketMappingProfile));
#endregion

#region RabitMQ
builder.Services.AddRabbitMQService(builder.Configuration);

builder.Services.AddRabitMQFanoutConsumer<ProductUpdatedNameEvent, BasketProductUpdatedNameHandler>(ExchangeNames.ProductUpdatedNameEvent, QueueNames.BasketProductUpdatedName);
#endregion
builder.Services.AddControllers();
builder.Services.AddOpenApi();

#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Basket Service API",
        Version = "v1",
        Description = "API for managing Baskets"
    });
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
