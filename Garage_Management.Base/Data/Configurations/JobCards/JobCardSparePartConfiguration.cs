using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.JobCards
{
    /// <summary>
    /// Cấu hình cho bảng trung gian JobCardSparePart (phụ tùng sử dụng trên phiếu sửa chữa).
    /// </summary>
    public class JobCardSparePartConfiguration : IEntityTypeConfiguration<JobCardSparePart>
    {
        public void Configure(EntityTypeBuilder<JobCardSparePart> builder)
        {
            builder.HasKey(jsp => new { jsp.JobCardId, jsp.SparePartId });
            builder.Property(jsp => jsp.Note)
                .HasMaxLength(500);
            builder.Property(jsp => jsp.UnitPrice)
                .HasPrecision(18, 2);
            builder.Property(jsp => jsp.TotalAmount)
                .HasPrecision(18, 2);

            // Quan hệ N-1: JobCardSparePart thuộc một JobCard
            builder.HasOne(jsp => jsp.JobCard)
                .WithMany(j => j.SpareParts)
                .HasForeignKey(jsp => jsp.JobCardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ N-1: Phụ tùng xuất từ Inventory
            builder.HasOne(jsp => jsp.Inventory)
                .WithMany(i => i.JobCardSpareParts)
                .HasForeignKey(jsp => jsp.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}