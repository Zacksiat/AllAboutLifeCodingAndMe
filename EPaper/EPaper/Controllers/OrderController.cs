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
        public IActionResult Index()
        {
            var userId = GetUserId();
            List<Order> orders = _context.Orders.Where(o => o.ApplicationUserId == userId).ToList();
            return View();
        }
        

        // GET : /Admin/Orders
        [Authorize(Roles ="Admin")]
        public IActionResult AdminIndex()
        {
             List<Order> Orders =_context.Orders.ToList();
            return View(Orders);
        }
        private string GetUserId()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId;
        }
    }
}