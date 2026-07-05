using System;
using Microsoft.EntityFrameworkCore;
using API.Entities;

namespace API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<Photo> Photos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AppUser>().HasKey(u => u.Id);
        modelBuilder.Entity<AppUser>().Property(u => u.UserName).IsRequired();
        modelBuilder.Entity<AppUser>().Property(u => u.Email).IsRequired();

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.Photos)
            .WithOne(p => p.AppUser)
            .HasForeignKey(p => p.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
