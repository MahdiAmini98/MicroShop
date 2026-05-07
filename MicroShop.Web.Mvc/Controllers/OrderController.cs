using MicroShop.Web.Mvc.Services.OrderService;
using MicroShop.Web.Mvc.Services.PaymentService;
using Microsoft.AspNetCore.Mvc;
using static Common.Core.Enums.Enumration;

namespace MicroShop.Web.Mvc.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IPaymentService paymentService;
        private readonly string UserId = "1";

        public OrderController(IOrderService orderService, IPaymentService paymentService)
        {
            this.orderService = orderService;
            this.paymentService = paymentService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var orders = orderService.GetOrders(UserId);
            return View(orders);
        }

        [HttpGet]
        public IActionResult Detail(Guid Id)
        {
            var order = orderService.OrderDetail(Id);
            return View(order);
        }

        [HttpGet]
        public IActionResult Pay(Guid OrderId)
        {
            var order = orderService.OrderDetail(OrderId);
            if (order.PaymentStatus == PaymentStatus.isPaid)
            {
                return RedirectToAction(nameof(Detail), new { Id = OrderId });
            }
            if (order.PaymentStatus == PaymentStatus.unPaid)
            {
                //ارسال درخواست پرداخت برای سرویس سفارش
                var paymentRequest = orderService.RequestPayment(OrderId);
            }

            // دریافت لینک پرداخت از سرویس پرداخت
            string callbackUrl = Url.Action(nameof(Detail), "Order",
                new { Id = OrderId }, protocol: Request.Scheme);
            var paymentlink = paymentService.GetPaymentlink(order.Id, callbackUrl);

            if (paymentlink.IsSuccess)
            {
                return Redirect(paymentlink.Data.PaymentLink);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
