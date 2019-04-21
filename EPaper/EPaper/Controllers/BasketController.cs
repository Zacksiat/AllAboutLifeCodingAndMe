using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPaper.Data;
using EPaper.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EPaper.Controllers
{
    public class BasketController : Controller
    {

        private readonly ApplicationDbContext _context;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public BasketController(ApplicationDbContext context)
        {
            _context = context;

        }
        /// <summary>
        ///  GET :/Basket
        /// </summary>
        /// <returns></returns>
        public IActionResult Index(string i)
        {

            var tempUserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            IEnumerable<BasketProduct> result = new List<BasketProduct>();
            result = _context.BasketProducts

                .Include(bp => bp.Basket)
                .Include(bp => bp.Product)
                .Where(bp => bp.Basket.ApplicationUserId == tempUserID).ToList();
            //if (result.Count() == 0)
            //    return NotFound();
            //else
            return View(result);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddtoCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products
               .Include(p => p.Type)
               .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        /// <summary>
        ///  Create new basket for current user if not exists and adds product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AddtoCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddtoCart([Bind("Id", "Name", "Price")] Product product)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userName = User.Identity.Name;
            Basket basket = null;
            //  Check if user has basket
            basket = await _context.Baskets.Where(u => u.ApplicationUserId == userId).FirstOrDefaultAsync();

            // if user doesnt have basket create and add the item
            if (basket == null)
            {
                basket = new Basket();
                basket.ApplicationUserId = userId;
                //      basket.Name= userName;
                _context.Baskets.Add(basket);
                _context.SaveChanges();
                BasketProduct prod = new BasketProduct { ProductId = product.ProductId, BasketId = basket.BasketId, Quantity = 1 };
                _context.BasketProducts.Add(prod);
                await _context.SaveChangesAsync();
            }
            else // if user has basket check if item has already been added before and update quantity if he has 
            {
                if (_context.BasketProducts.Where(p => p.ProductId == product.ProductId && p.BasketId == basket.BasketId).Any())
                {
                    BasketProduct basketproducts = await _context.BasketProducts.Where(p => p.ProductId == product.ProductId && p.BasketId == basket.BasketId).FirstAsync();

                    basketproducts.Quantity = ++basketproducts.Quantity;
                    _context.BasketProducts.Update(basketproducts);
                    await _context.SaveChangesAsync();
                }
                else //if user doesnt have the item in the basket add it with quantity 1
                {
                    BasketProduct prod = new BasketProduct { ProductId = product.ProductId, BasketId = basket.BasketId, Quantity = 1 };
                    _context.BasketProducts.Add(prod);
                    await _context.SaveChangesAsync();
                }
            }
            // finally return to Products/Index 
            return RedirectToAction("Index", "Products");
        }

        // GET: Basket/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var bid = _context.Baskets
                // .Include(b => b.IdentityUserId)
                .Where(b => b.ApplicationUserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .Select(b => b.BasketId)
                .First();

            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.BasketProducts
            .Include(p => p.Product)
           .FirstOrDefaultAsync(i => i.ProductId == id && i.BasketId == bid);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Basket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int productid, int basketId)
        {
            var product = await _context.BasketProducts.FindAsync(basketId, productid);
            if (product.Quantity <= 1)
            {
                _context.BasketProducts.Remove(product);
            }
            else
            {
                product.Quantity = --product.Quantity;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("CheckOut")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            Basket basket = await _context.Baskets.FirstAsync(i => i.ApplicationUserId == User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Order order = new Order();
            order.ApplicationUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _context.Orders.AddAsync(order);

            OrderProduct orderProducts = new OrderProduct();

            //   orderProducts.OrderId = order.OrderId;
            //  IEnumerable
            ///       await _context.OrderProducts.AddAsync();

            _context.Baskets.Remove(basket);
            await _context.SaveChangesAsync();
            return View(order);
        }
    }
}
