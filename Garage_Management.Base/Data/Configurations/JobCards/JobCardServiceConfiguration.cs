using Garage_Management.Base.Entities;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.JobCards
{
    /// <summary>
    /// Cấu hình cho bảng trung gian JobCardService (dịch vụ thực hiện trên phiếu sửa chữa).
    /// </summary>
    public class JobCardServiceConfiguration : IEntityTypeConfiguration<JobCardService>
    {
        public void Configure(EntityTypeBuilder<JobCardService> builder)
        {
            builder.HasKey(js => js.JobCardServiceId);
            builder.Property(js => js.Description)
                .HasMaxLength(1000);
            builder.Property(js => js.Price)
                .HasPrecision(18, 2)
                .IsRequired();
            // Quan hệ với Service 
            builder.HasOne(js => js.Service)
                   .WithMany(s => s.JobCardServices)
                   .HasForeignKey(js => js.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}