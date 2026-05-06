using OrderService.Application.Dtos;
using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces
{
    public interface IProductService
    {
        Product GetProduct(ProductDto productDto);
    }
}
