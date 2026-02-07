using Garage_Management.Base.Entities.Inventories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Inventories
{
    /// <summary>
    /// Cấu hình cho bảng Inventory (danh mục phụ tùng / tồn kho).
    /// </summary>
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.HasKey(i => i.SparePartId);
            builder.Property(i => i.PartName)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(i => i.Unit)
                .HasMaxLength(50);
            builder.Property(i => i.ModelCompatible)
                .HasMaxLength(200);
            builder.Property(i => i.VehicleBrand)
                .HasMaxLength(100);
            builder.Property(i => i.LastPurchasePrice)
                .HasPrecision(18, 2);
            builder.Property(i => i.SellingPrice)
                .HasPrecision(18, 2);

            // Quan hệ N-1: Inventory thuộc một SparePartBrand
            builder.HasOne(i => i.SparePartBrand)
                .WithMany(sb => sb.SpareParts)
                .HasForeignKey(i => i.SparePartBrandId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}