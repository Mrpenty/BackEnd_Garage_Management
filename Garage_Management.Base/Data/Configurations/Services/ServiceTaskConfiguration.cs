using Garage_Management.Base.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Services
{
    /// <summary>
    /// Cấu hình cho bảng ServiceTask (công việc dịch vụ).
    /// </summary>
    public class ServiceTaskConfiguration : IEntityTypeConfiguration<ServiceTask>
    {
        public void Configure(EntityTypeBuilder<ServiceTask> builder)
        {
            builder.HasKey(t => t.ServiceTaskId);
            builder.Property(t => t.TaskName)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}