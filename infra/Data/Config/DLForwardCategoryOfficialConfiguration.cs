using core.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
	public class DLForwardCategoryOfficialConfiguration : IEntityTypeConfiguration<DLForwardCategoryOfficial>
     {
          public void Configure(EntityTypeBuilder<DLForwardCategoryOfficial> builder)
          {
               builder.Property(p => p.DateTimeForwarded).IsRequired();
               builder.Property(p => p.CustomerOfficialId).IsRequired();
               builder.Property(p => p.LoggedInEmployeeId).IsRequired();
               builder.HasIndex(p => new {p.DLForwardCategoryId, p.DateOnlyForwarded, p.CustomerOfficialId}).IsUnique();
          }
     }
}