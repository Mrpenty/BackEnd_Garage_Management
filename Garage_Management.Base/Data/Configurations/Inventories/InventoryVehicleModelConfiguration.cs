using Garage_Management.Base.Entities.Inventories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Inventories
{
    public class InventoryVehicleModelConfiguration : IEntityTypeConfiguration<InventoryVehicleModel>
    {
        public void Configure(EntityTypeBuilder<InventoryVehicleModel> builder)
        {
            builder.HasKey(ivm => new { ivm.SparePartId, ivm.VehicleModelId });

            builder.HasOne(ivm => ivm.Inventory)
                .WithMany(i => i.CompatibleModels)
                .HasForeignKey(ivm => ivm.SparePartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ivm => ivm.VehicleModel)
                .WithMany(vm => vm.CompatibleSpareParts)
                .HasForeignKey(ivm => ivm.VehicleModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
