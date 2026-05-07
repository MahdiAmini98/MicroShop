using Microsoft.EntityFrameworkCore;
using ProductService.Application.Dtos;
using ProductService.Application.Dtos.Product;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext Context;

        public ProductService(ProductDbContext context)
        {
            Context = context;
        }

        public void AddNewProduct(AddNewProductDto addNewProduct)
        {
            var category = Context.Categories.Find(addNewProduct.CategoryId);
            if (category == null)
                throw new Exception("Category Not Found...!");
            Product product = new Product()
            {
                Category = category,
                Description = addNewProduct.Description,
                Image = addNewProduct.Image,
                Name = addNewProduct.Name,
                Price = addNewProduct.Price
            };
            Context.Products.Add(product);
            Context.SaveChanges();
        }

        public void AddProduct(AddNewProductDto product)
        {
            var category = Context.Categories.Find(product.CategoryId);
            if (category == null)
                throw new Exception("Category Not Found...");

            Product newProduct = new()
            {
                CategoryId = category.Id,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
                Price = product.Price,
            };
            Context.Products.Add(newProduct);
            Context.SaveChanges();
        }

        public ProductDto GetProduct(Guid id)
        {
            var product = Context.Products.Include(x => x.Category).Where(x => x.Id == id).Select(x => new ProductDto
            {
                Category = new ProductCategoryDto
                {
                    Category = x.Category.Name,
                    CategoryId = x.Category.Id,
                },
                Description = x.Description,
                Name = x.Name,
                Image = x.Image,
                Id = x.Id,
                Price = x.Price
            }).SingleOrDefault();

            if (product is null)
                throw new Exception("Product Not Found...");

            return product;
        }

        public List<ProductDto> GetProducts()
        {
            var data = Context.Products.Include(p => p.Category).OrderByDescending(p => p.Id).Select(p => new ProductDto
            {
                Id = p.Id,
                Category = new ProductCategoryDto
                {
                    Category = p.Category.Name,
                    CategoryId = p.CategoryId
                },
                Description = p.Description,
                Name = p.Name,
                Image = p.Image,
                Price = p.Price
            }).ToList();
            return data;
        }

        public bool UpdateProductName(UpdateProductDto updateProduct)
        {
            var product = Context.Products.Find(updateProduct.ProductId);
            if (product is not null)
            {
                product.Name = updateProduct.Name;
                Context.SaveChanges();
                return true;
            }
            else
                return false;
        }

    }
}
