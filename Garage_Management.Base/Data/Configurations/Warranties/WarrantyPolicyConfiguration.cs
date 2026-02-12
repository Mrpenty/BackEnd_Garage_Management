using Garage_Management.Base.Entities.Warranties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Warranties
{
    /// <summary>
    /// Cấu hình cho bảng WarrantyPolicy (chính sách bảo hành).
    /// </summary>
    public class WarrantyPolicyConfiguration : IEntityTypeConfiguration<WarrantyPolicy>
    {
        public void Configure(EntityTypeBuilder<WarrantyPolicy> builder)
        {
            builder.HasKey(wp => wp.WarrantyPolicyId);
            builder.Property(wp => wp.PolicyName)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(wp => wp.TermsAndConditions)
                .HasMaxLength(2000);
        }
    }
}