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
    public class OrderController : Controller
    {

        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET : /Orders
        [Authorize]
        public IActionResult Index()
        {
            IEnumerable<OrdersViewModel> ordersList;

            var idtemp = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                ordersList = (from f in _context.Carts
                              join d in _context.Orders
                              on f.UserId equals d.UserId
                              join p in _context.Products
                              on f.ProductId equals p.ProductId
                              where (f.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && d.PaymentId == f.OrderId)
                              select new OrdersViewModel { PaymentId = d.PaymentId, Name = p.Name, Category = p.Type, Quantity = f.Quantity, Price = p.Price }).ToList();

            return View(ordersList);

        }


        // GET : /Admin/Orders
        [Authorize(Roles = "Admin")]
        public IActionResult AdminIndex()
        {
            var carts = _context.Carts
                .Include(p=>p.Product)
                .Include(u => u.ApplicationUser)
                .Include(c => c.Order)
                .ThenInclude(p => p.Payment)
                .Where(c => c.OrderId != null)
                .ToList();
            return View(carts);
        }

        private string GetUserId()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId;
        }
    }
}