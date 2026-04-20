using System;
using System.Collections.Generic;
using System.Text;

namespace BasketService.Application.Dtos
{
    public class BasketDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();
        public int Total()
        {
            if (Items.Count > 0)
            {
                int total = Items.Sum(p => p.UnitPrice * p.Quantity);
                return total;
            }
            return 0;
        }
    }
}
