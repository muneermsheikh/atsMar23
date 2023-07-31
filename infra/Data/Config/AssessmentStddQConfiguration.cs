using core.Entities;
using core.Entities.HR;
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class AssessmentStandardQConfiguration : IEntityTypeConfiguration<AssessmentStandardQ>
     {
          public void Configure(EntityTypeBuilder<AssessmentStandardQ> builder)
          {
               builder.Property(x => x.Subject).IsRequired();
               builder.Property(x => x.QuestionNo).IsRequired();
               builder.HasIndex(x => x.QuestionNo).IsUnique();
               builder.HasIndex(x => x.Question).IsUnique();
               builder.Property(x => x.Question).IsRequired();
               
          }
     }
}