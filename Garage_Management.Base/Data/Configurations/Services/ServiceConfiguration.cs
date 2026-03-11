using Garage_Management.Base.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Services
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
            // Quan hệ 1-n: Một Service có nhiều ServiceTask
            builder.HasMany(s => s.ServiceTasks)
                   .WithOne(t => t.Service)
                   .HasForeignKey(t => t.ServiceId)
                   .OnDelete(DeleteBehavior.Cascade); // Xóa Service → xóa luôn các task con

            // Quan hệ 1-n: Một Service được sử dụng trong nhiều phiếu (JobCardService)
            builder.HasMany(s => s.JobCardServices)
                   .WithOne(js => js.Service)
                   .HasForeignKey(js => js.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Service nếu đang dùng trong phiếu

            builder.HasMany(s => s.ServiceVehicleTypes)
                   .WithOne(sv => sv.Service)
                   .HasForeignKey(sv => sv.ServiceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
