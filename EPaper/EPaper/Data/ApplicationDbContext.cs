using System;
using System.Collections.Generic;
using System.Text;
using EPaper.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EPaper.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Book> Books { get; set; }

        public DbSet<Cd> Cds { get; set; }

        public DbSet<Magazine> Magazines { get; set; }

        public DbSet<Comic> Comics { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Cart> Carts { get; set; }

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

            builder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.ApplicationUser)
                .WithMany(p => p.Payments)
                .HasForeignKey(p => p.UserId)
                .IsRequired();
                entity.Property(p => p.PaymentMethod).IsRequired();
            });


            //builder.Entity<Cart>().HasOne(x => x.ApplicationUser)
            //    .WithMany(c => c.Carts)
            //    .HasForeignKey(x => x.UserId)
            //    .IsRequired();
            //builder.Entity<Cart>().HasOne(x => x.Product);
            ////    builder.Entity<Cart>().HasOne(o => o.Order);
            //builder.Entity<Cart>().Property(x => x.Quantity).IsRequired();
            //builder.Entity<Cart>().HasKey(o => o.Id);
           

            builder.Entity<Cart>(entity =>
            {
                entity.HasOne(x => x.ApplicationUser)
                      .WithMany(c => c.Carts)
                      .HasForeignKey(p => p.UserId)
                      .IsRequired();

                entity.HasOne(x => x.Product);

                entity.HasOne(o => o.Order)
                      .WithMany(c => c.Carts)
                      .HasForeignKey(o => o.OrderId);

                entity.Property(x => x.Quantity).IsRequired();

                entity.HasKey(c => c.Id);


                      
            });
            

            builder.Entity<Order>(entity =>
            {

                entity.HasOne(u => u.ApplicationUser)
                .WithMany(o => o.Orders)
                .HasForeignKey(u => u.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


                entity.Property(u => u.Address).IsRequired();

            });



            builder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Type).IsRequired();
                entity.Property(p => p.Name).IsRequired();
            });


            builder.Entity<Magazine>(entity =>
            {
                entity.Property(m => m.Publisher).IsRequired();

            });
            builder.Entity<Cd>(entity =>
            {
                entity.Property(c => c.Artist).IsRequired();
                entity.Property(c => c.Label).IsRequired();

            });


            builder.Entity<Book>(entity =>
            {

                entity.Property(b => b.Author).IsRequired();
                entity.Property(b => b.Publisher).IsRequired();
            });

            builder.Entity<Comic>(entity =>
            {
                entity.Property(b => b.Author).IsRequired();
                entity.Property(b => b.Publisher).IsRequired();
            });

        }
    }
    public static class ApplicationDbInitializer
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

                IdentityResult result = userManager.CreateAsync(user, "Asdf1234!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
            else
            {
                ApplicationUser user = userManager.FindByEmailAsync("asdf@gmail.com").Result;
                userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}


