using GoFishIng.Data;
using GoFishIng.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoFishIng.Common;
using GoFishIng.Data.Models.Enums;

namespace GoFishIng.Services
{
    public class TripsService : ITripsService
    {
        private readonly ApplicationDbContext db;
        public TripsService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public string CreateTrip(string name, string type,int groupSize, string startDate, string endDate)
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

            var trip = new Trip
            {
                Name=name,
                Type = (TripType)Enum.Parse(typeof(TripType),type,true),
                GroupSize = groupSize,
                StartDate = date1,
                EndDate = date2,
                Price = thePrice,
            };

            return trip.Id;
        }


    }
}
