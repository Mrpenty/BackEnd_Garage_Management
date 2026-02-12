using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.RepairEstimaties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.RepairEstimaties
{
    /// <summary>
    /// Cấu hình cho bảng RepairEstimate (báo giá sửa chữa).
    /// </summary>
    public class RepairEstimateConfiguration : IEntityTypeConfiguration<RepairEstimate>
    {
        public void Configure(EntityTypeBuilder<RepairEstimate> builder)
        {
            builder.HasKey(re => re.RepairEstimateId);
            builder.Property(re => re.Note)
                .HasMaxLength(1000);
            builder.Property(re => re.ServiceTotal)
                .HasPrecision(18, 2);
            builder.Property(re => re.SparePartTotal)
                .HasPrecision(18, 2);
            builder.Property(re => re.GrandTotal)
                .HasPrecision(18, 2);

            // Quan hệ N-1: RepairEstimate thuộc một JobCard
            builder.HasOne(re => re.JobCard)
                .WithMany(j => j.Estimates)
                .HasForeignKey(re => re.JobCardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}