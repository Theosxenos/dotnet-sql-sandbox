using Microsoft.EntityFrameworkCore;
using SqlPractice.Models;

namespace SqlPractice.Data;

public class PracticeSqliteContext: DbContext
{
    public DbSet<Sale> Sales { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=practice.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sale>().HasData([
            new()
            {
                Id =1,
                Year = 2023,
                Month = "Jan",
                Product = "Keyboard",
                Total = 100,
            },   new Sale
            {
                Id =2,
                Year = 2023,
                Month = "Jan",
                Product = "Mouse",
                Total = 150,
            },
            new Sale
            {
                Id =3,
                Year = 2023,
                Month = "Feb",
                Product = "Keyboard",
                Total = 120,
            },
            new Sale
            {
                Id =4,
                Year = 2023,
                Month = "Feb",
                Product = "Mouse",
                Total = 110,
            }
        ]);
    }
}