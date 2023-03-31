using core.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
	public class DLForwardToAgentConfiguration : IEntityTypeConfiguration<DLForwardToAgent>
     {
          public void Configure(EntityTypeBuilder<DLForwardToAgent> builder)
          {
               builder.Property(p => p.OrderId).IsRequired();
               builder.HasIndex(p => p.OrderId).IsUnique();
               builder.HasMany(s => s.DlForwardCategories).WithOne().IsRequired().OnDelete(DeleteBehavior.Cascade);
          }
     }
}