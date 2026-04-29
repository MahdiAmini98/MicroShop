using System;
using System.Collections.Generic;
using System.Text;

namespace BasketService.Application.Dtos
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int UnitPrice { get; set; }
        public string ImageUrl { get; set; }
    }
}
