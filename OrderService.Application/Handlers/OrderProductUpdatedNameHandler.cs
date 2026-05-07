using Common.EventBus.Interfaces;
using Common.EventBus.Messages.Events;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Handlers
{
    public class OrderProductUpdatedNameHandler : IMessageHandler<ProductUpdatedNameEvent>
    {
        private readonly IProductService productService;

        public OrderProductUpdatedNameHandler(IProductService productService)
        {
            this.productService = productService;
        }

        public async Task HandleAsync(ProductUpdatedNameEvent message, CancellationToken cancellationToken = default)
        {
           bool isSuccess = productService.ProductUpdatedName(message.Id, message.NewName);
            if (isSuccess)
                await Task.CompletedTask;
            else
                throw new Exception("ProductUpdatedName Has Error.");
        }
    }
}
