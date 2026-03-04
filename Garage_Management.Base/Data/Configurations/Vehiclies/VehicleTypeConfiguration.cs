using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Vehiclies
{
    /// <summary>
    /// Cấu hình cho bảng VehicleType.
    /// </summary>
    public class VehicleTypeConfiguration : IEntityTypeConfiguration<VehicleType>
    {
        public void Configure(EntityTypeBuilder<VehicleType> builder)
        {
            builder.HasKey(x => x.VehicleTypeId);
            builder.Property(x => x.TypeName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.Description)
                .HasMaxLength(500);
            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);
        }
    }
}
