using ProductService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Application.Interfaces
{
    public interface ICategoryService
    {
        List<CategoryDto> GetCategories();
        void AddCategory(CategoryDto category);
    }
}
