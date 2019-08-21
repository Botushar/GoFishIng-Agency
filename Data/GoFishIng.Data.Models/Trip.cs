using GoFishIng.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoFishIng.Data.Models
{
    public class Trip
    {
        public Trip()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Participants = new HashSet<ApplicationUser>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public TripType Type { get; set; }

        public int GroupSize { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        public string CartId { get; set; }

        public Cart Cart { get; set; }

        public string OrderId { get; set; }

        public Order Order { get; set; }

        public ICollection<ApplicationUser> Participants { get; set; }
    }
}
