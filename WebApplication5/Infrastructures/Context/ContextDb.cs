using Microsoft.EntityFrameworkCore;
using WebApplication5.Models;

namespace WebApplication5.Context;

public class ContextDb : DbContext
{
    public ContextDb(DbContextOptions<ContextDb> options) : base(options)
    {
        
    }
    
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>().HasKey(u => u.Id);
        modelBuilder.Entity<Room>().HasAlternateKey(u => u.RoomName);
        
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().HasAlternateKey(u => u.UserName);

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Name = "AdminUser",
            Password = "admin",
            Role = "Admin",
            UserName = "OnlyAdmin"
        });
    }
}