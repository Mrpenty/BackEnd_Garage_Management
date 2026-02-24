using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Vehiclies
{
    /// <summary>
    /// Cấu hình cho bảng Vehicle (xe của khách hàng).
    /// </summary>
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasKey(v => v.VehicleId);
            builder.Property(v => v.LicensePlate)
                .HasMaxLength(20);
            builder.Property(v => v.Vin)
                .HasMaxLength(50);

            // Quan hệ N-1: Vehicle thuộc một Customer
            builder.HasOne(v => v.Customer)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-1: Vehicle thuộc một VehicleBrand
            builder.HasOne(v => v.Brand)
                .WithMany(vb => vb.Vehicles)
                .HasForeignKey(v => v.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-1: Vehicle thuộc một VehicleModel
            builder.HasOne(v => v.Model)
                .WithMany(vm => vm.Vehicles)
                .HasForeignKey(v => v.ModelId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}