using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.JobCards
{
    /// <summary>
    /// Cấu hình cho bảng JobCardLog (nhật ký thay đổi trên phiếu sửa chữa).
    /// </summary>
    public class JobCardLogConfiguration : IEntityTypeConfiguration<JobCardLog>
    {
        public void Configure(EntityTypeBuilder<JobCardLog> builder)
        {
            builder.HasKey(jl => jl.JobCardLogId);
            builder.Property(jl => jl.Action)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(jl => jl.Note)
                .HasMaxLength(1000);

            // Quan hệ N-1: Log thuộc một JobCard
            builder.HasOne(jl => jl.JobCard)
                .WithMany(j => j.Logs)
                .HasForeignKey(jl => jl.JobCardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ N-1: User thực hiện hành động
            builder.HasOne(jl => jl.User)
                .WithMany()
                .HasForeignKey(jl => jl.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}