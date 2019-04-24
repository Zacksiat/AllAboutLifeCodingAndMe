using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ProductId { get; set; }
        
        public int Quantity { get; set; }

        public int? OrderId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        
        public Product Product { get; set; }

        public Order Order { get; set; }
    }
}
