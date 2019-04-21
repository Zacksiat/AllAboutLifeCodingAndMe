using System;
using System.Collections.Generic;
using System.Text;
using ESHOPFORCODINGSCHOOL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ESHOPFORCODINGSCHOOL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Basket> Baskets { get; set; }

        public DbSet<BasketProduct> BasketProducts { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Cd> Cds { get; set; }

        public DbSet<Magazine> Magazines { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<Product> Products { get; set; }



        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // seed Admin Role
            builder.Entity<IdentityRole>(entity =>
            {
                entity.HasData(new IdentityRole { Name = "Admin", NormalizedName = "Admin".ToUpper() });
                entity.HasData(new IdentityRole { Name = "User", NormalizedName = "User".ToUpper() });
            });
           

            builder.Entity<OrderProduct>().HasKey(op => new { op.ProductId, op.OrderId });

            builder.Entity<BasketProduct>().HasKey(bp => new { bp.BasketId, bp.ProductId });

            builder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Type).IsRequired();
                entity.Property(p => p.Name).IsRequired();
            });
            builder.Entity<Order>(entity =>
            {
                entity.Property(o => o.ApplicationUserId).IsRequired();
            });

            builder.Entity<Magazine>(entity =>
            {
                entity.Property(m => m.Publisher).IsRequired();
                entity.Ignore(m => m.TypeName)
              .Ignore(m => m.Name)
              .Ignore(m => m.Price);
            });
            builder.Entity<Cd>(entity =>
            {
                entity.Property(c => c.Artist).IsRequired();
                entity.Property(c => c.Label).IsRequired();
                entity.Ignore(c => c.TypeName)
              .Ignore(c => c.Name)
              .Ignore(c => c.Price);
            });

            builder.Entity<Book>(entity =>
            {
                entity.Property(b => b.Author).IsRequired();
                entity.Property(b => b.Publisher).IsRequired();
                entity.Ignore(b => b.TypeName)
                .Ignore(b => b.Name)
                .Ignore(b => b.Price);
            });
            builder.Entity<Basket>(entity =>
            {
                entity.Property(b => b.ApplicationUserId).IsRequired();
            });
        }
    }
   /* public static class ApplicationDbInitializer
    {
        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("asdf@gmail.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "asdf@gmail.com",
                    Email = "asdf@gmail.com"
                };

                IdentityResult result = userManager.CreateAsync(user,"Asdf1234!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user,"Admin").Wait();
                }
            }
        }
    }*/
}

