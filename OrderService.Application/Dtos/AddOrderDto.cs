using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Dtos
{
    public class AddOrderDto
    {
        public string UserId { get; set; }
        public List<AddOrderLineDto> OrderLines { get; set; }
    }
}
