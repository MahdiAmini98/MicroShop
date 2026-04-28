using BasketService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasketService.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BasketDataBaseContext>
    {
        public BasketDataBaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BasketDataBaseContext>();

            // تنظیم ConnectionString مستقیماً برای زمان طراحی
            // توجه: این ConnectionString فقط برای ابزارهای EF Core استفاده می‌شود
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MicroShop_BasketService_DB;Integrated Security=true;MultipleActiveResultSets=true;TrustServerCertificate=true;");

            return new BasketDataBaseContext(optionsBuilder.Options);
        }
    }
}
