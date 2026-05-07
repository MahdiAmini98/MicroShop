using BasketService.Application.Interfaces;
using BasketService.Infrastructure.Context;

namespace BasketService.Application.Services
{
    public class ProductService: IProductService
    {
        private readonly BasketDataBaseContext context;
        public ProductService(BasketDataBaseContext context)
        {
            this.context = context;
        }

        public bool ProductUpdatedName(Guid ProductId, string productName)
        {
            var product = context.Products.Find(ProductId);
            if (product is not null)
            {
                product.ProductName = productName;
                context.SaveChanges();

            }
            return true;

        }
    }
}
