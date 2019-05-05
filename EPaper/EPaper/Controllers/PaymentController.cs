using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPaper.Data;
using EPaper.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EPaper.Models
{
    [Authorize]
    public class PaymentController : Controller
    {

        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET : /Payment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST :/Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Payment,Address")]Order order)
        {
            if (ModelState.IsValid)
            {

                var userId = GetUserId();
                var carts = GetUserCarts();
                if (carts == null)
                {
                    return RedirectToAction("Index", "Cart");
                }
                if (Helper.GetUnavailableProducts(_context, carts).Count == 0)
                {
                    order.UserId = userId;
                    order.Payment.Total = CountTotal(userId);
                    order.Payment.UserId = userId;
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();
                    foreach (var cart in carts)
                    {
                        cart.OrderId = order.PaymentId;
                        _context.Carts.Update(cart);
                        _context.SaveChanges();
                        
                    }
                    await _context.SaveChangesAsync();
                    ReduceProductStock(carts);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    return BadRequest();
                }

            }
            return RedirectToAction("Index", "Order");
        }




        private double CountTotal(string userId)
        {

            var products = _context.Carts
             .Include(p => p.Product)
             .Where(c => c.UserId == userId && c.OrderId == null)
             .ToList();

            double total = 0;
            foreach (var item in products)
            {
                total += item.Product.Price * item.Quantity;
            }

            return total;
        }
        private List<Cart> GetUserCarts()
        {
            var userId = GetUserId();
            List<Cart> userCarts = _context.Carts
                                                        .Include(c => c.Product)
                                                        .Where(c => c.UserId == userId 
                                                                  && c.OrderId == null)
                                                        .ToList();
            if (userCarts.Count > 0)
            {
                return userCarts;
            }
            return null;
        }

        private void ReduceProductStock(List<Cart> userCarts)
        {
            foreach (var cart in userCarts)
            {
                var products = _context.Products.Where(p => p.ProductId == cart.ProductId).ToList();
                foreach (var prod in products)
                {
                    prod.Available -= cart.Quantity;
                    _context.Products.Update(prod);
                }

                _context.SaveChanges();
            }
        }

        private string GetUserId()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId;
        }

    }

}
