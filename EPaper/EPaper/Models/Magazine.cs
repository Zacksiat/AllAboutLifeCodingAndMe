using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class Magazine
    {
        [Required]
        [Key]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public string Publisher { get; set; }

        public string Genre { get; set; }

        public int Pages { get; set; }

        public int Issue { get; set; }

        public DateTime DatePublished { get; set; }

        public Product Product { get; set; }
        public string TypeName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
    }
}
