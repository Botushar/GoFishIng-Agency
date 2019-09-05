using GoFishIng.Data;
using GoFishIng.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GoFishIng.Services
{
    public class ApplicationUsersService:IApplicationUsersService
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ApplicationUsersService(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        //public string GetId()
        //{

        //    var id = this.userManager.GetUserId();
        //    return id;
        //}

        //public string GetTheIdOfUser()
        //{
        //    //    //var userId = this.httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x=>x.Properties.Values.).FindFirstValue(ClaimTypes.NameIdentifier);
        //    //    var userId = this.httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    var user = this.userManager.GetUserAsync(this.httpContextAccessor.HttpContext.User);
        //    var id = user.Result.Id;
        //    return id;
        //}

        //public async Task<ApplicationUser> GetCurrentUserAsync()
        //{
        //    ClaimsPrincipal principal = new ClaimsPrincipal();
        //    var currentUser = this.userManager.GetUserAsync(principal);

        //    return await currentUser;

        //}

        public ApplicationUser GetUserOrNull(string username, string password)
        {
            var passwordHash = this.HashPassword(password);
            var user = this.db.Users.FirstOrDefault(
                x => x.UserName == username
                && x.PasswordHash == passwordHash);
            return user;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return Encoding.UTF8.GetString(sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }


    }
}
