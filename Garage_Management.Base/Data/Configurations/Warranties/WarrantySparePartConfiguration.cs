using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.Warranties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Warranties
{
    /// <summary>
    /// Cấu hình cho bảng WarrantySparePart (bảo hành phụ tùng).
    /// </summary>
    public class WarrantySparePartConfiguration : IEntityTypeConfiguration<WarrantySparePart>
    {
        public void Configure(EntityTypeBuilder<WarrantySparePart> builder)
        {
            builder.HasKey(wsp => wsp.WarrantySparePartId);
            builder.Property(wsp => wsp.Description)
                .HasMaxLength(500);

            // Quan hệ N-1: Bảo hành gắn với phụ tùng (Inventory)
            builder.HasOne(wsp => wsp.Inventory)
                .WithMany(i => i.WarrantySpareParts)
                .HasForeignKey(wsp => wsp.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-1: Áp dụng SparePartWarrantyPolicy
            builder.HasOne(wsp => wsp.SparePartWarrantyPolicy)
                .WithMany(p => p.WarrantySpareParts)
                .HasForeignKey(wsp => wsp.SparePartWarrantyPolicyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
