using Garage_Management.Base.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Services
{
    public partial class ServiceWarrantyPolicyConfiguration : IEntityTypeConfiguration<ServiceWarrantyPolicy>
    {
        public void Configure(EntityTypeBuilder<ServiceWarrantyPolicy> builder)
        {
            builder.HasKey(p => p.PolicyId);
            builder.Property(p => p.PolicyName)
                .IsRequired()
                .HasMaxLength(150);
        }
    }
}