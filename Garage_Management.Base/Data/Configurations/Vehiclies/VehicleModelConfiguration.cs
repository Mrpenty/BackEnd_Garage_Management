using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Vehiclies
{
    /// <summary>
    /// Cấu hình cho bảng VehicleModel (dòng xe / model).
    /// </summary>
    public class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
    {
        public void Configure(EntityTypeBuilder<VehicleModel> builder)
        {
            builder.HasKey(vm => vm.ModelId);
            builder.Property(vm => vm.ModelName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(vm => vm.IsActive)
                .HasDefaultValue(true);

            // Quan hệ N-1: VehicleModel thuộc một VehicleBrand
            builder.HasOne(vm => vm.Brand)
                .WithMany(vb => vb.Models)
                .HasForeignKey(vm => vm.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
