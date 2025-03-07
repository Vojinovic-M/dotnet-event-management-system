using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;
using EMS.Infrastructure.Identity;

namespace EMS.Infrastructure.Contexts;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var a = new Event() { ImageUrl = null};
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId);

            entity.Property(e => e.Name)
                .IsRequired().HasMaxLength(255);

            entity.Property(e => e.Location)
                .IsRequired();

            entity.Property(e => e.Description)
                .IsRequired().HasMaxLength(255);

            entity.Property(e => e.ImageUrl)
                .IsRequired();

            entity.Property(e => e.Category)
                .HasConversion<string>();



        });
    }
}
