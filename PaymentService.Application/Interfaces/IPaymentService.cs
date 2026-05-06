using PaymentService.Application.Dtos;

namespace PaymentService.Application.Interfaces
{
    public interface IPaymentService
    {
        PaymentDto GetPaymentofOrder(Guid OrderId);
        PaymentDto GetPayment(Guid PaymentId);
        bool CreatePayment(Guid OrderId, int Amount);
        void PayDone(Guid Paymentid, string Authority, long RefId);
    }
}
