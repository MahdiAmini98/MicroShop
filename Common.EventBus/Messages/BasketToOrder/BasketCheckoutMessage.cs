namespace Common.EventBus.Messages.BasketToOrder
{
    public class BasketCheckoutMessage : BaseMessage
    {
        public Guid BasketId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int TotalPrice { get; set; }
        public List<BasketItemMessage> BasketItems { get; set; } = new();
    }

    public class BasketItemMessage
    {
        public Guid BasketItemId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
