using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Application.Dtos
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
