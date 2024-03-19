using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        //public DbSet<Books> Books { get; set; }
        public DbSet<Carts> Carts { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        //public DbSet<Comments> Comments { get; set; }

        public DbSet<Aircraft> Aircraft { get; set; }
        public DbSet<AircrafImages> AircrafImages { get; set; }
        public DbSet<AvailableCustomozation> AvailableCustomozations { get; set; }
        public DbSet<Specifcation> Specifcations { get; set; }
        public DbSet<CartCustomozationDetails> CartCustomozationDetails { get; set; }

        public DbSet<ShippingDetail> ShippingDetail { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<OrderCustomozationDetails> OrderCustomozationDetails { get; set; }

        public DbSet<Team> Team { get; set; }
        public DbSet<Job> Job { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<ShippingSet> ShippingSet { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
