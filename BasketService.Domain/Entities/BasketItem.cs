using System;
using System.Collections.Generic;
using System.Text;

namespace BasketService.Domain.Entities
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Basket Basket { get; set; }
        public Guid BasketId { get; set; }
        public void SetQuantity(int quantity)
        {
            Quantity = quantity;
        }
        public Product Product { get; set; }
    }
}
