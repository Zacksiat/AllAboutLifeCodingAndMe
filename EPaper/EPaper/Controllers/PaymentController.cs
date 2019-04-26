using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPaper.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EPaper.Models
{
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
        public async Task<IActionResult> Create([Bind("PaymentMethod,Orders.Address")]Payment payment)
        {
            if (ModelState.IsValid)
            {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var carts = GetUserCartProducts();

                if (CheckProductAvailability(carts))
                {
                    payment.UserId = userId;
                    payment.Order.ApplicationUserId = userId;
                    payment.Total = CountTotal(userId);

                    _context.Payments.Add(payment);

                    ReduceProductStock(carts);

                   await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }

            }
            //   _context.Payments.Add(payment);
            return RedirectToAction("Create", "Order");
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
        private List<ProductsInCart> GetUserCartProducts()
        {
            var userId = GetUserId();
            List<ProductsInCart> productsInCart = _context.Carts
                                               .Where(c => c.UserId == userId
                                                        && c.OrderId == null)
                                               .Select(x => new ProductsInCart
                                               {
                                                   Id = x.ProductId,
                                                   Quantity = x.Quantity
                                               }).ToList();
            return productsInCart;
        }

        private bool CheckProductAvailability(List<ProductsInCart> productsInCart)
        {
            foreach(var product in productsInCart)
            {
                var stock = _context.Products.Find(product.Id).Available;
                if (stock < product.Quantity)
                {
                    return false;
                }
            }
            return true;
        }

        private void ReduceProductStock(List<ProductsInCart> productsToRemoveFromStock)
        {
            foreach(var product in productsToRemoveFromStock)
            {
                var stock = _context.Products.Find(product.Id).Available;
                stock -= product.Quantity;
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
