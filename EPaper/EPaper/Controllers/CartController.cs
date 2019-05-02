﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPaper.Data;
using EPaper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EPaper.Helpers;

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

                List<Cart> carts = _context.Carts
                    .Include(p => p.Product)
                    .Where(c => c.UserId == userId && c.OrderId == null)
                    .ToList();

                carts = FromSessionCartToUserCart(carts);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", null);
            //    HttpContext.Session.Clear();

                if (carts != null)
                {
                    _context.SaveChanges();
                    return View(carts);
                }
                else
                {
                    List<Cart> EmptyCart = new List<Cart>();
                    return View(EmptyCart);
                }

            }

            else
            {
                if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
                {
                    List<Item> cart = new List<Item>();
                    ViewBag.cart = cart;
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                }
                else
                {
                    List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                    ViewBag.cart = cart;
                    if (cart != null)
                    {
                        ViewBag.total = cart.Sum(item => item.Product.Price * item.Quantity);
                    }
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                }
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AddtoCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddtoCart(int id)
        {
            var product = _context.Products.Find(id);
            int quantity = 1;
            if (product != null && CheckIfProductAvailable(id, quantity))
            {
                
                if (User.Identity.IsAuthenticated)
                {

                    string userId = GetUserId();


                    if (ProductIsInCart(product.ProductId) != null)
                    {
                        Cart cart = ProductIsInCart(product.ProductId);
                        ++cart.Quantity;
                    }
                    else
                    {
                        Cart cart = new Cart
                        {
                            UserId = userId,
                            ProductId = product.ProductId,
                            Quantity = 1
                        };

                        _context.Carts.Add(cart);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
                    {
                        List<Item> cart = new List<Item>();
                        cart.Add(new Item { Product = _context.Products.Find(product.ProductId), Quantity = 1 });
                        SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                    }
                    else
                    {
                        List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                        int index = isExist(product.ProductId);
                        if (index != -1)
                        {
                            cart[index].Quantity++;
                        }
                        else
                        {
                            cart.Add(new Item { Product = _context.Products.Find(product.ProductId), Quantity = 1 });
                        }
                        SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                    }
                    return RedirectToAction("Index", "Cart");
                }
                return RedirectToAction("Index", "Cart");
            }
            return BadRequest();
        }

        // GET: Basket / Delete / 5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Product product = _context.Products.Find(id);
            if (User.Identity.IsAuthenticated)
            {
                string userId = GetUserId();
                var cart = ProductIsInCart(id);

                if (cart == null)
                {
                    return BadRequest();
                }

                return View(product);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                return View(product);
            }

        }

        // POST: Basket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (User.Identity.IsAuthenticated)
            {
                string userId = GetUserId();
                var cart = ProductIsInCart(id);

                if (cart == null)
                {
                    return BadRequest();
                }
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart.RemoveAt(index);
                }
                else
                {
                    return BadRequest();
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                return RedirectToAction("Index");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity([Bind("Quantity")]Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return BadRequest();
        }

        // GET : /CheckOut
        [Authorize]
        public IActionResult CheckOut()
        {
            var userId = GetUserId();
            List<Cart> carts = _context.Carts
                   .Include(p => p.Product)
                   .Where(c => c.UserId == userId && c.OrderId == null)
                   .ToList();

            carts = FromSessionCartToUserCart(carts);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", null);
            if (carts != null)
            {
                foreach(var item in carts)
                {
                    _context.Update(item);
                }
               
                _context.SaveChanges();
                double total = CountTotal(carts);
               
                return View("~/Views/Payment/Create.cshtml");
            }
            return BadRequest();

        }

        private string GetUserId()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId;
        }

        private Cart ProductIsInCart(int? id)
        {
            string userId = GetUserId();
            var cart = _context.Carts
              .FirstOrDefault(i => i.ProductId == id
                                && i.UserId == userId
                                && i.OrderId == null);
            return cart;
        }
        private int isExist(int? id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.ProductId.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

        private List<Cart> FromSessionCartToUserCart(List<Cart> carts)
        {
            List<Item> sessionCart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (sessionCart != null)
            {

                foreach (var item in sessionCart)
                {
                    Cart cart = ProductIsInCart(item.Product.ProductId);
                    if (cart == null)
                    {
                        carts.Add(new Cart
                        {
                            UserId = GetUserId(),
                            Product = item.Product,
                            Quantity = item.Quantity
                        });
                    }
                    else
                    {
                        cart.Quantity += item.Quantity;
                    }
                }
            }
            return carts ;
        }

        private double CountTotal(List<Cart> carts)
        {
            double total = 0;
            foreach(var item in carts)
            {
                total += (item.Product.Price * item.Quantity);
            }
            return total;
        }
       private bool CheckIfProductAvailable(int id,int quantity)
        {
            Product product  = _context.Products.Find(id); 
            if(product.Available >= quantity)
            {
                product.Available -= quantity;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
