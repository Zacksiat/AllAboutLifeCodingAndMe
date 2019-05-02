using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class ComicViewModel
    {
        public List<string> Categories { get; set; }

        public List<Comic> Comics { get; set; }
    }
}
