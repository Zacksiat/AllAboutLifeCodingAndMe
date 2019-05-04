using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPaper.Data;
using EPaper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EPaper.Helpers;
using Microsoft.AspNetCore.Hosting.Internal;
using System.IO;
using EPaper.SSD;

namespace EPaper.Models
{
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HostingEnvironment _hostingEnvironment;

        public BookController(ApplicationDbContext context, HostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;

        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(string category, int page = 1)
        {
            BookViewModel viewModel = new BookViewModel();
            viewModel.Categories = await _context.Books.Select(c => c.Category).Distinct().ToListAsync();

            if (category == null)
            { 
                
                viewModel.Books = await _context.Books
                                            .Include(m => m.Product)
                                            .Where(p => p.Product.Available > 0)
                                            .ToListAsync();
                viewModel.CurrentPage = page;

                return View(viewModel);
            }
            else
            {
                if (_context.Books.Where(p => p.Category == category).Any())
                {
                    viewModel.CurrentCategory = category;
                    viewModel.Books = await _context.Books
                                                      .Include(m => m.Product)
                                                      .Where(p => p.Category == category &&
                                                             p.Product.Available > 0)
                                                             .ToListAsync();
                    viewModel.CurrentPage = page;
                    return View(viewModel);
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BookIndex()
        {
            var applicationDbContext = _context.Books.Include(b => b.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        //GET:/Book/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            return View();
        }

        // POST:/Book/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Author,Publisher,DatePublished,Pages,Category,Product")]Book book)
        {
            if (ModelState.IsValid)
            {

                book.Product.Type = "Book";

                await _context.AddAsync(book);
                await _context.SaveChangesAsync();

                //IMAGE
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var bookFromDb = _context.Books.Find(book.Product.ProductId);

                if (files.Count != 0)
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var filestream = new FileStream(Path.Combine(uploads, book.Product.ProductId + extension), FileMode.Create))//rename the file to the productid /reacts the file to the server
                    {
                        files[0].CopyTo(filestream);//moves the file to the server and renames it
                    }
                    bookFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + book.Product.ProductId + extension;

                }
                else
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                    System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + book.Product.ProductId + ".png");
                    bookFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + book.Product.ProductId + ".png";
                }
                await _context.SaveChangesAsync();


                return RedirectToAction("AdminIndex", "Product");
            }

            return View(book);
        }

        //GET:/Book/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Include(b => b.Product).Where(b => b.ProductId == id).FirstOrDefault() ;
            return View(book);
        }

        // POST:/Book/Edit/4
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ProductId,Author,Publisher,DatePublished,Pages,Category,Product,Product.Name,Product.Price,Product.Description,product.Available")]Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    var bookFromDb = _context.Books.Include(p => p.Product).Where(b => b.ProductId == book.ProductId).FirstOrDefault();

                    bookFromDb.Author = book.Author;
                    bookFromDb.Publisher = book.Publisher;
                    bookFromDb.DatePublished = book.DatePublished; 
                    bookFromDb.Pages = book.Pages;
                    bookFromDb.Category = book.Category;
                    bookFromDb.Product.Name = book.Product.Name;
                    bookFromDb.Product.Price = book.Product.Price;
                    bookFromDb.Product.Description = book.Product.Description;
                    bookFromDb.Product.Available = book.Product.Available;

                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;

                    if (files.Count != 0)
                    {
                        var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                        var extension = Path.GetExtension(files[0].FileName);

                        using (var filestream = new FileStream(Path.Combine(uploads, book.Product.ProductId + extension), FileMode.Create))//rename the file to the productid /reacts the file to the server
                        {
                            files[0].CopyTo(filestream);//moves the file to the server and renames it
                        }
                        bookFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + book.Product.ProductId + extension;

                    }
                    else
                    {
                        var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                        System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + book.Product.ProductId + ".png");
                        bookFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + book.Product.ProductId + ".png";
                    }
               

                    _context.Update(bookFromDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.ProductId))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("AdminIndex", "Product");
            }
            return View(book);
        }

        private bool BookExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return BadRequest();
            }

            return View(product);
        }
        /// <summary>
        ///  POST:/ Delete/Product/id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("BookIndex");
        }


        //GET:/Book/Details/6
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var book = _context.Books.Include(b => b.Product).Where(b => b.ProductId == id).FirstOrDefault() ;
            return View(book);
        }
        
    }
}
