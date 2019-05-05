using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPaper.Data;
using EPaper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal.ExternalLoginModel;

namespace EPaper.Models
{

    [Authorize(Roles ="Admin")]
    public class RoleController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public RoleController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Users;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET : User/RegisterRole
        [HttpGet]
        public IActionResult RegisterRole()
        {
            UserRoles UserRoles = new UserRoles();

//            UserRoles.Users
                ViewData["Users"]=new SelectList( _context.Users,"Id","UserName");
            ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name");

            return View(UserRoles);
        }

        //// POST : User/RegisterRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterRole([Bind("Users,Roles")]UserRoles UserRoles)
        {
            var user = _context.Users.Find(UserRoles.Users.Id);
            var role = _context.Roles.Find(UserRoles.Roles.Id);
            // Remove Any previous roles before assigning new role
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles.ToArray());

            await _userManager.AddToRoleAsync(user,role.Name);
            return RedirectToAction("Index");

        }
    }
}