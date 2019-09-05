using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal.LoginModel;

namespace GoFishIng.Services
{
    public interface ITripsService
    {
        string CreateTrip(string cartId,string name, string type,int groupSize, string startDate, string endDate);
    }
}
