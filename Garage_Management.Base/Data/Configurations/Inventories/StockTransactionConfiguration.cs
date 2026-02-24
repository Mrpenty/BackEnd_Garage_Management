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
            builder.Property(st => st.Note)
                .HasMaxLength(500);
            builder.Property(st => st.UnitPrice)
                .HasPrecision(18, 2);

            // Quan hệ N-1: Giao dịch kho gắn với một Inventory (phụ tùng)
            builder.HasOne(st => st.Inventory)
                .WithMany()
                .HasForeignKey(st => st.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}