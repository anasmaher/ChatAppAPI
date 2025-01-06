using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configs
{
    public class EmailMetadataConfiguration : IEntityTypeConfiguration<EmailMetadata>
    {
        public void Configure(EntityTypeBuilder<EmailMetadata> builder)
        {
            builder.
        }

        
    }
}
