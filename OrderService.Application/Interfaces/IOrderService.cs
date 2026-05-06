using Common.EventBus.Messages.BasketToOrder;
using OrderService.Application.Dtos;

namespace OrderService.Application.Interfaces
{
    public interface IOrderService
    {
        List<OrderDto> GetOrdersForUser(string UserId);
        OrderDetailDto GetOrderById(Guid Id);
        bool RegisterOrderService(BasketCheckoutMessage basket);
    }
}
