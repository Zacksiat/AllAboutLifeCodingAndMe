using EPaper.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Controllers
{
    
        public class ProductsViewModel
        {
          //public Product Products { get; set; }
            public IEnumerable<Product> Product { get; set; }


        }
    
}