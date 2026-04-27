using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Dtos
{
    public class AddOrderLineDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public int Quantity { get; set; }
    }
}
