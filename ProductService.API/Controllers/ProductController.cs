using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Dtos;
using ProductService.Application.Interfaces;
using ProductService.Application.Services;

namespace ProductService.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Route("api/[Controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService ProductService;

        public ProductController(IProductService productService)
        {
            ProductService = productService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var data = ProductService.GetProducts();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var data = ProductService.GetProduct(id);
            return Ok(data);
        }
    }
}
