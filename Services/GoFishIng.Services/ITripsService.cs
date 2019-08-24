using System;
using System.Collections.Generic;
using System.Text;

namespace GoFishIng.Services
{
    public interface ITripsService
    {
        string CreateTrip(string name,string type, int groupSize, string startDate, string endDate);
    }
}
