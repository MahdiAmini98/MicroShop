using MicroShop.Web.Mvc.Services.OrderService;
using Microsoft.AspNetCore.Mvc;

namespace MicroShop.Web.Mvc.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly string UserId = "1";

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        public IActionResult Index()
        {
            var orders = orderService.GetOrders(UserId);
            return View(orders);
        }

        public IActionResult Detail(Guid Id)
        {
            var order = orderService.OrderDetail(Id);
            return View(order);
        }
    }
}
