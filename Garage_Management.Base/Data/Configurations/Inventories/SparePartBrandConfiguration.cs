using Garage_Management.Base.Entities.Inventories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Inventories
{
    /// <summary>
    /// Cấu hình cho bảng SparePartBrand (hãng phụ tùng).
    /// </summary>
    public class SparePartBrandConfiguration : IEntityTypeConfiguration<SparePartBrand>
    {
        public void Configure(EntityTypeBuilder<SparePartBrand> builder)
        {
            builder.HasKey(sb => sb.SparePartBrandId);
            builder.Property(sb => sb.BrandName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(sb => sb.Description)
                .HasMaxLength(500);

            // Quan hệ 1-N: Một hãng phụ tùng có nhiều Inventory
            builder.HasMany(sb => sb.SpareParts)
                .WithOne(i => i.SparePartBrand)
                .HasForeignKey(i => i.SparePartBrandId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}