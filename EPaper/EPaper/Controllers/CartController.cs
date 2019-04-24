using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPaper.Data;
using EPaper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EPaper.Models
{
    public class CartController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = GetUserId();
                IEnumerable<Cart> carts = _context.Carts
                    .Include(p => p.Product)
                    .Where(c => c.UserId == userId && c.OrderId == null)
                    .ToList();

                return View(carts);
            }
            return RedirectToAction("Index", "Home");


        }

        [HttpGet]
        public async Task<IActionResult> AddtoCart(int? id)
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
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AddtoCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddtoCart([Bind("Id")] Product product)
        {

            if (User.Identity.IsAuthenticated)
            {

                string userId = GetUserId();
                Cart cart = ProductIsInCart(product.ProductId);

                if (cart != null)
                {
                    ++cart.Quantity;
                }
                else
                {
                    cart.UserId = userId;
                    cart.ProductId = product.ProductId;
                    cart.Quantity = 1;
                    _context.Carts.Add(cart);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Products");
        }

        // GET: Basket / Delete / 5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                string userId = GetUserId();
                var product = ProductIsInCart(id);

                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
            return View();
        }

        // POST: Basket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (User.Identity.IsAuthenticated)
            {
                string userId = GetUserId();
                var product = ProductIsInCart(id);

                if (product == null)
                {
                    return NotFound();
                }

                if (product.Quantity <= 1)
                {
                    _context.Carts.Remove(product);
                }
                else
                {
                    product.Quantity = --product.Quantity;
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        // GET : /CheckOut
        public IActionResult CheckOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                RedirectToAction("Create", "Payment");

            }

            return NotFound();
        }

        private string GetUserId()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId;
        }

        private Cart ProductIsInCart(int? id)
        {
            string userId = GetUserId();
            var product =  _context.Carts
              .FirstOrDefault(i => i.ProductId == id
                                && i.UserId == userId
                                && i.OrderId == null);
            return product;
        }
    }
}
