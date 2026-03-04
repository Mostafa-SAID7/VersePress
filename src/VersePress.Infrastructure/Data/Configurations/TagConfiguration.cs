using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Tag entity.
/// Configures many-to-many relationship with BlogPost.
/// </summary>
public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> entity)
    {
        entity.HasKey(e => e.Id);
        
        // Configure indexes
        entity.HasIndex(e => e.Slug).IsUnique();
        entity.HasIndex(e => e.IsDeleted);
        
        // Configure properties
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.NameEn).IsRequired().HasMaxLength(100);
        entity.Property(e => e.NameAr).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
        
        // Configure many-to-many relationship with BlogPost
        entity.HasMany(e => e.BlogPosts)
              .WithMany(p => p.Tags)
              .UsingEntity<Dictionary<string, object>>(
                  "BlogPostTag",
                  j => j.HasOne<BlogPost>().WithMany().HasForeignKey("BlogPostId").OnDelete(DeleteBehavior.Cascade),
                  j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Cascade)
              );
    }
}
