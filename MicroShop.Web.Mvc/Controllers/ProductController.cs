using MicroShop.Web.Mvc.Services.ProductServices;
using Microsoft.AspNetCore.Mvc;

namespace MicroShop.Web.Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }
        public IActionResult Index()
        {
            var products = productService.GetAllProduct();
            return View(products);
        }

        public IActionResult Details(Guid id)
        {
            var product = productService.Getproduct(id);
            return View(product);
        }
    }
}
