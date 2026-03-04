using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Comment entity.
/// Configures indexes, properties, and self-referencing relationship for nested comments.
/// </summary>
public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> entity)
    {
        entity.HasKey(e => e.Id);
        
        // Configure indexes
        entity.HasIndex(e => e.BlogPostId);
        entity.HasIndex(e => e.CreatedAt);
        entity.HasIndex(e => e.IsDeleted);
        
        // Configure properties
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
        entity.Property(e => e.IsApproved).HasDefaultValue(false);
        
        // Configure self-referencing relationship for nested comments
        entity.HasOne(e => e.ParentComment)
              .WithMany(c => c.Replies)
              .HasForeignKey(e => e.ParentCommentId)
              .OnDelete(DeleteBehavior.Restrict);
        
        // Relationship to User
        entity.HasOne(e => e.User)
              .WithMany(u => u.Comments)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
