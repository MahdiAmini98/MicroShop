using OrderService.Application.Dtos;
using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces
{
    public interface IProductService
    {
        Product GetProduct(ProductDto productDto);
        bool ProductUpdatedName(Guid ProductId, string productName);

    }
}
