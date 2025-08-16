using Microsoft.EntityFrameworkCore;
using SIMS.Models;

namespace SIMS.Data
{
    public class SIMSContext : DbContext
    {
        public SIMSContext(DbContextOptions<SIMSContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table-per-hierarchy mapping (TPH) với Discriminator
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Admin>("Admin")
                .HasValue<Student>("Student")
                .HasValue<Faculty>("Faculty");

            // Unique Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Relationships
            modelBuilder.Entity<Faculty>()
                .HasMany(f => f.Courses)
                .WithOne(c => c.Faculty)
                .HasForeignKey(c => c.FacultyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Assignments)
                .WithOne(a => a.Course)
                .HasForeignKey(a => a.CourseId);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Enrollments)
                .WithOne(e => e.Student)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);




            modelBuilder.Entity<Enrollment>()
       .Property(e => e.Status)
       .HasConversion<string>()   // Lưu enum dưới dạng string
       .HasMaxLength(20)
       .HasDefaultValue(EnrollmentStatus.Pending);









            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssignmentId);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany(st => st.Submissions)
                .HasForeignKey(s => s.StudentId);
            modelBuilder.Entity<Assignment>()
             .HasOne(a => a.Course)
             .WithMany(c => c.Assignments)
                 .HasForeignKey(a => a.CourseId)
                 .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
