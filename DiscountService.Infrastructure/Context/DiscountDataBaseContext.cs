using DiscountService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscountService.Infrastructure.Context
{
    public class DiscountDataBaseContext : DbContext
    {
        public DiscountDataBaseContext(DbContextOptions<DiscountDataBaseContext> options)
        : base(options)
        {
        }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
    }
}
