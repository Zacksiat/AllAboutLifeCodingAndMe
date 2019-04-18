using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class CartOrder
    {
        public int CartOrderId { get; set; }
        public int UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Category { get; set; }
        public int OrderStatus { get; set; }
        public DateTime CreationTimeStamp { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string ExtraInfo { get; set; }
    }
}
