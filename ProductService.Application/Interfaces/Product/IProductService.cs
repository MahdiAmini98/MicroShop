using ProductService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Application.Interfaces
{
    public interface IProductService
    {
        List<ProductDto> GetProducts();
        void AddProduct(AddNewProductDto product);
        ProductDto GetProduct(Guid id);
    }
}
