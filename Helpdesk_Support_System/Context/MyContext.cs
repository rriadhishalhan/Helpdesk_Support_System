using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Context
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
           
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketHistory> TicketHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasOne(e => e.Account).WithOne(a => a.Employee).HasForeignKey<Account>(a => a.Email);

            modelBuilder.Entity<Customer>().HasOne(c => c.Account).WithOne(p => p.Customer).HasForeignKey<Account>(p => p.Email);

            //modelBuilder.Entity<TicketHistory>().HasOne(th => th.Employee).WithMany(e => e.TicketHistories).HasForeignKey(th => th.Employee_email);
            //modelBuilder.Entity<TicketHistory>().HasOne(th => th.Employee).WithMany(e => e.TicketHistories).HasForeignKey(th => th.Employee_position);

            //modelBuilder.Entity<Employee>().HasMany(e => e.TicketHistories).WithOne(th => th.Employee).HasForeignKey(th => th.Employee_position);
            //modelBuilder.Entity<Employee>().HasMany(e => e.TicketHistories).WithOne(th => th.Employee).HasForeignKey(th => th.Employee_email);
        }

       


    }
}
