using ProductService.Application.Dtos;
using ProductService.Application.Dtos.Product;
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
        void AddNewProduct(AddNewProductDto addNewProduct);
        bool UpdateProductName(UpdateProductDto updateProduct);
    }
}
