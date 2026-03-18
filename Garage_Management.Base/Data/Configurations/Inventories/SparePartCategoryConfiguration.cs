using Garage_Management.Base.Entities.Inventories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Inventories
{
    public class SparePartCategoryConfiguration : IEntityTypeConfiguration<SparePartCategory>
    {
        public void Configure(EntityTypeBuilder<SparePartCategory> builder)
        {
            builder.HasKey(c => c.CategoryId);

            builder.Property(c => c.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            // Quan hệ 1-N: Category có nhiều Inventory
            builder.HasMany(c => c.SparePart)
                .WithOne(i => i.SparePartCategory)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
