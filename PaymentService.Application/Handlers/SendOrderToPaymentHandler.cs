using Common.EventBus.Interfaces;
using Common.EventBus.Messages.OrderToPayment;
using PaymentService.Application.Interfaces;

namespace PaymentService.Application.Handlers
{
    public class SendOrderToPaymentHandler : IMessageHandler<SendOrderToPaymentMessage>
    {
        private readonly IPaymentService paymentService;
        public SendOrderToPaymentHandler(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        public async Task HandleAsync(SendOrderToPaymentMessage message,
                                      CancellationToken cancellationToken = default)
        {
            paymentService.CreatePayment(message.OrderId, message.Amount);
            await Task.CompletedTask;
        }
    }
}
