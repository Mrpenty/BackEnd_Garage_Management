using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.JobCards
{
    /// <summary>
    /// Cấu hình cho bảng JobCard (phiếu sửa chữa).
    /// </summary>
    public class JobCardConfiguration : IEntityTypeConfiguration<JobCard>
    {
        public void Configure(EntityTypeBuilder<JobCard> builder)
        {
            builder.HasKey(j => j.JobCardId);
            builder.Property(j => j.Note)
                .HasMaxLength(1000);
            builder.Property(j => j.QueueOrder)
                .HasPrecision(18, 6);

            // Quan hệ N-1: JobCard có thể phát sinh từ một Appointment
            builder.HasOne(j => j.Appointment)
                .WithMany(a => a.GeneratedJobCards)
                .HasForeignKey(j => j.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ N-1: JobCard thuộc một Customer
            builder.HasOne(j => j.Customer)
                .WithMany(c => c.JobCards)
                .HasForeignKey(j => j.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-1: JobCard gắn với một Vehicle
            builder.HasOne(j => j.Vehicle)
                .WithMany(v => v.JobCards)
                .HasForeignKey(j => j.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-1: Supervisor giám sát phiếu
            //builder.HasOne(j => j.Supervisor)
            //    .WithMany()
            //    .HasForeignKey(j => j.SupervisorId)
            //    .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-N: JobCard có nhiều JobCardService
            builder.HasMany(j => j.Services)
                   .WithOne(js => js.JobCard)
                   .HasForeignKey(js => js.JobCardId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(j => j.CreatedByEmployee)
       .WithMany()
       .HasForeignKey(j => j.CreatedBy)
       .OnDelete(DeleteBehavior.Restrict);



            builder.HasOne(j => j.Supervisor)
                   .WithMany(e => e.SupervisedJobCards)
                   .HasForeignKey(j => j.SupervisorId)
                   .OnDelete(DeleteBehavior.Restrict);




        }
    }
}
