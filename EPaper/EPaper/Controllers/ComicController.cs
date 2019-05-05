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

namespace EPaper.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ComicController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HostingEnvironment _hostingEnvironment;

        public ComicController(ApplicationDbContext context, HostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string category, int page = 1)
        {
            ComicViewModel viewModel = new ComicViewModel();
            viewModel.Categories = await _context.Comics.Select(c => c.Category).Distinct().ToListAsync();

            if (category == null)
            {
                viewModel.Comics = await _context.Comics
                                            .Include(m => m.Product)
                                            .Where(p => p.Product.Available > 0)
                                            .ToListAsync();
                viewModel.CurrentPage = page;
                return View(viewModel);
            }
            else
            {
                if (_context.Comics.Where(p => p.Category == category).Any())
                {
                    viewModel.CurrentCategory = category;
                    viewModel.Comics = await _context.Comics
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
        public async Task<IActionResult> ComicIndex()
        {

            var applicationDbContext = _context.Comics.Include(c => c.Product);
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
                await _context.AddAsync(comic);
                await _context.SaveChangesAsync();


                //IMAGE
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var comicFromDb = _context.Comics.Find(comic.Product.ProductId);
                if (files.Count != 0)
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var filestream = new FileStream(Path.Combine(uploads, comic.Product.ProductId + extension), FileMode.Create))//rename the file to the productid /reacts the file to the server
                    {
                        files[0].CopyTo(filestream);//moves the file to the server and renames it
                    }
                    comicFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + comic.Product.ProductId + extension;

                }
                else
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                    System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + comic.Product.ProductId + ".png");
                    comicFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + comic.Product.ProductId + ".png";
                }
                await _context.SaveChangesAsync();


                return RedirectToAction("AdminIndex", "Product");
            }

            return View(comic);
        }

        //GET:/Comic/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var comic = _context.Comics.Include(c => c.Product).Where(c => c.ProductId == id).FirstOrDefault();
            return View(comic);
        }

        // POST:/Comic/Edit/4
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Author,Publisher,DatePublished,Category,Pages,Product,ProductId,Product.Name,Product.Price,Product.Description,Product.Available")]Comic comic)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var comicFromDb = _context.Comics.Include(p => p.Product).Where(b => b.ProductId == comic.ProductId).FirstOrDefault();

                    comicFromDb.Author = comic.Author;
                    comicFromDb.Publisher = comic.Publisher;
                    comicFromDb.DatePublished = comic.DatePublished;
                    comicFromDb.Pages = comic.Pages;
                    comicFromDb.Category = comic.Category;
                    comicFromDb.Product.Name = comic.Product.Name;
                    comicFromDb.Product.Price = comic.Product.Price;
                    comicFromDb.Product.Description = comic.Product.Description;
                    comicFromDb.Product.Available = comic.Product.Available;

                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;

                    if (files.Count != 0)
                    {
                        var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                        var extension = Path.GetExtension(files[0].FileName);

                        using (var filestream = new FileStream(Path.Combine(uploads, comic.Product.ProductId + extension), FileMode.Create))//rename the file to the productid /reacts the file to the server
                        {
                            files[0].CopyTo(filestream);//moves the file to the server and renames it
                        }
                        comicFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + comic.Product.ProductId + extension;

                    }
                    else
                    {
                        var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                        System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + comic.Product.ProductId + ".png");
                        comicFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + comic.Product.ProductId + ".png";
                    }


                    _context.Update(comicFromDb);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!ComicExists(comic.ProductId))
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
            return View(comic);
        }

        private bool ComicExists(int id)
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
            return RedirectToAction("ComicIndex");
        }

        //GET:/Comic/Details/6
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var comic = _context.Comics.Include(p => p.Product).Where(p => p.ProductId == id).FirstOrDefault();
            return View(comic);
        }



    }
}