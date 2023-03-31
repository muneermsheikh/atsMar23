using core.Entities.AccountsNFinance;
using core.Entities.MasterEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
	public class FinanceConfiguration : IEntityTypeConfiguration<Coa>
     {
          public void Configure(EntityTypeBuilder<Coa> builder)
          {
               builder.Property(p => p.AccountName).IsRequired().HasMaxLength(100);
               builder.HasIndex(p => p.AccountName).IsUnique();
               builder.Property(p => p.Divn).IsRequired().HasMaxLength(1);
               builder.Property(p => p.AccountType).IsRequired().HasMaxLength(1);
          }
     }
}