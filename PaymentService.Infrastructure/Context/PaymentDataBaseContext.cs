using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure.Context
{
    public class PaymentDataBaseContext : DbContext
    {

        public PaymentDataBaseContext(DbContextOptions<PaymentDataBaseContext> options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}
