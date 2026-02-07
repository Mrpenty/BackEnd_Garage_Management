using Garage_Management.Base.Entities;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.JobCards
{
    /// <summary>
    /// Cấu hình cho bảng trung gian JobCardService (dịch vụ thực hiện trên phiếu sửa chữa).
    /// </summary>
    public class JobCardServiceConfiguration : IEntityTypeConfiguration<JobCardService>
    {
        public void Configure(EntityTypeBuilder<JobCardService> builder)
        {
            builder.HasKey(js => new { js.JobCardId, js.ServiceId });
            builder.Property(js => js.Description)
                .HasMaxLength(500);
            builder.Property(js => js.WarrantyCode)
                .HasMaxLength(50);
            builder.Property(js => js.ActualPrice)
                .HasPrecision(18, 2);
            builder.Property(js => js.TotalAmount)
                .HasPrecision(18, 2);

            // Quan hệ N-1: JobCardService thuộc một JobCard
            builder.HasOne(js => js.JobCard)
                .WithMany(j => j.Services)
                .HasForeignKey(js => js.JobCardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ N-1: JobCardService gắn với một Service
            builder.HasOne(js => js.Service)
                .WithMany(s => s.JobCardServices)
                .HasForeignKey(js => js.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-1: Người thực hiện dịch vụ
            builder.HasOne(js => js.PerformedByUser)
                .WithMany()
                .HasForeignKey(js => js.PerformedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ N-1: Người tạo dòng (audit)
            builder.HasOne(js => js.CreatedByUser)
                .WithMany()
                .HasForeignKey(js => js.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-1: Người cập nhật (audit)
            builder.HasOne(js => js.UpdatedByUser)
                .WithMany()
                .HasForeignKey(js => js.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}