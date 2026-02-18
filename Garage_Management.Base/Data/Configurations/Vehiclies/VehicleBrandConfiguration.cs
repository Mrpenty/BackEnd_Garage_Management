using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Vehiclies
{
    /// <summary>
    /// Cấu hình cho bảng VehicleBrand (hãng xe).
    /// </summary>
    public class VehicleBrandConfiguration : IEntityTypeConfiguration<VehicleBrand>
    {
        public void Configure(EntityTypeBuilder<VehicleBrand> builder)
        {
            builder.HasKey(vb => vb.BrandId);
            builder.Property(vb => vb.BrandName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(vb => vb.IsActive)
                .HasDefaultValue(true);

            // Quan hệ 1-N: Một hãng xe có nhiều VehicleModel
            builder.HasMany(vb => vb.Models)
                .WithOne(m => m.Brand)
                .HasForeignKey(m => m.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
