using MicroShop.Web.Mvc.Services.BasketServices;
using MicroShop.Web.Mvc.Services.DiscountService;
using MicroShop.Web.Mvc.Services.OrderService;
using MicroShop.Web.Mvc.Services.PaymentService;
using MicroShop.Web.Mvc.Services.ProductServices;
using RestSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mvcService = builder.Services.AddControllersWithViews();
if (builder.Environment.IsDevelopment())
    mvcService.AddRazorRuntimeCompilation();

builder.Services.AddScoped<IProductService>(p =>
{
    return new ProductService(
        new RestClient(builder.Configuration.GetSection("MicroservicAddress:Product:Uri").Value!));
});

builder.Services.AddScoped<IBasketService>(p =>
{
    return new BasketService(
        new RestClient(builder.Configuration.GetSection("MicroservicAddress:Basket:Uri").Value!));
});


builder.Services.AddScoped<IDiscountService, MicroShop.Web.Mvc.Services.DiscountService.DiscountService>();

builder.Services.AddScoped<IOrderService>(p =>
{
    return new OrderService(
        new RestClient(builder.Configuration.GetSection("MicroservicAddress:Order:Uri").Value!));
});


builder.Services.AddScoped<IPaymentService>(p =>
{
    return new PaymentService(
        new RestClient(builder.Configuration.GetSection("MicroservicAddress:Payment:Uri").Value!));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
