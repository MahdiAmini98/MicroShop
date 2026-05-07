using OrderService.Application.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Context;

namespace OrderService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly OrderDataBaseContext context;

        public ProductService(OrderDataBaseContext context)
        {
            this.context = context;
        }
        public Product GetProduct(ProductDto productDto)
        {
            var existProduct = context.Products.SingleOrDefault(p => p.ProductId == productDto.ProductId);
            if (existProduct != null)
                return existProduct;
            else
                return CreateNewProduct(productDto);
        }

        public bool ProductUpdatedName(Guid ProductId, string productName)
        {
            var product = context.Products.Find(ProductId);
            if (product is not null)
            {
                product.Name = productName;
                context.SaveChanges();
            }
            return true;
        }

        private Product CreateNewProduct(ProductDto productDto)
        {
            Product newProduct = new Product()
            {
                ProductId = productDto.ProductId,
                Name = productDto.Name,
                Price = productDto.Price,
            };
            context.Products.Add(newProduct);
            context.SaveChanges();
            return newProduct;
        }

    }
}
