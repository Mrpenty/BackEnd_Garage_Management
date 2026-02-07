using Garage_Management.Base.Entities;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations
{
    /// <summary>
    /// Cấu hình cho bảng Invoice (hóa đơn thanh toán).
    /// </summary>
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(i => i.InvoiceId);
            builder.Property(i => i.ServiceTotal)
                .HasPrecision(18, 2);
            builder.Property(i => i.SparePartTotal)
                .HasPrecision(18, 2);
            builder.Property(i => i.GrandTotal)
                .HasPrecision(18, 2);
            builder.Property(i => i.PaymentStatus)
                .HasMaxLength(50);
            builder.Property(i => i.PaymentMethod)
                .HasMaxLength(50);

            // Quan hệ 1-1: Một Invoice gắn với một JobCard
            builder.HasOne(i => i.JobCard)
                .WithOne(j => j.Invoice)
                .HasForeignKey<Invoice>(i => i.JobCardId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
