using GoFishIng.Data;
using GoFishIng.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoFishIng.Common;
using GoFishIng.Data.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.Threading;


namespace GoFishIng.Services
{
    public class TripsService : ITripsService
    {
        private readonly ApplicationDbContext db;
        private readonly ICartsService cartsServices;
        private readonly IApplicationUsersService applicationUsersServices;
        

        public TripsService(ApplicationDbContext db, ICartsService cartsServices, IApplicationUsersService applicationUsersServices)
        {
            this.db = db;
            this.cartsServices = cartsServices;
            this.applicationUsersServices = applicationUsersServices;
           
        }

        public string CreateTrip(string cartId,string name, string type,int groupSize, string startDate, string endDate)
        {
            var date1 = DateTime.Parse(startDate);
            var date2 = DateTime.Parse(endDate);
            decimal thePrice = 0;

            if (type == "BigGame")
            {
                thePrice = GlobalConstants.PriceMadeira * groupSize;
                name = GlobalConstants.Madeira;
                type = TripType.BigGame.ToString();
            }

            else if (type == "FreshWater")
            {
                thePrice = GlobalConstants.PriceBosnia * groupSize;
                name = GlobalConstants.Bosnia;
                type = TripType.FreshWater.ToString();
            }

            else
            {
                thePrice = GlobalConstants.PriceGreece * groupSize;
                name = GlobalConstants.Greece;
                type = TripType.SaltWater.ToString();

            }

            
            //var currentUserId = this.applicationUsersServices.GetT;
            //var currentCartId = this.db.Carts.FirstOrDefault(c => c.UserId == currentUserId).Id;         

            var trip = new Trip
            {
                CartId = cartId,
                Name =name,
                Type = (TripType)Enum.Parse(typeof(TripType),type,true),
                GroupSize = groupSize,
                StartDate = date1,
                EndDate = date2,
                Price = thePrice,
               
            };

            this.db.Trips.Add(trip);
            this.db.SaveChanges();

            return trip.TripId;
        }

        public Trip GetTripById(string id)
        {
            var tripId = this.db.Trips.Find(id);

            return tripId;
        }

        
        


    }
}
