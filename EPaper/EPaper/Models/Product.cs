using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class Product
    {
        public int ProductId { get; set; }
      
        public string Type { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool Available { get; set; }
        public string Image { get; set; }


    }

    
}

