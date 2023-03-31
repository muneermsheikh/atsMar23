using core.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Config
{
     public class CVsRefCountDtoConfiguration : IEntityTypeConfiguration<CVsRefCountDto>
     {
        public void Configure(EntityTypeBuilder<CVsRefCountDto> builder)
        {
            builder.HasNoKey();
            builder.ToView("ats_CVsSubmitted");
            builder.Property("OrderItemId").HasColumnName("OrderItemId");
        }
     }
}