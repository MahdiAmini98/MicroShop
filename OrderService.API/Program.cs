using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
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
#endregion

builder.Services.AddControllers();
builder.Services.AddOpenApi();

#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Service API",
        Version = "v1",
        Description = "API for managing Orders and OrderLines"
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
