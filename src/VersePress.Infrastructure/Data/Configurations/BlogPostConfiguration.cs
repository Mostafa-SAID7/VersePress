using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for BlogPost entity.
/// Configures indexes, properties, and relationships.
/// </summary>
public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> entity)
    {
        entity.HasKey(e => e.Id);
        
        // Configure indexes
        entity.HasIndex(e => e.Slug).IsUnique();
        entity.HasIndex(e => e.PublishedAt);
        entity.HasIndex(e => e.AuthorId);
        entity.HasIndex(e => e.IsDeleted);
        
        // Configure properties
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.Slug).IsRequired().HasMaxLength(250);
        entity.Property(e => e.TitleEn).IsRequired().HasMaxLength(200);
        entity.Property(e => e.TitleAr).IsRequired().HasMaxLength(200);
        entity.Property(e => e.ContentEn).IsRequired();
        entity.Property(e => e.ContentAr).IsRequired();
        entity.Property(e => e.ViewCount).HasDefaultValue(0);
        entity.Property(e => e.IsFeatured).HasDefaultValue(false);
        
        // Configure relationships
        entity.HasOne(e => e.Author)
              .WithMany(u => u.BlogPosts)
              .HasForeignKey(e => e.AuthorId)
              .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(e => e.Series)
              .WithMany(s => s.BlogPosts)
              .HasForeignKey(e => e.SeriesId)
              .OnDelete(DeleteBehavior.SetNull);
        
        entity.HasOne(e => e.Project)
              .WithMany(p => p.BlogPosts)
              .HasForeignKey(e => e.ProjectId)
              .OnDelete(DeleteBehavior.SetNull);
        
        entity.HasMany(e => e.Comments)
              .WithOne(c => c.BlogPost)
              .HasForeignKey(c => c.BlogPostId)
              .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasMany(e => e.Reactions)
              .WithOne(r => r.BlogPost)
              .HasForeignKey(r => r.BlogPostId)
              .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasMany(e => e.Shares)
              .WithOne(s => s.BlogPost)
              .HasForeignKey(s => s.BlogPostId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
