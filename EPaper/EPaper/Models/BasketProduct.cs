using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESHOPFORCODINGSCHOOL.Models
{
    public class BasketProduct
    {
        public int BasketId { get; set; }
        public Basket Basket { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
