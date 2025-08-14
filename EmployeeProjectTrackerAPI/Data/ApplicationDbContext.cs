using Microsoft.EntityFrameworkCore;
using EmployeeProjectTrackerAPI.Models;

namespace EmployeeProjectTrackerAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional model configuration can be added here if needed
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.ProjectId);
                entity.HasIndex(e => e.ProjectCode).IsUnique();
                entity.Property(e => e.ProjectCode).IsRequired().HasMaxLength(10);
                entity.Property(e => e.ProjectName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.Budget).HasColumnType("decimal(18,2)");
            });

            // Configure Employee entity
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);
                entity.HasIndex(e => e.EmployeeCode).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(8);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Designation).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            });

            // Configure relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Project)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data
            modelBuilder.Entity<Project>().HasData(
                new Project
                {
                    ProjectId = 1,
                    ProjectCode = "P001",
                    ProjectName = "E-Commerce Platform",
                    StartDate = new DateTime(2024, 1, 15),
                    EndDate = new DateTime(2024, 12, 31),
                    Budget = 250000.00m
                },
                new Project
                {
                    ProjectId = 2,
                    ProjectCode = "P002",
                    ProjectName = "Mobile Banking App",
                    StartDate = new DateTime(2025, 3, 1),
                    Budget = 180000.00m
                }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeId = 1,
                    EmployeeCode = "EMP001",
                    FullName = "Harish k",
                    Email = "harish@kanini.com",
                    Designation = "Developer",
                    Salary = 75000.00m,
                    ProjectId = 1
                },
                new Employee
                {
                    EmployeeId = 2,
                    EmployeeCode = "EMP002",
                    FullName = "Kumar",
                    Email = "kumar@kanini.com",
                    Designation = "Manager",
                    Salary = 85000.00m,
                    ProjectId = 1
                }
            );
        }
    }
    
}
