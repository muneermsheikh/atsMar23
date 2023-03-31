using core.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
	public class DateForwardCatgoryConfiguration : IEntityTypeConfiguration<DLForwardCategory>
     {
          public void Configure(EntityTypeBuilder<DLForwardCategory> builder)
          {
               builder.Property(p => p.OrderItemId).IsRequired();
               builder.HasIndex(p => p.OrderItemId).IsUnique();
               builder.HasMany(s => s.DlForwardCategoryOfficials).WithOne().IsRequired().OnDelete(DeleteBehavior.Cascade);
          }
     }
}