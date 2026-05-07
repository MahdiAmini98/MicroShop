using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Dtos;
using ProductService.Application.Dtos.Product;
using ProductService.Application.Interfaces;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductManagementController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductManagementController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] AddNewProductDto addNewProductDto)
        {
            productService.AddNewProduct(addNewProductDto);
            return Ok();
        }


        [HttpGet]
        public IActionResult Get()
        {
            var products = productService.GetProducts();
            return Ok(products);
        }


        [HttpGet("Id")]
        public IActionResult Get(Guid Id)
        {
            var product = productService.GetProduct(Id);
            return Ok(product);
        }

        [HttpPut]
        public IActionResult Put(UpdateProductDto updateProduct)
        {
            return Ok(productService.UpdateProductName(updateProduct));
        }

    }
}
