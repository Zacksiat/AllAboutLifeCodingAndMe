using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class Book
    {
        [Required]
        [Key]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [Required]
        public string Author { get; set; }
        [Required] 
        public string Publisher { get; set; }

        public DateTime DatePublished { get; set; }
        [Required]
        public string Category { get; set; }

        public int Pages { get; set; }

        public Product Product { get; set; }
      
    }
}
