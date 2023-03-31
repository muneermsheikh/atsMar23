using core.Entities;
using core.Entities.Attachments;
using core.Entities.MasterEntities;
using core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class VoucherAttachmentConfiguration : IEntityTypeConfiguration<VoucherAttachment>
     {
          public void Configure(EntityTypeBuilder<VoucherAttachment> builder)
          {
               builder.Property(p => p.FinanceVoucherId).IsRequired();
               builder.Property(p => p.FileName).IsRequired();
               builder.Property(p => p.Url).IsRequired().HasMaxLength(50);
               builder.HasOne(p => p.FinanceVoucher).WithMany().HasForeignKey(p => p.FinanceVoucherId);
          }
     }
}