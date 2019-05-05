using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class Cd
    {
        [Required]
        [Key]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public string Category { get; set; }
        public string Artist { get; set; }
        //diskografiki
        public string Label { get; set; }
        public int NumberOfSongs { get; set; }

        public Product Product { get; set; }
      
    }
}
