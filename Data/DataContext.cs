using Coursea.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Coursea.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PaymentDetail>(x => x.HasKey(p => new { p.PaymentId, p.CourseId }));

            builder.Entity<PaymentDetail>()
                .HasOne(o => o.Course)
                .WithMany(m => m.PaymentDetails)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(f => f.CourseId);

            builder.Entity<PaymentDetail>()
                .HasOne(o => o.Payment)
                .WithMany(m => m.PaymentDetails)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(f => f.PaymentId);

            builder.Entity<User>()
                .HasOne(o => o.Instructor)
                .WithOne(i => i.User)
                .HasForeignKey<Instructor>(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.Entity<User>()
               .HasOne(o => o.Student)
               .WithOne(i => i.User)
               .HasForeignKey<Student>(i => i.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired(false);

            builder.Entity<Report>()
                .HasOne(o => o.Student)
                .WithMany(i => i.Reports)
                .OnDelete(DeleteBehavior.NoAction);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "1"
                },
                new IdentityRole
                {
                    Name = "Instructor",
                    NormalizedName = "INSTRUCTOR",
                    ConcurrencyStamp = "2"
                },
                new IdentityRole
                {
                    Name = "Student",
                    NormalizedName = "STUDENT",
                    ConcurrencyStamp = "3"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
