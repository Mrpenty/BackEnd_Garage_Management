using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.JobCards
{
    /// <summary>
    /// Cấu hình cho bảng trung gian JobCardMechanic (phân công thợ máy cho phiếu sửa chữa).
    /// </summary>
    public class JobCardMechanicConfiguration : IEntityTypeConfiguration<JobCardMechanic>
    {
        public void Configure(EntityTypeBuilder<JobCardMechanic> builder)
        {
            builder.HasKey(jm => new { jm.JobCardId, jm.UserId });
            builder.Property(jm => jm.Note)
                .HasMaxLength(500);

            // Quan hệ N-1: JobCardMechanic thuộc một JobCard
            builder.HasOne(jm => jm.JobCard)
                .WithMany(j => j.Mechanics)
                .HasForeignKey(jm => jm.JobCardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ N-1: Thợ máy (User) được phân công
            builder.HasOne(jm => jm.User)
                .WithMany(u => u.AssignedMechanics)
                .HasForeignKey(jm => jm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-1: Người phân công thợ (audit)
            builder.HasOne(jm => jm.AssignedByUser)
                .WithMany()
                .HasForeignKey(jm => jm.AssignedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}