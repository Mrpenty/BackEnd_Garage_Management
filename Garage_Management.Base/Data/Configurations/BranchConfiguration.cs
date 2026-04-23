using Garage_Management.Base.Entities.Branches;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations
{
    /// <summary>
    /// Cấu hình cho bảng Branch (chi nhánh gara).
    /// </summary>
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(b => b.BranchId);

            builder.Property(b => b.BranchCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(b => b.BranchCode).IsUnique();

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Address)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(b => b.Phone)
                .HasMaxLength(20);

            builder.Property(b => b.Email)
                .HasMaxLength(100);

            builder.HasOne(b => b.ManagerEmployee)
                .WithMany()
                .HasForeignKey(b => b.ManagerEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(b => b.Employees)
                .WithOne(e => e.Branch)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Inventories)
                .WithOne(i => i.Branch)
                .HasForeignKey(i => i.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.JobCards)
                .WithOne(j => j.Branch)
                .HasForeignKey(j => j.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Invoices)
                .WithOne(i => i.Branch)
                .HasForeignKey(i => i.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Appointments)
                .WithOne(a => a.Branch)
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.WorkBays)
                .WithOne(w => w.Branch)
                .HasForeignKey(w => w.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.StockTransactions)
                .WithOne(s => s.Branch)
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
