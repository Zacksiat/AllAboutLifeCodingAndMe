using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPaper.Data;
using EPaper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EPaper.Models
{
    public class MagazineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MagazineController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string category)
        {
            MagazineViewModel viewModel = new MagazineViewModel();
            viewModel.Categories = await _context.Magazines.Select(c => c.Category).Distinct().ToListAsync();

            if (category == null)
            {
                viewModel.Magazines = await _context.Magazines
                                            .Include(m => m.Product)
                                            .Where(p => p.Product.Available > 0)
                                            .ToListAsync();
                return View(viewModel);
            }
            else
            {
                if (_context.Magazines.Where(p => p.Category == category).Any())
                {

                    viewModel.Magazines = await _context.Magazines
                                                      .Include(m => m.Product)
                                                      .Where(p => p.Category == category &&
                                                             p.Product.Available > 0)
                                                             .ToListAsync();
                    return View(viewModel);
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        public async Task<IActionResult> MagazineIndex()
        {

            var applicationDbContext = _context.Magazines.Include(m => m.Product);
            return View(await applicationDbContext.ToListAsync());
        }
        //GET:/Magazine/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            return View();
        }

        // POST:/Magazine/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Genre,Publisher,DatePublished,Pages,Issue,Category,Product")]Magazine magazine)
        {
            if (ModelState.IsValid)
            {
                magazine.Product.Type = "Magazine";
                await _context.AddAsync(magazine);
                await _context.SaveChangesAsync();

                return RedirectToAction("AdminIndex", "Product");
            }

            return View(magazine);
        }

        //GET:/Magazine/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Product product)
        {
            var magazine = _context.Magazines.Find(product.ProductId);
            return View(magazine);
        }

        // POST:/Magazine/Edit/4
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Genre,Publisher,DatePublished,Pages,Issue,Product")]Magazine magazine)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(magazine);
                    //Product product = _context.Products.Find(magazine.ProductId);
                    //product.Price = magazine..Price;
                    //product.Name = magazine.Name;
                    //_context.Update(magazine);
                    //_context.Update(product);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MagazineExists(magazine.ProductId))
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
            return View(magazine);
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
            return RedirectToAction("MagazineIndex");
        }

        private bool MagazineExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

    }
}
