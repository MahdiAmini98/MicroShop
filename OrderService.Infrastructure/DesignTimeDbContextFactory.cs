using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OrderService.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderDataBaseContext>
    {
        public OrderDataBaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderDataBaseContext>();

            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MicroShop_OrderService_DB;Integrated Security=true;MultipleActiveResultSets=true;TrustServerCertificate=true;");

            return new OrderDataBaseContext(optionsBuilder.Options);
        }
    }
}
