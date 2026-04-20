using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
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


builder.Services.AddControllers();
builder.Services.AddOpenApi();

#region Swagger
builder.Services.AddEndpointsApiExplorer();  
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Service API",
        Version = "v1",
        Description = "API for managing products and categories"
    });
});
#endregion
var app = builder.Build();
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
