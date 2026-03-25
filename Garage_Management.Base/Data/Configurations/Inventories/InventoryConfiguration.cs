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

            builder.Property(i => i.PartCode)
                .HasMaxLength(50);

            builder.Property(i => i.PartName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.Unit)
                .HasMaxLength(50);

            builder.Property(i => i.LastPurchasePrice)
                .HasPrecision(18, 2);

            builder.Property(i => i.SellingPrice)
                .HasPrecision(18, 2);

            // PartCode unique khi có giá trị
            builder.HasIndex(i => i.PartCode)
                .IsUnique()
                .HasFilter("[PartCode] IS NOT NULL");

            // Quan hệ N-1: Inventory thuộc một SparePartBrand
            builder.HasOne(i => i.SparePartBrand)
                .WithMany(sb => sb.SpareParts)
                .HasForeignKey(i => i.SparePartBrandId)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ N-1: Inventory thuộc một SparePartCategory
            builder.HasOne(i => i.SparePartCategory)
                .WithMany(c => c.SparePart)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ 1-N: Inventory có nhiều StockTransaction
            builder.HasMany(i => i.StockTransactions)
                .WithOne(st => st.Inventory)
                .HasForeignKey(st => st.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-N: Inventory tương thích với nhiều VehicleModel
            builder.HasMany(i => i.CompatibleModels)
                .WithOne(ivm => ivm.Inventory)
                .HasForeignKey(ivm => ivm.SparePartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}