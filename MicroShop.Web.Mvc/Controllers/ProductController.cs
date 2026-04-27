using Microsoft.AspNetCore.Mvc;

namespace MicroShop.Web.Mvc.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
