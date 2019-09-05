using GoFishIng.Data.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GoFishIng.Services
{
    public interface IApplicationUsersService
    {
        //string GetId(ClaimsPrincipal principal);
        //string GetTheIdOfUser();
        //Task<ApplicationUser> GetCurrentUserAsync();
        ApplicationUser GetUserOrNull(string username, string password);

    }
}
