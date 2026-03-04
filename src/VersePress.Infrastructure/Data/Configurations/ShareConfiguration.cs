using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Share entity.
/// Tracks social media shares of blog posts.
/// </summary>
public class ShareConfiguration : IEntityTypeConfiguration<Share>
{
    public void Configure(EntityTypeBuilder<Share> entity)
    {
        entity.HasKey(e => e.Id);
        
        // Configure indexes
        entity.HasIndex(e => e.BlogPostId);
        entity.HasIndex(e => e.SharedAt);
        entity.HasIndex(e => e.IsDeleted);
        
        // Configure properties
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.Platform).IsRequired();
        entity.Property(e => e.SharedAt).IsRequired();
    }
}
