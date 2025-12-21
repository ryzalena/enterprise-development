using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.PassportNumber)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(d => d.FullName)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(d => d.BirthYear)
            .IsRequired();
            
        builder.Property(d => d.ExperienceYears)
            .IsRequired();
        
        builder.HasOne(d => d.Specialization)
            .WithMany()
            .HasForeignKey(d => d.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(d => d.PassportNumber)
            .IsUnique();
    }
}