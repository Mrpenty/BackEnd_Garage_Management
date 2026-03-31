using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.RepairEstimaties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.RepairEstimaties
{
    /// <summary>
    /// Cấu hình cho bảng trung gian RepairEstimateSparePart (phụ tùng trong báo giá).
    /// </summary>
    public class RepairEstimateSparePartConfiguration : IEntityTypeConfiguration<RepairEstimateSparePart>
    {
        public void Configure(EntityTypeBuilder<RepairEstimateSparePart> builder)
        {
            builder.HasKey(resp => new { resp.RepairEstimateId, resp.SparePartId });
            builder.Property(resp => resp.UnitPrice)
                .HasPrecision(18, 2);
            builder.Property(resp => resp.TotalAmount)
                .HasPrecision(18, 2);
            builder.Property(resp => resp.Status)
                .HasConversion<int>()
                .HasDefaultValue(RepairEstimateApprovalStatus.WaitingApproval);

            // Quan hệ N-1: Dòng phụ tùng thuộc một RepairEstimate
            builder.HasOne(resp => resp.RepairEstimate)
                .WithMany(re => re.SpareParts)
                .HasForeignKey(resp => resp.RepairEstimateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ N-1: Phụ tùng từ Inventory
            builder.HasOne(resp => resp.Inventory)
                .WithMany(i => i.RepairEstimateSpareParts)
                .HasForeignKey(resp => resp.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
