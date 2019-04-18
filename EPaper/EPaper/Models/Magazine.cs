using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class Magazine
    {
        public int MagazineId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public int Pages { get; set; }
        public int ISBN { get; set; }
        public int TotalQuantity { get; set; }
        public DateTime CreationTimeStamp { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string ExtraInfo { get; set; }
    }
}
