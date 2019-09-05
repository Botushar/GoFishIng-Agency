using GoFishIng.Data;
using GoFishIng.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoFishIng.Services
{
    public class CartsService : ICartsService
    {
        public readonly ApplicationDbContext db;
        //private readonly IApplicationUsersService applicationUsersServices;

        public CartsService(ApplicationDbContext db)
        {
            this.db = db;
           // this.applicationUsersServices = applicationUsersServices;
            
        }


        public string CreateCart(string userId)
        {
            //var userIdToEnter = this.applicationUsersServices.GetUserById(userId);

            var cart = new Cart
            {
                UserId = userId,

            };

            this.db.Carts.Add(cart);
            this.db.SaveChanges();

            return cart.Id;
       
        }

        public Cart GetCartByUserId(string id)
        {
            //var cartId =this.db.Carts.Find(id);

            var theCart = this.db.Carts
                              .Include(u => u.User)
                              .ThenInclude(ut => ut.UserTrips)
                              .Include(t=>t.Trips)
                              .Include(p=>p.Products)
                              .SingleOrDefault(u => u.UserId == id);

            //var theUser = this.db.Users.FirstOrDefault(u => u.Id == id);
            //var currentCartId = this.db.Carts.FirstOrDefault(c => c.UserId == id);

            return theCart;


        }
    }
}
