using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;

namespace EMS.Infrastructure.Contexts;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var a = new Event() { Image = null };
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);

            entity.Property(e => e.Location).IsRequired();

            entity.Property(e => e.Description).IsRequired().HasMaxLength(255);

            entity.Property(e => e.Image).IsRequired();

            entity.Property(e => e.Category).HasConversion<string>();
        });

        modelBuilder.Entity<EventRegistration>()
            .HasKey(er => new { er.EventId, er.UserId });

        modelBuilder.Entity<EventRegistration>()
            .HasOne(er => er.Event)
            .WithMany(e => e.EventRegistrations)
            .HasForeignKey(er => er.EventId);

        modelBuilder.Entity<EventRegistration>()
            .HasOne(er => er.User)
            .WithMany(u => u.EventRegistrations)
            .HasForeignKey(er => er.UserId);
    }
}