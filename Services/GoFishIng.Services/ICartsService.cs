using GoFishIng.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoFishIng.Services
{
    public interface ICartsService
    {
        Cart GetCartByUserId(string id);

        string CreateCart(string userId);
    }
}
