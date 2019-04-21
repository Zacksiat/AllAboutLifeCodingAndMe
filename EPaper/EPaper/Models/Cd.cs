﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ESHOPFORCODINGSCHOOL.Models
{
    public class Cd
    {
        [Required]
        [Key]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public string Genre { get; set; }
        public string Artist { get; set; }
        public string Label { get; set; }
        public int NumberOfSongs { get; set; }

        public Product Product { get; set; }
        public string TypeName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
    }
}
