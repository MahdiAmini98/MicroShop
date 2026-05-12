using Common.EventBus.Constants;
using Common.EventBus.Extensions;
using Common.EventBus.Messages.OrderToPayment;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PaymentService.Application.Handlers;
using PaymentService.Application.Interfaces;
using PaymentService.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

#region DbContext
var connectionString = builder.Configuration.GetConnectionString("PaymentConnection");
builder.Services.AddDbContext<PaymentDataBaseContext>(p => p.UseSqlServer(connectionString));
#endregion

#region Services
builder.Services.AddTransient<IPaymentService, PaymentService.Application.Services.PaymentService>();

#endregion

#region RabitMQ
// 1. ثبت Producer و تنظیمات RabbitMQ
builder.Services.AddRabbitMQService(builder.Configuration);

// 2. ثبت Consumer 
builder.Services.AddRabbitMQConsumer<SendOrderToPaymentMessage, SendOrderToPaymentHandler>(
    QueueNames.SendOrderToPayment);
#endregion
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

#region Api
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers.Clear();

        document.Servers.Add(new()
        {
            Url = "https://localhost:5000/payment"
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payment Service API",
        Version = "v1",
        Description = "Payment microservice endpoints"
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
                  "PaymentService");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
