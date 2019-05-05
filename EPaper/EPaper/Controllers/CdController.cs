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
    public class CdController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HostingEnvironment _hostingEnvironment;

        public CdController(ApplicationDbContext context, HostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(string category, int page = 1)
        {
            CdViewModel viewModel = new CdViewModel();
            viewModel.Categories = await _context.Cds.Select(c => c.Category).Distinct().ToListAsync();

            if (category == null)
            {
                viewModel.Cds = await _context.Cds
                                            .Include(m => m.Product)
                                            .Where(p => p.Product.Available > 0)
                                            .ToListAsync();
                viewModel.CurrentPage = page;
                return View(viewModel);
            }
            else
            {
                if (_context.Cds.Where(p => p.Category == category).Any())
                {
                    viewModel.CurrentCategory = category;
                    viewModel.Cds = await _context.Cds
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
        //Index gia to cd
        public async Task<IActionResult> CdIndex()
        {

            var applicationDbContext = _context.Cds.Include(c => c.Product);
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
        public async Task<IActionResult> Create([Bind("Artist,Label,NumberOfSongs,Product,Category")]Cd cd)
        {
            if (ModelState.IsValid)
            {
                cd.Product.Type = "Cd";
                await _context.AddAsync(cd);
                await _context.SaveChangesAsync();

                //IMAGE
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;


                var cdFromDb = _context.Cds.Find(cd.Product.ProductId);
                if (files.Count != 0)
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var filestream = new FileStream(Path.Combine(uploads, cd.Product.ProductId + extension), FileMode.Create))//rename the file to the productid /reacts the file to the server
                    {
                        files[0].CopyTo(filestream);//moves the file to the server and renames it
                    }
                    cdFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + cd.Product.ProductId + extension;

                }
                else
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                    System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + cd.Product.ProductId + ".png");
                    cdFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + cd.Product.ProductId + ".png";
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("AdminIndex", "Product");
            }

            return View(cd);
        }

        //GET:/Cd/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var cd = _context.Cds.Include(c => c.Product).Where(c => c.ProductId == id).FirstOrDefault();
            return View(cd);
        }

        // POST:/Cd/Edit/4
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ProductId,Artist,Label,NumberOfSongs,Product,Category,Product.Name,Product.Price,Product.Description")]Cd cd)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var cdFromDb = _context.Cds.Include(p => p.Product).Where(b => b.ProductId == cd.ProductId).FirstOrDefault();
                    cdFromDb.Artist = cd.Artist;
                    cdFromDb.Label = cd.Label;
                    cdFromDb.NumberOfSongs = cd.NumberOfSongs;
                    cdFromDb.Product.Name = cd.Product.Name;
                    cdFromDb.Product.Price = cd.Product.Price;
                    cdFromDb.Product.Description = cd.Product.Description;
                    cdFromDb.Product.Available = cd.Product.Available;
                    cdFromDb.Category = cd.Category;
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;

                    if (files.Count != 0)
                    {
                        var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                        var extension = Path.GetExtension(files[0].FileName);

                        using (var filestream = new FileStream(Path.Combine(uploads, cd.Product.ProductId + extension), FileMode.Create))//rename the file to the productid /reacts the file to the server
                        {
                            files[0].CopyTo(filestream);//moves the file to the server and renames it
                        }
                        cdFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + cd.Product.ProductId + extension;

                    }
                    else
                    {
                        var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                        System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + cd.Product.ProductId + ".png");
                        cdFromDb.Product.Image = @"\" + SD.ImageFolder + @"\" + cd.Product.ProductId + ".png";
                    }


                    _context.Update(cdFromDb);
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
            return RedirectToAction("CdIndex");
        }

        //GET:/Cd/Details/6
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var cd = _context.Cds.Include(c => c.Product).Where(c => c.ProductId == id).FirstOrDefault();
            return View(cd);
        }


    }
}
