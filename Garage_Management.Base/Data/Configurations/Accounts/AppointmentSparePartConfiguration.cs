using Garage_Management.Base.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Accounts
{
    /// <summary>
    /// Configuration for AppointmentSparePart join table.
    /// </summary>
    public class AppointmentSparePartConfiguration : IEntityTypeConfiguration<AppointmentSparePart>
    {
        public void Configure(EntityTypeBuilder<AppointmentSparePart> builder)
        {
            builder.HasKey(x => new { x.AppointmentId, x.SparePartId });

            builder.HasOne(x => x.Appointment)
                .WithMany(a => a.SpareParts)
                .HasForeignKey(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Inventory)
                .WithMany(i => i.AppointmentSpareParts)
                .HasForeignKey(x => x.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
