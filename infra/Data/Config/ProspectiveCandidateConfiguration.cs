using core.Entities;
using core.Entities.HR;
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class ProspectiveCandidateConfiguration : IEntityTypeConfiguration<ProspectiveCandidate>
     {
          public void Configure(EntityTypeBuilder<ProspectiveCandidate> builder)
          {
               builder.HasIndex(p => p.ResumeId).IsUnique();
          }
     }
}