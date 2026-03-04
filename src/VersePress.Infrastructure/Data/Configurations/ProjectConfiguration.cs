using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Project entity.
/// Configures one-to-many relationship with BlogPost.
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> entity)
    {
        entity.HasKey(e => e.Id);
        
        // Configure indexes
        entity.HasIndex(e => e.Slug).IsUnique();
        entity.HasIndex(e => e.IsDeleted);
        
        // Configure properties
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
        entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
    }
}
