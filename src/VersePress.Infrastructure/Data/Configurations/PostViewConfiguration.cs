using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for PostView entity.
/// Tracks unique post views per session for view counting.
/// </summary>
public class PostViewConfiguration : IEntityTypeConfiguration<PostView>
{
    public void Configure(EntityTypeBuilder<PostView> entity)
    {
        entity.HasKey(e => e.Id);
        
        // Configure composite index for efficient querying by post and session
        entity.HasIndex(e => new { e.BlogPostId, e.SessionId });
        entity.HasIndex(e => e.ViewedAt);
        entity.HasIndex(e => e.IsDeleted);
        
        // Configure properties
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.SessionId).IsRequired().HasMaxLength(200);
        entity.Property(e => e.ViewedAt).IsRequired();
        
        // Relationship to BlogPost
        entity.HasOne(e => e.BlogPost)
              .WithMany()
              .HasForeignKey(e => e.BlogPostId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
