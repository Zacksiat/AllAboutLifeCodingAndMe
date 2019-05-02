using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class BookViewModel
    {
        public List<string> Categories { get; set; }

        public List<Book> Books { get; set; }
    }
}
