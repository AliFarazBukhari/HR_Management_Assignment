using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Entities;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace Repository
{
   

    public class HrDbContext : DbContext
    {
        public HrDbContext(DbContextOptions<HrDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();

        public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();

        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureEmployee(modelBuilder);
            ConfigureLeaveType(modelBuilder);
            ConfigureLeaveBalance(modelBuilder);
            ConfigureLeaveRequest(modelBuilder);
        }

        private static void ConfigureEmployee(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");

                entity.HasKey(x => x.EmployeeNumber);

                entity.Property(x => x.EmployeeNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.FullName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasIndex(x => x.EmployeeNumber)
                    .IsUnique();

                entity.HasIndex(x => x.Email)
                    .IsUnique();
            });
        }

        private static void ConfigureLeaveType(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeaveType>(entity =>
            {
                entity.ToTable("LeaveTypes");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.MonthlyAccrualRate)
                    .HasPrecision(18, 2);

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });
        }

        private static void ConfigureLeaveBalance(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeaveBalance>(entity =>
            {
                entity.ToTable("LeaveBalances");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Balance)
                    .HasPrecision(18, 2);

                ////entity.HasOne(x => x.Employee)
                ////    .WithMany(x => x.LeaveBalances)
                ////    .HasForeignKey(x => x.EmployeeId)
                ////    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.LeaveType)
                    .WithMany(x => x.LeaveBalances)
                    .HasForeignKey(x => x.LeaveTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                // One balance per employee per leave type
                entity.HasIndex(x => new
                {
                    x.EmployeeId,
                    x.LeaveTypeId
                })
                .IsUnique();
            });
        }

        private static void ConfigureLeaveRequest(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.ToTable("LeaveRequests");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Reason)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(x => x.RejectionComment)
                    .HasMaxLength(1000);

                entity.Property(x => x.DaysRequested)
                    .HasPrecision(18, 2);

                entity.Property(x => x.Status)
                    .HasConversion<int>();

                //entity.HasOne(x => x.Employee)
                //    .WithMany(x => x.LeaveRequests)
                //    .HasForeignKey(x => x.EmployeeId)
                //    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.LeaveType)
                    .WithMany(x => x.LeaveRequests)
                    .HasForeignKey(x => x.LeaveTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.EmployeeId);

                entity.HasIndex(x => x.Status);

                entity.HasIndex(x => x.StartDate);

                entity.HasIndex(x => x.EndDate);

                entity.HasIndex(x => new
                {
                    x.EmployeeId,
                    x.StartDate,
                    x.EndDate
                });
            });
        }
    }
}
