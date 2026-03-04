using Garage_Management.Base.Entities.Warranties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Warranties
{
    /// <summary>
    /// Cấu hình cho bảng SparePartWarrantyPolicy.
    /// </summary>
    public class SparePartWarrantyPolicyConfiguration : IEntityTypeConfiguration<SparePartWarrantyPolicy>
    {
        public void Configure(EntityTypeBuilder<SparePartWarrantyPolicy> builder)
        {
            builder.HasKey(p => p.PolicyId);
            builder.Property(p => p.PolicyName)
                .IsRequired()
                .HasMaxLength(150);
            builder.Property(p => p.TermsAndConditions)
                .HasMaxLength(2000);
        }
    }
}
