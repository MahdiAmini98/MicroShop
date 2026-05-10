using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Dtos;
using ProductService.Application.Interfaces;

namespace ProductService.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService CategoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.CategoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var data = CategoryService.GetCategories();
            return Ok(data);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CategoryDto categoryDto)
        {
            CategoryService.AddCategory(categoryDto);
            return Ok();
        }

    }
}
