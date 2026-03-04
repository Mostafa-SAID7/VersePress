using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Category entity.
/// Configures many-to-many relationship with BlogPost.
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> entity)
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
              .WithMany(p => p.Categories)
              .UsingEntity<Dictionary<string, object>>(
                  "BlogPostCategory",
                  j => j.HasOne<BlogPost>().WithMany().HasForeignKey("BlogPostId").OnDelete(DeleteBehavior.Cascade),
                  j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId").OnDelete(DeleteBehavior.Cascade)
              );
    }
}
