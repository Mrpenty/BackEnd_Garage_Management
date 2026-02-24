using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage_Management.Base.Data.Configurations.Services
{
    public partial class ServiceWarrantyPolicyConfiguration
    {
        public class JobCardServiceTaskConfiguration : IEntityTypeConfiguration<JobCardServiceTask>
        {
            public void Configure(EntityTypeBuilder<JobCardServiceTask> builder)
            {
                builder.HasKey(jt => jt.JobCardServiceTaskId);
                //Quan hệ 1-N: Một JobCardService có nhiều JobCardServiceTask
                builder.HasOne(jt => jt.ServiceTask)
                    .WithMany(t => t.JobCardServiceTasks)
                    .HasForeignKey(jt => jt.ServiceTaskId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}