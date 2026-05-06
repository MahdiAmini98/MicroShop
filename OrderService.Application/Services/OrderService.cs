using Common.Core.Dtos;
using Common.EventBus.Constants;
using Common.EventBus.Interfaces;
using Common.EventBus.Messages.BasketToOrder;
using Common.EventBus.Messages.OrderToPayment;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Context;

namespace OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderDataBaseContext context;
        private readonly IProductService productService;
        private readonly IMessageBus _messageBus;

        public OrderService(OrderDataBaseContext context, IProductService productService, IMessageBus messageBus)
        {
            this.context = context;
            this.productService = productService;
            _messageBus = messageBus;
        }

        public OrderDetailDto GetOrderById(Guid Id)
        {
            var orders = context.Orders.
                   Include(p => p.OrderLines)
                  .ThenInclude(p => p.Product)
                 .FirstOrDefault(p => p.Id == Id);

            if (orders == null)
                throw new Exception("Order Not Found");
            var result = new OrderDetailDto
            {
                Id = orders.Id,
                Address = orders.Address,
                FirstName = orders.FirstName,
                LastName = orders.LastName,
                PhoneNumber = orders.PhoneNumber,
                UserId = orders.UserId,
                TotalPrice = orders.TotalPrice,
                OrderPaid = orders.OrderPaid,
                OrderPlaced = orders.OrderPlaced,
                PaymentStatus = orders.PaymentStatus,
                OrderLines = orders.OrderLines.Select(ol => new OrderLineDto
                {
                    Id = ol.Id,
                    Name = ol.Product.Name,
                    Price = ol.Product.Price,
                    Quantity = ol.Quantity,

                }).ToList(),

            };
            return result;

        }

        public List<OrderDto> GetOrdersForUser(string UserId)
        {
            var orders = context.Orders.
             Include(p => p.OrderLines)
            .Where(p => p.UserId == UserId)
            .Select(p => new OrderDto
            {
                Id = p.Id,
                OrderPaid = p.OrderPaid,
                OrderPlaced = p.OrderPlaced,
                ItemCount = p.OrderLines.Count(),
                TotalPrice = p.TotalPrice,
                PaymentStatus = p.PaymentStatus
            }).ToList();
            return orders;
        }

        public bool RegisterOrderService(BasketCheckoutMessage basket)
        {
            List<OrderLine> orderLines = new List<OrderLine>();
            foreach (var basketItem in basket.BasketItems)
            {
                var product = productService.GetProduct(new ProductDto
                {
                    Name = basketItem.Name,
                    Price = basketItem.Price,
                    ProductId = basketItem.ProductId,
                });

                orderLines.Add(new OrderLine
                {
                    Id = Guid.NewGuid(),
                    Quantity = basketItem.Quantity,
                    Product = product,
                });

            }

            Order order = new Order(basket.UserId,
                  basket.FirstName,
                  basket.LastName,
                  basket.Address,
                  basket.PhoneNumber,
                  basket.TotalPrice,
                  orderLines);

            context.Orders.Add(order);
            context.SaveChanges();
            return true;
        }

        public ResultDto RequestPayment(Guid OrderId)
        {
            var order = context.Orders.SingleOrDefault(p => p.Id == OrderId);
            if (order == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "سفارش پیدا نشد"
                };
            }
            // ارسال پیام پرداخت برای سرویس پرداخت
            SendOrderToPaymentMessage paymentMessage = new SendOrderToPaymentMessage()
            {
                Amount = order.TotalPrice,
                CreationTime = DateTime.Now,
                MessageId = Guid.NewGuid(),
                OrderId = order.Id,
               
            };
            _messageBus.PublishAsync(paymentMessage, QueueNames.SendOrderToPayment).GetAwaiter();
            //تغییر وضعیت پرداخت سفارش
            order.RequestPayment();
            context.SaveChanges();
            return new ResultDto
            {
                IsSuccess = true,
                Message = "درخواست پرداخت ثبت شد"
            };
        }
    }
}
