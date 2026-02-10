using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Data.Configurations.JobCards
{
    public class WorkBayConfiguration : IEntityTypeConfiguration<WorkBay>
    {
        public void Configure(EntityTypeBuilder<WorkBay> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(100);
            // 1 bay có thể có lịch sử nhiều jobcard
            builder.HasOne(w => w.JobCard)
                .WithOne() 
                .HasForeignKey<WorkBay>(w => w.JobcardId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
