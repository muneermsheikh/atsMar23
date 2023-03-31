using core.Entities.AccountsNFinance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
	public class VoucherEntryConfiguration : IEntityTypeConfiguration<VoucherEntry>
     {
          public void Configure(EntityTypeBuilder<VoucherEntry> builder)
          {
               builder.Property(p => p.CoaId).IsRequired();
               builder.HasIndex(p => p.CoaId);
               builder.HasIndex(p => p.TransDate);
          }
     }
}