using BandRecruiting.Core.Entities;
using BandRecruiting.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BandRecruiting.Infrastructure.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Optional: rename “AspNet*” tables -> “Auth*” for clarity
        //    foreach (var e in builder.Model.GetEntityTypes())
        //    {
        //        if (e.GetTableName()!.StartsWith("AspNet"))
        //            e.SetTableName(e.GetTableName()!.Replace("AspNet", "Auth"));
        //    }

        // --- Student 1-to-1 with ApplicationUser ----------------------------
        builder.Entity<Student>(student =>
        {
            student.ToTable("Students");

            student.HasKey(s => s.StudentId);

            student.Property(s => s.FirstName).IsRequired().HasMaxLength(100);
            student.Property(s => s.LastName).IsRequired().HasMaxLength(100);
            student.Property(s => s.Instrument).HasMaxLength(100);
            student.Property(s => s.HighSchool).HasMaxLength(150);
            student.Property(s => s.ProfilePictureUrl).HasMaxLength(250);
            student.Property(s => s.CreatedAt).IsRequired();

            // 1-to-1: ApplicationUser ↔ Student
            student.HasOne(s => s.User)
                   .WithOne(u => u.Student)
                   .HasForeignKey<Student>(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

    }

}
