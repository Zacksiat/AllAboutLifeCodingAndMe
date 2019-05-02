using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class MagazineViewModel
    {
       public List<string> Categories { get; set; }
        
       public List<Magazine> Magazines { get; set; }
    }
}
