using System;
using System.Collections.Generic;
using System.Text;
using static OrderService.Domain.Enums.Enumration;

namespace OrderService.Application.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public int ItemCount { get; set; }
        public int TotalPrice { get; set; }
        public bool OrderPaid { get; set; }
        public DateTime OrderPlaced { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

    }
}
