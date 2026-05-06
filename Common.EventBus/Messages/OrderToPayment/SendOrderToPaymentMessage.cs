
namespace Common.EventBus.Messages.OrderToPayment
{
    public class SendOrderToPaymentMessage : BaseMessage
    {
        public Guid OrderId { get; set; }
        public int Amount { get; set; }
    }
}
