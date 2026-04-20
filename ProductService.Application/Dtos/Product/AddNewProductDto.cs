using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Application.Dtos
{
    public class AddNewProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }
        public Guid CategoryId { get; set; }

    }
}
