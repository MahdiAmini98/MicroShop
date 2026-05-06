using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Interfaces;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderPaymentController : ControllerBase
    {
        private readonly IOrderService orderService;
        public OrderPaymentController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost]
        public IActionResult Post(Guid OrderId)
        {
            return Ok(orderService.RequestPayment(OrderId));
        }
    }
}
