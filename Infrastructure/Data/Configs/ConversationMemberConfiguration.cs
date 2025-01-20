using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configs
{
    public class ConversationMemberConfiguration : IEntityTypeConfiguration<ConversationMember>
    {
        public void Configure(EntityTypeBuilder<ConversationMember> builder)
        {
            builder.HasKey(cm => new { cm.UserId, cm.ConversationId });

            builder
                .HasOne(cm => cm.User)
                .WithMany(u => u.Conversations)
                .HasForeignKey(cm => cm.UserId);

            builder
                .HasOne(cm => cm.Conversation)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.ConversationId);
        }
    }
}
