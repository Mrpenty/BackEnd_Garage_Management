using Garage_Management.Base.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Accounts
{
    /// <summary>
    /// Cấu hình cho bảng User (tài khoản đăng nhập hệ thống).
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);               
            builder.Property(u => u.Email)
                .HasMaxLength(100);
            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            // Quan hệ N-1: User thuộc một Role
            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}