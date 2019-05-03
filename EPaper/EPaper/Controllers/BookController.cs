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

namespace EPaper.Models
{
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
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

                return RedirectToAction("AdminIndex", "Product");
            }

            return View(book);
        }

        //GET:/Book/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Product product)
        {
            var book = _context.Books.Find(product.ProductId);
            return View(book);
        }

        // POST:/Book/Edit/4
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ProductId,Author,Publisher,DatePublished,Pages,Category,Name,Price")]Book book)
        {


            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(book);
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

    }
}
