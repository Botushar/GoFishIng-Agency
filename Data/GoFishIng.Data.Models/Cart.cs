﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GoFishIng.Data.Models
{
    public class Cart
    {
        public Cart()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Products = new HashSet<Product>();
            this.Trips = new HashSet<Trip>();
        }

        public string Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string OrderId { get; set; }

        public Order Order { get; set; }

        public ICollection<Product> Products { get; set; }

        public ICollection<Trip> Trips { get; set; }

        public ICollection<Permit> Permits { get; set; }
    }
}
