using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configs
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<UserRelationship>
    {
        public void Configure(EntityTypeBuilder<UserRelationship> builder)
        {
            // Define Primary Key
            builder.HasKey(f => f.Id);

            // Define Foreign Keys and Relationships
            builder.HasOne(f => f.User1)
                .WithMany()
                .HasForeignKey(f => f.User1Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.User2Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(f => f.ActionUser)
                .WithMany()
                .HasForeignKey(f => f.ActionUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Enforce Unique Constraint on UserId1 and UserId2
            builder.HasIndex(f => new { f.User1Id, f.User2Id })
                .IsUnique();

            // Create Indexes for Performance
            builder.HasIndex(f => f.User1Id);
            builder.HasIndex(f => f.User2Id);
            builder.HasIndex(f => f.Status);
        }
    }
}
