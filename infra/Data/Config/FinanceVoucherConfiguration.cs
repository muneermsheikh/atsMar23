using core.Entities.AccountsNFinance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
	public class FinanceVoucherConfiguration : IEntityTypeConfiguration<FinanceVoucher>
     {
          public void Configure(EntityTypeBuilder<FinanceVoucher> builder)
          {
               builder.Property(p => p.VoucherNo).IsRequired().HasMaxLength(10);
               builder.HasIndex(p => p.VoucherNo).IsUnique();
               builder.HasIndex(P => P.VoucherDated);

               builder.HasMany(s => s.VoucherEntries).WithOne().OnDelete(DeleteBehavior.Cascade);
               builder.HasMany(s => s.VoucherAttachments).WithOne().OnDelete(DeleteBehavior.Cascade);
          }
     }
}