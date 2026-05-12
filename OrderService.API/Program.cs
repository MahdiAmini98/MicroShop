using Common.EventBus.Constants;
using Common.EventBus.Extensions;
using Common.EventBus.Messages.BasketToOrder;
using Common.EventBus.Messages.Events;
using Common.EventBus.Messages.PaymentToOrder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using OrderService.Application.Handlers;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region DbContext
var connectionString = builder.Configuration.GetConnectionString("OrderConnection");
builder.Services.AddDbContext<OrderDataBaseContext>(p => p.UseSqlServer(connectionString));
#endregion

#region Services
builder.Services.AddTransient<IOrderService, OrderService.Application.Services.OrderService>();
builder.Services.AddTransient<IProductService, OrderService.Application.Services.ProductService>();
#endregion

#region RabitMQ
builder.Services.AddRabbitMQService(builder.Configuration);

builder.Services.AddRabbitMQConsumer<BasketCheckoutMessage, BasketCheckoutHandler>(
    QueueNames.BasketCheckout);

builder.Services.AddRabbitMQConsumer<PaymentIsDoneMessage, PaymentIsDoneHandler>(
    QueueNames.PaymentIsDone);

builder.Services.AddRabitMQFanoutConsumer<ProductUpdatedNameEvent, OrderProductUpdatedNameHandler>(ExchangeNames.ProductUpdatedNameEvent, QueueNames.OrderProductUpdatedName);
#endregion
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


#region Api
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers.Clear();

        document.Servers.Add(new()
        {
            Url = "https://localhost:5000/order"
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Service API",
        Version = "v1",
        Description = "Order microservice endpoints"
    });
});
#endregion



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
          "/openapi/v1.json",
          "OrderService");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
