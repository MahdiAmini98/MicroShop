using BasketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasketService.Infrastructure.Context
{
    public class BasketDataBaseContext : DbContext
    {
        public BasketDataBaseContext(DbContextOptions<BasketDataBaseContext> options)
       : base(options)
        {

        }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Product> Products { get; set; }

    }
}
