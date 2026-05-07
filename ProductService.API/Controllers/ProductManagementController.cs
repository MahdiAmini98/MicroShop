using Common.EventBus.Constants;
using Common.EventBus.Interfaces;
using Common.EventBus.Messages.Events;
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
        public IActionResult Put(UpdateProductDto updateProduct, [FromServices] IMessageBus messageBus)
        {
            var result = productService.UpdateProductName(updateProduct);
            if (result)
            {
                ProductUpdatedNameEvent @event = new ProductUpdatedNameEvent
                {
                    CreationTime = DateTime.Now,
                    Id = updateProduct.ProductId,
                    NewName = updateProduct.Name,
                    MessageId = Guid.NewGuid(),
                };

                 messageBus.PublishFanoutAsync(@event, ExchangeNames.ProductUpdatedNameEvent).GetAwaiter();
            }
            return Ok(result);
        }

    }
}
