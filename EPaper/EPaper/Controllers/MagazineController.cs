using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EPaper.Data;
using EPaper.Models;
using EPaper.SSD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EPaper.Models
{
    [Authorize(Roles = "Admin")]
    public class MagazineController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HostingEnvironment _hostingEnvironment;

        public MagazineController(ApplicationDbContext context, HostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;

        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string category, int page = 1)
        {
            MagazineViewModel viewModel = new MagazineViewModel();
            viewModel.Categories = await _context.Magazines.Select(c => c.Category).Distinct().ToListAsync();

            if (category == null)
            {

                viewModel.Magazines = await _context.Magazines
                                            .Include(m => m.Product)
                                            .Where(p => p.Product.Available > 0)
                                            .ToListAsync();
                viewModel.CurrentPage = page;
                return View(viewModel);
            }
            else
            {
                if (_context.Magazines.Where(p => p.Category == category).Any())
                {
                    viewModel.CurrentCategory = category;
                    viewModel.Magazines = await _context.Magazines
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


                //IMAGE
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var magazineFromDb = _context.Magazines.Find(magazine.Product.ProductId);
                if (files.Count != 0)
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var filestream = new FileStream(Path.Combine(uploads, magazine.Product.ProductId + extension), FileMode.Create))//rename the file to the productid /reacts the file to the server
                    {
                        files[0].CopyTo(filestream);//moves the file to the server and renames it
                    }
                    magazineFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + magazine.Product.ProductId + extension;

                }
                else
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                    System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + magazine.Product.ProductId + ".png");
                    magazineFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + magazine.Product.ProductId + ".png";
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("AdminIndex", "Product");
            }

            return View(magazine);
        }

        //GET:/Magazine/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var magazine = _context.Magazines.Include(m => m.Product).Where(m => m.ProductId == id).FirstOrDefault();
            return View(magazine);
        }

        // POST:/Magazine/Edit/4
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Category,Publisher,DatePublished,Pages,Issue,Product,ProductId,Product.Price,Product.Available,Product.Name,Product.Description")]Magazine magazine)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var magazineFromDb = _context.Magazines.Include(p => p.Product).Where(b => b.ProductId == magazine.ProductId).FirstOrDefault();

                    magazineFromDb.Publisher = magazine.Publisher;
                    magazineFromDb.DatePublished = magazine.DatePublished;
                    magazineFromDb.Pages = magazine.Pages;
                    magazineFromDb.Category = magazine.Category;
                    magazineFromDb.Product.Name = magazine.Product.Name;
                    magazineFromDb.Product.Price = magazine.Product.Price;
                    magazineFromDb.Product.Description = magazine.Product.Description;
                    magazineFromDb.Product.Available = magazine.Product.Available;

                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;

                    if (files.Count != 0)
                    {
                        var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                        var extension = Path.GetExtension(files[0].FileName);

                        using (var filestream = new FileStream(Path.Combine(uploads, magazine.Product.ProductId + extension), FileMode.Create))//rename the file to the productid /reacts the file to the server
                        {
                            files[0].CopyTo(filestream);//moves the file to the server and renames it
                        }
                        magazineFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + magazine.Product.ProductId + extension;

                    }
                    else
                    {
                        var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                        System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + magazine.Product.ProductId + ".png");
                        magazineFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + magazine.Product.ProductId + ".png";
                    }


                    _context.Update(magazineFromDb);
                    await _context.SaveChangesAsync();
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

        //GET:/Magazine/Edit/6
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var magazine = _context.Magazines.Include(m => m.Product).Where(m => m.ProductId == id).FirstOrDefault();
            return View(magazine);
        }

    }

}

