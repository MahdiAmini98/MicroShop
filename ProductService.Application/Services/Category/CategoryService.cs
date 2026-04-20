using ProductService.Application.Dtos;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ProductDbContext Context;
        public CategoryService(ProductDbContext context)
        {
            this.Context = context;
        }
        public void AddCategory(CategoryDto category)
        {
            Category newCategory = new()
            {
                Name = category.Name,
                Description = category.Description
            };
            
            Context.Categories.Add(newCategory);
            Context.SaveChanges();
        }

        public List<CategoryDto> GetCategories()
        {
            var data = Context.Categories.OrderBy(x => x.Name).Select(x => new CategoryDto
            {
                Name = x.Name,
                Description = x.Description,
                Id = x.Id
            }).ToList();
            return data;
        }
    }
}
