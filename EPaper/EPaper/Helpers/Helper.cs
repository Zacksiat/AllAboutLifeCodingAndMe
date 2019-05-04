using EPaper.Data;
using EPaper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Helpers
{
    public static class Helper
    {
        public static List<Product> GetUnavailableProducts(ApplicationDbContext _context, List<Cart> userCarts)
        {

            List<Product> unavailableProducts = new List<Product>();
            foreach (var cart in userCarts)
            {
                var product = _context.Products.Where(p => p.ProductId == cart.ProductId).First();
                if (product.Available < cart.Quantity)
                    unavailableProducts.Add(product);
            }

            return unavailableProducts;
        }
    }
}
