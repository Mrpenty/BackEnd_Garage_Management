using Garage_Management.Base.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Services
{
    /// <summary>
    /// Cấu hình cho bảng trung gian ServiceVehicleType.
    /// </summary>
    public class ServiceVehicleTypeConfiguration : IEntityTypeConfiguration<ServiceVehicleType>
    {
        public void Configure(EntityTypeBuilder<ServiceVehicleType> builder)
        {
            builder.HasKey(x => new { x.ServiceId, x.VehicleTypeId });

            builder.HasOne(x => x.Service)
                .WithMany(s => s.ServiceVehicleTypes)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.VehicleType)
                .WithMany(vt => vt.ServiceVehicleTypes)
                .HasForeignKey(x => x.VehicleTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
