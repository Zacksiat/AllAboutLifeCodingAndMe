using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPaper.Data;
using EPaper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace EPaper.Models
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;



        public ProductController(ApplicationDbContext context)
        {
            _context = context;
            //ProductsVM = new ProductsViewModel()
            {
                //Type = _context.Type.ToList(),

                //Product = new Models.Product()

            };
        }



        //////////////////////////ADMIN////////////////////////
        // GET: Product/Admin
        [Authorize(Roles = "Admin")]
        [HttpGet("Product/Admin")]
        public async Task<IActionResult> AdminIndex()
        {

            var applicationDbContext = _context.Products;
            return View(await applicationDbContext.ToListAsync());
        }

        //GET:Products Create
        public IActionResult Create()
        {
            return View();
        }

    }
}

        //POST: Products Create

        /*[HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            //ean to model state den einai valid tha gurisoume sto view ton model
            if (!ModelState.IsValid)
            {
                return View();
            }

            //allios tha to prosthesoyme sth vasi
            _context.Products.Add(ProductsVM.Product);
            await _context.SaveChangesAsync();
            /*
            //Image being saved
            //string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productsFromDb = _context.Products.Find(ProductsVM.Products.Id);
            if (files.Count != 0)
            {
                //Image has been uploaded
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                //find out the extention of the file
                var extension = Path.GetExtension(files[0].FileName);

                //copy the file from the uploaded to the server
                using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + extension;

            }
            else
            {
                //a dummy image which will always viewsin case the user doesnot apply any file
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png");
                //
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png";
            }
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AdminIndex));
        }
        */


       // [Authorize(Roles = "Admin")]
        /*public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductsVM.Product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            if (ProductsVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductsVM);
        }
        */

        //POST:Edit
      //  [Authorize(Roles = "Admin")]
      //  [HttpPost]
     //   [ValidateAntiForgeryToken]
        /*public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                //string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;


                var product = _context.Products.FirstOrDefault(m=>m.ProductId==id);
                
                /* update image
                if (files.Count > 0 && files[0] != null)
                {
                    //if user uploads a new image
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(productsFromDb.Image);
                    //delete the old image
                    if (System.IO.File.Exists(Path.Combine(uploads, ProductsVM.Products.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, ProductsVM.Products.Id + extension_old));

                    }
                    //add the newone
                    using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    ProductsVM.Products.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + extension_new;

                }
                //save to the database
                if (ProductsVM.Products.Image != null)
                {
                    productsFromDb.Image = ProductsVM.Products.Image;
                }
                */
                /*product.Name = ProductsVM.Product.Name;
                product.Price = ProductsVM.Product.Price;
                product.Available = ProductsVM.Product.Available;
               
               // productsFromDb.Pages = ProductsVM.Products.Pages;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(AdminIndex));


            }

            return View(ProductsVM);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
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
            return RedirectToAction("AdminIndex");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 

        /*
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
       
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            string type = product.Type;
            switch (type)
            {
                case "Book":
                    return RedirectToAction("Edit","Book",product);
                case "Magazine":
                    return RedirectToAction("Edit", "Magazine",product);
                case "Cd":
                    return RedirectToAction("Edit", "Cd",product);
                default:
                    return RedirectToAction("AdminIndex");

            }

            
        }*/
    //}
//}