using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class CartViewModel
    {
        public CartViewModel()
        {
            Carts = new List<Cart>();
            UnavailableProducts = new List<Product>();
            EnablePopUpWarning = false;
        }

        public List<Cart> Carts { get; set; }

        public bool EnablePopUpWarning { get; set; }

        public List<Product> UnavailableProducts { get; set; }
    }
}
