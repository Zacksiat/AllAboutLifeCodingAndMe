using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class Order
    {
        public int PaymentId { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string Address { get; set; }

        public Cart Cart { get; set; }

        public Payment Payment { get; set; }
    }

}
