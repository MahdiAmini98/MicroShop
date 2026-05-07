using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Application.Dtos.Product
{
    public record UpdateProductDto(Guid ProductId, string Name);
}
