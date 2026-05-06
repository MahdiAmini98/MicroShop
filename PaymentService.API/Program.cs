using Common.EventBus.Constants;
using Common.EventBus.Messages.OrderToPayment;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Common.EventBus.Extensions;
using PaymentService.Application.Interfaces;
using PaymentService.Infrastructure.Context;
using PaymentService.Application.Handlers;

var builder = WebApplication.CreateBuilder(args);

#region DbContext
var connectionString = builder.Configuration.GetConnectionString("PaymentConnection");
builder.Services.AddDbContext<PaymentDataBaseContext>(p => p.UseSqlServer(connectionString));
#endregion

#region Services
builder.Services.AddTransient<IPaymentService, PaymentService.Application.Services.PaymentService>();

#endregion
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payment Service API",
        Version = "v1",
        Description = "API for managing Payments"
    });
});
#endregion


#region RabitMQ
// 1. ثبت Producer و تنظیمات RabbitMQ
builder.Services.AddRabbitMQService(builder.Configuration);

// 2. ثبت Consumer 
builder.Services.AddRabbitMQConsumer<SendOrderToPaymentMessage, SendOrderToPaymentHandler>(
    QueueNames.SendOrderToPayment);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Service API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
