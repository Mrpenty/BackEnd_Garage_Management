using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Accounts
{
    /// <summary>
    /// Cấu hình cho bảng Appointment (lịch hẹn dịch vụ).
    /// </summary>
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(a => a.AppointmentId);

            // Quan hệ N-1: Lịch hẹn thuộc một Customer
            builder.HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-1: Lịch hẹn có thể gắn với một Vehicle
            builder.HasOne(a => a.Vehicle)
                .WithMany(v => v.Appointments)
                .HasForeignKey(a => a.VehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ N-1: Nhân viên tạo lịch hẹn (audit)
            builder.HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ N-1: Nhân viên cập nhật (audit)
            builder.HasOne(a => a.UpdatedByUser)
                .WithMany()
                .HasForeignKey(a => a.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(a => a.Description)
                .HasMaxLength(500);

        }
    }
}