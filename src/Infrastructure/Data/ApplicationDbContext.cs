using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet для всех сущностей
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;
    public DbSet<Specialization> Specializations { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Игнорируем вычисляемое свойство Age
        modelBuilder.Entity<Patient>()
            .Ignore(p => p.Age);

        // Конфигурация Patient
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            // Уникальный индекс для номера паспорта
            entity.HasIndex(p => p.PassportNumber)
                .IsUnique();
                
            // Настройка строковых полей
            entity.Property(p => p.PassportNumber)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(p => p.FullName)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(p => p.Address)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(p => p.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);
                
            // Настройка перечислений
            entity.Property(p => p.Gender)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);
                
            entity.Property(p => p.BloodGroup)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(5);
                
            entity.Property(p => p.RhFactor)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(5);
                
            // Дата рождения
            entity.Property(p => p.BirthDate)
                .IsRequired()
                .HasConversion<DateOnlyConverter, DateOnlyComparer>();
        });

        // Конфигурация Specialization
        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(s => s.Id);
            
            // Уникальный индекс для названия специализации
            entity.HasIndex(s => s.Name)
                .IsUnique();
                
            // Настройка свойств
            entity.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(s => s.Description)
                .HasMaxLength(500);
        });

        // Конфигурация Doctor
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(d => d.Id);
            
            // Уникальный индекс для номера паспорта
            entity.HasIndex(d => d.PassportNumber)
                .IsUnique();

            // Связь с Specialization
            entity.HasOne(d => d.Specialization)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Настройка свойств
            entity.Property(d => d.PassportNumber)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(d => d.FullName)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(d => d.BirthYear)
                .IsRequired();
                
            entity.Property(d => d.ExperienceYears)
                .IsRequired();
        });

        // Конфигурация Appointment
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(a => a.Id);
            
            // Индекс для быстрого поиска по дате
            entity.HasIndex(a => a.AppointmentDateTime);
            
            // Индекс для комбинированного поиска по врачу и дате
            entity.HasIndex(a => new { a.DoctorId, a.AppointmentDateTime });

            // Связь с Patient
            entity.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь с Doctor
            entity.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Настройка свойств
            entity.Property(a => a.AppointmentDateTime)
                .IsRequired();
                
            entity.Property(a => a.RoomNumber)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(a => a.IsFollowUp)
                .IsRequired();
        });
    }

    // Конвертер для DateOnly (если используете EF Core 7+)
    public class DateOnlyConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() 
            : base(dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
                   dateTime => DateOnly.FromDateTime(dateTime))
        {
        }
    }

    public class DateOnlyComparer : Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<DateOnly>
    {
        public DateOnlyComparer() 
            : base((d1, d2) => d1 == d2 && d1.DayNumber == d2.DayNumber,
                   d => d.GetHashCode())
        {
        }
    }
}