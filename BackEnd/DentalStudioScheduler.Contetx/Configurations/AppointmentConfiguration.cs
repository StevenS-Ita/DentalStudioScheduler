using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DentalStudioScheduler.Context.Base;
using DentalStudioScheduler.Models;

namespace DentalStudioScheduler.Context
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> entity)
        {
            entity.ToTable("Appointments");

            entity.HasKey(e => e.AppointmentId).HasName("PK_Appointment");

            entity.Property(e => e.AppointmentId)
                .HasColumnOrder(0)
                .HasDefaultValueSql("(newsequentialid())");

            entity.Property(e => e.PatientFirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.PatientTelephoneNumber)
                .IsRequired()
                .HasMaxLength(13);

            entity.Property(e => e.Notes)
                .HasMaxLength(250);

            // Apply Base Configuration
            entity.ConfigureBase();
        }
    }
}
