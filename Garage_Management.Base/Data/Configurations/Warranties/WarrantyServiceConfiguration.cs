using Garage_Management.Base.Entities;
using Garage_Management.Base.Entities.Warranties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Warranties
{
    /// <summary>
    /// Cấu hình cho bảng WarrantyService (bảo hành dịch vụ).
    /// </summary>
    public class WarrantyServiceConfiguration : IEntityTypeConfiguration<WarrantyService>
    {
        public void Configure(EntityTypeBuilder<WarrantyService> builder)
        {
            builder.HasKey(ws => ws.WarrantyServiceId);
            builder.Property(ws => ws.Description)
                .HasMaxLength(500);

            // Quan hệ N-1: WarrantyService gắn với Service
            builder.HasOne(ws => ws.Service)
                .WithMany(s => s.WarrantyServices)
                .HasForeignKey(ws => ws.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-1: Áp dụng WarrantyPolicy
            builder.HasOne(ws => ws.WarrantyPolicy)
                .WithMany(wp => wp.WarrantyServices)
                .HasForeignKey(ws => ws.WarrantyPolicyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}