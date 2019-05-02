using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class CdViewModel
    {
        public List<string> Categories { get; set; }

        public List<Cd> Cds { get; set; }
    }
}
