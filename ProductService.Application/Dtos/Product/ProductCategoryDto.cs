using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Application.Dtos
{
    public class ProductCategoryDto
    {
        public Guid CategoryId { get; set; }
        public string Category { get; set; }
    }
}
