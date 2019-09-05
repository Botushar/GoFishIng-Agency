using GoFishIng.Common;
using GoFishIng.Data;
using GoFishIng.Data.Models;
using GoFishIng.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoFishIng.Services
{
    public class ProductsService : IProductsService
    {
        private readonly ApplicationDbContext db;

        public ProductsService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public Product ProductsView(string name,decimal price, int quantity,string type, bool inStock, string cartId, string orderId)
        {
            var product = new Product
            {
                Name = name,
                Price = price,
                Quantity=quantity,
                InStock = quantity<=GlobalConstants.ItemsQuantity?true:false,
                Type = (ProductType)Enum.Parse(typeof(ProductType), type, true),
                CartId = cartId,
                OrderId = orderId,

            };

            this.db.Products.Add(product);
            this.db.SaveChanges();

            return product;
        } 

    }
}
