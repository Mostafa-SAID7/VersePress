using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Reaction entity.
/// Configures composite unique index to ensure one reaction per user per post.
/// </summary>
public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> entity)
    {
        entity.HasKey(e => e.Id);
        
        // Configure composite index for unique constraint (one reaction per user per post)
        entity.HasIndex(e => new { e.BlogPostId, e.UserId }).IsUnique();
        
        // Configure properties
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.ReactionType).IsRequired();
        
        // Relationship to User
        entity.HasOne(e => e.User)
              .WithMany(u => u.Reactions)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
