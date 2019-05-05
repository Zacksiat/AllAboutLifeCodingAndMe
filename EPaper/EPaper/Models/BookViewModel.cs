using EPaper.Helpers;
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

        public string CurrentCategory { get; set; }

        public int CurrentPage { get; set; }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(Books.Where(p => p.Product.Available > 0).Count() / (double)Constants.PRODUCTS_PER_PAGE));

        }
        public List<Book> PaginatedBooks()
        {
            int start = (CurrentPage - 1) * Constants.PRODUCTS_PER_PAGE;
            return Books.OrderBy(p => p.ProductId).Skip(start).Take(Constants.PRODUCTS_PER_PAGE).ToList();
        }
    }

}
