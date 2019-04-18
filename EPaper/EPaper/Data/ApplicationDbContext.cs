using System;
using System.Collections.Generic;
using System.Text;
using EPaper.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EPaper.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<CartOrder> CartOrders { get; set; }
        public DbSet<Comic> Comics { get; set; }
        public DbSet<Magazine> Magazines { get; set; }
        public DbSet<MusicCd> MusicCds { get; set; }
        public DbSet<LookUpTable> LookUpTables { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
