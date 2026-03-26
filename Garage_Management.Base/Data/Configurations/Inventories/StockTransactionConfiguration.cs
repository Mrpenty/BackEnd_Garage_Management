using Garage_Management.Base.Entities.Inventories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Inventories
{
    /// <summary>
    /// Cấu hình cho bảng StockTransaction (giao dịch nhập/xuất kho).
    /// </summary>
    public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
    {
        public void Configure(EntityTypeBuilder<StockTransaction> builder)
        {
            builder.HasKey(st => st.StockTransactionId);

            builder.Property(st => st.ReceiptCode)
                .HasMaxLength(50);

            builder.Property(st => st.LotNumber)
                .HasMaxLength(100);

            builder.Property(st => st.SerialNumber)
                .HasMaxLength(100);

            builder.Property(st => st.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(st => st.Note)
                .HasMaxLength(500);

            // Quan hệ N-1: Giao dịch kho gắn với một Inventory (phụ tùng)
            builder.HasOne(st => st.Inventory)
                .WithMany(i => i.StockTransactions)
                .HasForeignKey(st => st.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ N-1: Giao dịch kho gắn với một Supplier (khi nhập)
            builder.HasOne(st => st.Supplier)
                .WithMany(s => s.StockTransactions)
                .HasForeignKey(st => st.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ N-1: Giao dịch kho gắn với một JobCard (khi xuất)
            builder.HasOne(st => st.JobCard)
                .WithMany()
                .HasForeignKey(st => st.JobCardId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
