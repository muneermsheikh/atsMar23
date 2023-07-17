using core.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
    public class DeployConfiguration : IEntityTypeConfiguration<Deployment>
     {
        public void Configure(EntityTypeBuilder<Deployment> builder)
        {
            builder.HasIndex(x => x.DeployCVRefId);
            builder.Property(x => x.TransactionDate).IsRequired();
            builder.HasOne(p => p.CVRef).WithMany().HasForeignKey(p => p.DeployCVRefId).OnDelete(DeleteBehavior.Restrict);
        }
     }
}