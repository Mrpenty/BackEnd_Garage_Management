using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities;
using Garage_Management.Base.Entities.RepairEstimaties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.RepairEstimaties
{
    /// <summary>
    /// Cấu hình cho bảng trung gian RepairEstimateService (dịch vụ trong báo giá).
    /// </summary>
    public class RepairEstimateServiceConfiguration : IEntityTypeConfiguration<RepairEstimateService>
    {
        public void Configure(EntityTypeBuilder<RepairEstimateService> builder)
        {
            builder.HasKey(res => new { res.RepairEstimateId, res.ServiceId });
            builder.Property(res => res.UnitPrice)
                .HasPrecision(18, 2);
            builder.Property(res => res.TotalAmount)
                .HasPrecision(18, 2);
            builder.Property(res => res.Status)
                .HasConversion<int>()
                .HasDefaultValue(RepairEstimateApprovalStatus.WaitingApproval);

            // Quan hệ N-1: Dòng dịch vụ thuộc một RepairEstimate
            builder.HasOne(res => res.RepairEstimate)
                .WithMany(re => re.Services)
                .HasForeignKey(res => res.RepairEstimateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ N-1: Gắn với Service
            builder.HasOne(res => res.Service)
                .WithMany(s => s.RepairEstimateServices)
                .HasForeignKey(res => res.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
