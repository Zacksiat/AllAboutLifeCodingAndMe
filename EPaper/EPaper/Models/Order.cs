using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class Order
    {
        [Required]
        [Key]
        [ForeignKey("Payment")]
        public int PaymentId { get; set; }

        public string UserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string Address { get; set; }

        public IEnumerable<Cart> Carts { get; set; }

        public Payment Payment { get; set; }
    }

}
