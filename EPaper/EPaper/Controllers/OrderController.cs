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

            var orders = _context.Carts.Include(c => c.Order)
                                       .Where(x => x.OrderId != null &&
                                                   x.UserId == GetUserId())
                                       .ToList();
               
                return View(orders);
            
        }
        

        // GET : /Admin/Orders
        [Authorize(Roles ="Admin")]
        public IActionResult AdminIndex()
        {
            var orders = _context.Carts.Include(c => c.Order)
                                       .Where(c => c.OrderId != null)
                                       .ToList();
            return View(orders);
        }

        private string GetUserId()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId;
        }
    }
}