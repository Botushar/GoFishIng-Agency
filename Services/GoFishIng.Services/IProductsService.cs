using GoFishIng.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoFishIng.Services
{
    public interface IProductsService
    {
        Product ProductsView(string name, decimal price, int quantity, string type, bool inStock, string cartId, string orderId);
    }
}
