using System;
using System.Collections.Generic;
using System.Text;

namespace GoFishIng.Data.Models
{
    public class Permit
    {
        public Permit()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Number { get; set; }

        public string IdentityCardNumber { get; set; }

        public decimal Price { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string CartId { get; set; }

        public Cart Cart { get; set; }

        public string OrderId { get; set; }

        public Order Order { get; set; }
    }
}
