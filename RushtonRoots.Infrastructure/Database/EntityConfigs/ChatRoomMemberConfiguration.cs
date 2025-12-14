using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class ChatRoomMemberConfiguration : IEntityTypeConfiguration<ChatRoomMember>
{
    public void Configure(EntityTypeBuilder<ChatRoomMember> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.ChatRoomId)
            .IsRequired();
        
        builder.Property(e => e.UserId)
            .IsRequired();
        
        builder.Property(e => e.Role)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasOne(e => e.ChatRoom)
            .WithMany(c => c.Members)
            .HasForeignKey(e => e.ChatRoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(e => new { e.ChatRoomId, e.UserId })
            .IsUnique();
        
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.IsActive);
    }
}
