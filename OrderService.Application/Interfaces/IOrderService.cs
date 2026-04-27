using OrderService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Interfaces
{
    public interface IOrderService
    {
        void AddOrder(AddOrderDto addOrder);
        List<OrderDto> GetOrdersForUser(string UserId);
        OrderDto GetOrderById(Guid Id);
    }
}
