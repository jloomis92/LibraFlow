using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraFlow.Models;

namespace LibraFlow.Data
{
    public class LibraFlowContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use an absolute path to ensure the database file can be created/accessed
            var dbPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LibraFlow", "library.db");

            // Ensure the directory exists
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath));

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*
            // Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "The Pragmatic Programmer", Author = "Andrew Hunt", ISBN = "0-8479-9529-1", IsCheckedOut= true },
                new Book { Id = 2, Title = "Clean Code", Author = "Robert C. Martin", ISBN = "0-1500-2863-6", IsCheckedOut = true }
            );

            // Seed Members
            modelBuilder.Entity<Member>().HasData(
                new Member { Id = 1, Name = "Alice Smith", Email = "alicesmith@example.com" },
                new Member { Id = 2, Name = "Bob Johnson", Email = "bobjohnson@example.com" }
            );

            // Seed Loans
            modelBuilder.Entity<Loan>().HasData(
                new Loan
                {
                    Id = 1,
                    BookId = 1,
                    MemberId = 1,
                    CheckedOutDate = new DateTime(2024, 7, 1),
                    ReturnDate = null
                },
                new Loan
                {
                    Id = 2,
                    BookId = 2,
                    MemberId = 2,
                    CheckedOutDate = new DateTime(2024, 7, 5),
                    ReturnDate = new DateTime(2024, 7, 20)
                }
            ); */
        }
    }
}
