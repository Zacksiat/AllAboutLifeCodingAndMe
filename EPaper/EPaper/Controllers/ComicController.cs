using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPaper.Data;
using EPaper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EPaper.Controllers
{
    public class ComicController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComicController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var applicationDbContext = _context.Comics.Include(c=>c.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> ComicIndex()
        {

            var applicationDbContext = _context.Comics.Include(c=>c.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        //GET:/Comic/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            return View();
        }

        // POST:/Comic/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Author,Label,Publisher,Category,Pages,Product")]Comic comic)
        {
            if (ModelState.IsValid)
            {
                comic.Product.Type = "Comic";
                await _context.AddAsync(comic.Product);
                comic.ProductId = comic.Product.ProductId;
                await _context.AddAsync(comic);
                await _context.SaveChangesAsync();

                return RedirectToAction("AdminIndex", "Product");
            }

            return View(comic);
        }

        //GET:/Comic/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Product product)
        {
            var comic = _context.Comics.Find(product.ProductId);
            return View(comic);
        }

        // POST:/Comic/Edit/4
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ProductId,Genre,Artist,Label,NumberOfSongs,Publisher,Product")]Comic comic)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComicExists(comic.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("AdminIndex", "Product");
            }
            return View(comic);
        }

        private bool ComicExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}