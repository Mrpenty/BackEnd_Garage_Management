using Garage_Management.Base.Entities.Inventories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Inventories
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(s => s.SupplierId);

            builder.Property(s => s.SupplierName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Phone)
                .HasMaxLength(20);

            builder.Property(s => s.Address)
                .HasMaxLength(500);

            builder.Property(s => s.TaxCode)
                .HasMaxLength(20);
        }
    }
}
