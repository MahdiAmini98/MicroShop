using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Dtos;
using ProductService.Application.Interfaces;

namespace ProductService.API.Controllers
{
    [Route("api/[Controller]")]
    public class ProductAdminController : ControllerBase
    {
        private readonly IProductService ProductService;

        public ProductAdminController(IProductService productService)
        {
            ProductService = productService;
        }


        [HttpPost]
        public IActionResult Post([FromBody] AddNewProductDto productDto)
        {
            ProductService.AddProduct(productDto);
            return Ok();
        }
    }
}
