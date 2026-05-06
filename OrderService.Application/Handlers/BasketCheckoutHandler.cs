using Common.EventBus.Interfaces;
using Common.EventBus.Messages.BasketToOrder;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Handlers
{
    public class BasketCheckoutHandler : IMessageHandler<BasketCheckoutMessage>
    {
        private readonly ILogger<BasketCheckoutHandler> _logger;
        private readonly IOrderService orderService;
        public BasketCheckoutHandler(ILogger<BasketCheckoutHandler> logger, IOrderService orderService)
        {
            _logger = logger;
            this.orderService = orderService;
        }

        public async Task HandleAsync(BasketCheckoutMessage message, CancellationToken cancellationToken = default)
        {
            orderService.RegisterOrderService(message);
            await Task.CompletedTask;
        }
    }
}
