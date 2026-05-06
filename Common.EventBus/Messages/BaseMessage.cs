namespace Common.EventBus.Messages
{
    public class BaseMessage
    {
        public Guid MessageId { get; set; } = Guid.NewGuid();
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public string MessageType { get; set; } = string.Empty;
    }
}
