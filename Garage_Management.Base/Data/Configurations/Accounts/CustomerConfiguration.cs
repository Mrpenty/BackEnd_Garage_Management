using Garage_Management.Base.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Accounts
{
    /// <summary>
    /// Cấu hình cho bảng Customer.
    /// </summary>
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.CustomerId);
            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.Address)
                .HasMaxLength(500);

            // Quan hệ 1-1: Customer có thể gắn với một User 
            builder.HasOne(c => c.User)
               .WithOne()
               .HasForeignKey<Customer>(c => c.UserId)
               .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
