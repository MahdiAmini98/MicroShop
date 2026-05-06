using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
