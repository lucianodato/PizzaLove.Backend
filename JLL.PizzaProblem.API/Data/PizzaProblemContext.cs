using JLL.PizzaProblem.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLL.PizzaProblem.API.Data
{
    public class PizzaProblemContext : DbContext
    {
        public PizzaProblemContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Test",
                    Username = "test",
                    Password = "test",
                    PizzaLove = 1
                });

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 2,
                    FirstName = "User",
                    LastName = "User",
                    Username = "user",
                    Password = "user",
                    PizzaLove = 3
                });
        }
    }
}
