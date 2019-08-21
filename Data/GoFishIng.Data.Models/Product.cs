using GoFishIng.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoFishIng.Data.Models
{
    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public bool InStock { get; set; }

        public ProductType Type { get; set; }

        public string CartId { get; set; }

        public Cart Cart { get; set; }

        public string OrderId { get; set; }

        public Order Order { get; set; }
    }
}
