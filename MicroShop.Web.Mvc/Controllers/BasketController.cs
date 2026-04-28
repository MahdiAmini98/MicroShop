using MicroShop.Web.Mvc.Services.BasketServices;
using MicroShop.Web.Mvc.Services.ProductServices;
using Microsoft.AspNetCore.Mvc;

namespace MicroShop.Web.Mvc.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketService basketService;
        private readonly IProductService productService;
        private readonly string UserId = "1";
        public BasketController(IBasketService basketService, IProductService productService)
        {
            this.basketService = basketService;
            this.productService = productService;
        }
        public IActionResult Index()
        {
            var basket = basketService.GetBasket(UserId);
            return View(basket);
        }

        public IActionResult Delete(Guid Id)
        {
            basketService.DeleteFromBasket(Id);
            return RedirectToAction("Index");
        }

        public IActionResult AddToBasket(Guid ProductId)
        {
            var product = productService.Getproduct(ProductId);
            var basket = basketService.GetBasket(UserId);

            AddToBasketDto item = new AddToBasketDto()
            {
                BasketId = basket.id,
                ImageUrl = product.image,
                ProductId = product.id,
                ProductName = product.name,
                Quantity = 1,
                UnitPrice = product.price,
            };
            basketService.AddToBasket(item, UserId);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(Guid BasketItemId, int quantity)
        {
            basketService.UpdateQuantity(BasketItemId, quantity);
            return RedirectToAction("Index");

        }
    }
}
