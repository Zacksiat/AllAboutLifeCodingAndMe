using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class LookUpTable
    {
        public int LookUpTableId { get; set; }
        public string RecKey { get; set; }
        public int RecCode { get; set; }
        public string Description { get; set; }
        public DateTime CreationTimeStamp { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string ExtraInfo { get; set; }
    }
}
