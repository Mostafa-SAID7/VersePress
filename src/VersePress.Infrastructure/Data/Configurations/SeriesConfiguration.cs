using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Series entity.
/// Configures one-to-many relationship with BlogPost.
/// </summary>
public class SeriesConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> entity)
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
