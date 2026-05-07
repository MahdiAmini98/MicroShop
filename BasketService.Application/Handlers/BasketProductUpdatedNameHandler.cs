using BasketService.Application.Interfaces;
using Common.EventBus.Interfaces;
using Common.EventBus.Messages.Events;

namespace BasketService.Application.Handlers
{
    public class BasketProductUpdatedNameHandler : IMessageHandler<ProductUpdatedNameEvent>
    {
        private readonly IProductService productService;

        public BasketProductUpdatedNameHandler(IProductService productService)
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
