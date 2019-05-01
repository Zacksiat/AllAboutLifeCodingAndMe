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
            if (category == null)
            {
                var applicationDbContext = _context.Magazines
                                                   .Include(m => m.Product)
                                                   .Where(p => p.Product.Available > 0);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                if (_context.Magazines.Where(p => p.Category == category).Any())
                {
                    var applicationDbContext = _context.Magazines
                                                       .Include(m => m.Product)
                                                       .Where(p => p.Category == category &&
                                                              p.Product.Available > 0);
                    return View(await applicationDbContext.ToListAsync());
                }
                else
                {
                    return BadRequest();
                }
             
                
            }
        
        }

        public async Task<IActionResult> MagazineIndex()
        {

            var applicationDbContext = _context.Magazines.Include(m=>m.Product);
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

        private bool MagazineExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
