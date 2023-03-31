using core.Entities;
using core.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class HelpItemConfiguration : IEntityTypeConfiguration<HelpItem>
     {
          public void Configure(EntityTypeBuilder<HelpItem> builder)
          {
               builder.Property(p => p.HelpText).IsRequired().HasMaxLength(250);
               builder.HasIndex(p => new {p.Sequence, p.HelpId}).IsUnique();
               builder.HasMany(s => s.HelpSubItems).WithOne().OnDelete(DeleteBehavior.Cascade);
          }
     }
}