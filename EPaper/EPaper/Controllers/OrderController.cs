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
            return View();
        }
        

        // GET : /Admin/Orders
        [Authorize(Roles ="Admin")]
        public IActionResult AdminIndex()
        {
             List<Order> Orders =_context.Orders.ToList();
            return View(Orders);
        }

 
  


    }
}