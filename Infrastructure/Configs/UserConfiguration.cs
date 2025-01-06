using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configs
{
    public class UserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.FirstName)
               .HasMaxLength(30)
               .IsRequired();

            builder.Property(u => u.LastName)
               .HasMaxLength(30)
               .IsRequired();

            builder.Property(u => u.PhotoFilePath)
               .HasMaxLength(500);

            builder.Property(u => u.RefreshToken)
               .HasMaxLength(200);
        }
    }
}
