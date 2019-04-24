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
        public IActionResult Index()
        {
            var cds = _context.Cds.ToList();
            cds.Add(new Cd() { Name = "TERLEGKAS" });
            return View(cds);
        }

        public async Task<IActionResult> CdIndex()
        {

            var applicationDbContext = _context.Cds;
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
        public async Task<IActionResult> Create([Bind("Genre,Artist,Label,NumberOfSongs,Name,Price")]Cd cd)
        {
            if (ModelState.IsValid)
            {

                Product product = new Product
                {
                    Type = "Cd",
                    Name = cd.Name,
                    Price = cd.Price
                };
                await _context.AddAsync(product);
                cd.ProductId = product.ProductId;
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
        public async Task<IActionResult> Edit([Bind("ProductId,Genre,Artist,Label,NumberOfSongs,Name,Price")]Cd cd)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Product product = _context.Products.Find(cd.ProductId);
                    product.Price = cd.Price;
                    product.Name = cd.Name;
                    _context.Update(cd);
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CdExists(cd.ProductId))
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
            return View(cd);
        }

        private bool CdExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
