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
    [Authorize(Roles ="Admin")]
    public class CdController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CdController(ApplicationDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(string category)
        {
            if (category == null)
            {
                var applicationDbContext = _context.Cds
                                                   .Include(m => m.Product)
                                                   .Where(p => p.Product.Available > 0);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                if (_context.Cds.Where(p => p.Category == category).Any())
                {
                    var applicationDbContext = _context.Cds
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
        //Index gia to cd
        public async Task<IActionResult> CdIndex()
        {

            var applicationDbContext = _context.Cds.Include(c=>c.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        //GET:/Cd/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            return View();
        }

        // POST:/Cd/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Genre,Artist,Label,NumberOfSongs,Product")]Cd cd)
        {
            if (ModelState.IsValid)
            {
                cd.Product.Type = "Cd"; 
                await _context.AddAsync(cd);
                await _context.SaveChangesAsync();

                return RedirectToAction("AdminIndex", "Product");
            }

            return View(cd);
        }

        //GET:/Cd/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Product product)
        {
            var cd = _context.Cds.Find(product.ProductId);
            return View(cd);
        }

        // POST:/Cd/Edit/4
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ProductId,Genre,Artist,Label,NumberOfSongs,Product")]Cd cd)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(cd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CdExists(cd.ProductId))
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
            return View(cd);
        }

        private bool CdExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
