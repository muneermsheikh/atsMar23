using core.Entities;
using core.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class HelpConfiguration : IEntityTypeConfiguration<Help>
     {
          public void Configure(EntityTypeBuilder<Help> builder)
          {
               builder.Property(p => p.Topic).IsRequired().HasMaxLength(50);
               builder.HasIndex(p => p.Topic).IsUnique();
               builder.HasMany(s => s.HelpItems).WithOne().IsRequired().OnDelete(DeleteBehavior.Cascade);
               //builder.HasMany(s => s.HelpItems).WithOne().IsRequired().OnDelete(DeleteBehavior.Cascade);
               
          }
     }
}