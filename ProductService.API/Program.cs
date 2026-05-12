using Common.EventBus.Extensions;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces;
using ProductService.Application.Services;
using ProductService.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

#region DbContext
var connectionString = builder.Configuration.GetConnectionString("ProductConnection");
builder.Services.AddDbContext<ProductDbContext>(p => p.UseSqlServer(connectionString));
#endregion

#region Services
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IProductService, ProductService.Application.Services.ProductService>();
#endregion

#region RabitMQ
builder.Services.AddRabbitMQService(builder.Configuration);
#endregion

#region API

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

#endregion

#region OpenAPI + Swagger

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers.Clear();

        document.Servers.Add(new()
        {
            Url = "https://localhost:5000/product"
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Product Service API",
        Version = "v1",
        Description = "Product microservice endpoints"
    });
});

#endregion

var app = builder.Build();

#region Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "ProductService");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();
