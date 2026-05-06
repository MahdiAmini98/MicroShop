using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Dtos
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}
