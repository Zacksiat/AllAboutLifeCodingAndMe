using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESHOPFORCODINGSCHOOL.Data;
using ESHOPFORCODINGSCHOOL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESHOPFORCODINGSCHOOL.Controllers
{
    public class MagazineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MagazineController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
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
        public async Task<IActionResult> Create([Bind("Genre,Publisher,DatePublished,Pages,Issue,Name,Price")]Magazine magazine)
        {
            if (ModelState.IsValid)
            {

                Product product = new Product
                {
                    Type = "Magazine",
                    Name = magazine.Name,
                    Price = magazine.Price
                };
                await _context.AddAsync(product);
                magazine.ProductId = product.ProductId;
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
        public async Task<IActionResult> Edit([Bind("ProductId,Genre,Publisher,DatePublished,Pages,Issue,Name,Price")]Magazine magazine)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    Product product = _context.Products.Find(magazine.ProductId);
                    product.Price = magazine.Price;
                    product.Name = magazine.Name;
                    _context.Update(magazine);
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MagazineExists(magazine.ProductId))
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
            return View(magazine);
        }

        private bool MagazineExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
