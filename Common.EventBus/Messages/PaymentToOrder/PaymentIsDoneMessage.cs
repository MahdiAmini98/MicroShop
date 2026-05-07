namespace Common.EventBus.Messages.PaymentToOrder
{
    public class PaymentIsDoneMessage : BaseMessage
    {
        public Guid OrderId { get; set; }
    }
}
