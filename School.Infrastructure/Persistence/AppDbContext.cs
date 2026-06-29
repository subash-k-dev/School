using Microsoft.EntityFrameworkCore;
using School.Domain.Entities;

namespace School.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<FeePayment> FeePayments => Set<FeePayment>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .Property(x => x.Rollnumber)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Student>()
            .HasIndex(x => x.Rollnumber)
            .IsUnique();

        modelBuilder.Entity<AttendanceRecord>()
            .Property(x => x.Date)
            .HasColumnType("timestamp without time zone");
    }
}