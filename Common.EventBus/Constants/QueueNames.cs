namespace Common.EventBus.Constants
{
    public static class QueueNames
    {
        public const string BasketCheckout = "basket-checkout-queue";
        public const string SendOrderToPayment = "send-order-to-payment-queue";
        public const string PaymentIsDone = "payment-is-done-queue";
    }
}
