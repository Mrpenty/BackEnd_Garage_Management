using Garage_Management.Base.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations
{
    /// <summary>
    /// Cấu hình cho bảng Service (danh mục dịch vụ sửa chữa / bảo dưỡng).
    /// </summary>
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(s => s.ServiceId);
            builder.Property(s => s.ServiceName)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(s => s.Description)
                .HasMaxLength(500);
            builder.Property(s => s.BasePrice)
                .HasPrecision(18, 2);
        }
    }
}