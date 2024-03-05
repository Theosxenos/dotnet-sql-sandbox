using Microsoft.EntityFrameworkCore;
using SqlPractice.Models;

namespace SqlPractice.Data;

public class PracticeSqlServerContext: PracticeSqliteContext
{
    public DbSet<Good> Goods { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=sqlpractice;User Id=sa;Password=Brood1234;Encrypt=false");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Good>().HasNoKey();
    }
}