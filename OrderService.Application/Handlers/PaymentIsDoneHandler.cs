using Common.EventBus.Interfaces;
using Common.EventBus.Messages.PaymentToOrder;
using OrderService.Infrastructure.Context;
namespace OrderService.Application.Handlers
{
    public class PaymentIsDoneHandler : IMessageHandler<PaymentIsDoneMessage>
    {
        private readonly OrderDataBaseContext context;

        public PaymentIsDoneHandler(OrderDataBaseContext context)
        {
            this.context = context;
        }

        public async Task HandleAsync(PaymentIsDoneMessage message, CancellationToken cancellationToken = default)
        {
            var order = context.Orders.SingleOrDefault(p => p.Id == message.OrderId);
            order.PaymentIsDone();
            context.SaveChanges();
            await Task.CompletedTask;
        }
    }
}
