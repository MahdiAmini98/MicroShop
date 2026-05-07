namespace Common.EventBus.Messages.Events
{
    public class ProductUpdatedNameEvent: BaseMessage
    {
        public Guid Id { get; set; }
        public string NewName { get; set; }
    }
}
