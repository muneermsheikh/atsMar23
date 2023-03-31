using System;
using core.Entities.Admin;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class UserHistoryHeaderConfiguration : IEntityTypeConfiguration<UserHistoryHeader>
     {
          public void Configure(EntityTypeBuilder<UserHistoryHeader> builder)
          {
               //builder.HasMany(o => o.UserHistories).WithOne().OnDelete(DeleteBehavior.NoAction);
          }
     }
}