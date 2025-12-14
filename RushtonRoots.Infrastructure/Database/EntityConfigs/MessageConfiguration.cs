using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Content)
            .IsRequired();
        
        builder.Property(e => e.SenderUserId)
            .IsRequired();
        
        builder.Property(e => e.MentionedUserIds)
            .HasMaxLength(1000);
        
        builder.HasOne(e => e.Sender)
            .WithMany()
            .HasForeignKey(e => e.SenderUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Recipient)
            .WithMany()
            .HasForeignKey(e => e.RecipientUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.ChatRoom)
            .WithMany(c => c.Messages)
            .HasForeignKey(e => e.ChatRoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.ParentMessage)
            .WithMany(m => m.Replies)
            .HasForeignKey(e => e.ParentMessageId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(e => e.SenderUserId);
        builder.HasIndex(e => e.RecipientUserId);
        builder.HasIndex(e => e.ChatRoomId);
        builder.HasIndex(e => e.CreatedDateTime);
    }
}
