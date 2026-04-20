using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Domain.Entities
{
    public class Category
    {
        public Category()
        {
            Products = new List<Product>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
