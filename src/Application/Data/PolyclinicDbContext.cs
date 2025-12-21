using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Application.Data;

public class PolyclinicDbContext : DbContext
{
    public PolyclinicDbContext(DbContextOptions<PolyclinicDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Specialization> Specializations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Конфигурация Patient
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.PassportNumber)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(p => p.FullName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.Address)
                .HasMaxLength(200);
            entity.Property(p => p.PhoneNumber)
                .HasMaxLength(20);
            
            entity.HasIndex(p => p.PassportNumber).IsUnique();
        });

        // Конфигурация Doctor
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.PassportNumber)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(d => d.FullName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.HasIndex(d => d.PassportNumber).IsUnique();
            
            // Связь с Specialization
            entity.HasOne(d => d.Specialization)
                .WithMany()
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Конфигурация Appointment
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.RoomNumber)
                .IsRequired()
                .HasMaxLength(10);
            
            // Связь с Patient
            entity.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Связь с Doctor
            entity.HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Конфигурация Specialization
        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(s => s.Description)
                .HasMaxLength(500);
            
            entity.HasIndex(s => s.Name).IsUnique();
        });
    }
}