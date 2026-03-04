using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Notification entity.
/// Configures real-time user notifications.
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> entity)
    {
        entity.HasKey(e => e.Id);
        
        // Configure indexes
        entity.HasIndex(e => e.UserId);
        entity.HasIndex(e => e.IsRead);
        entity.HasIndex(e => e.CreatedAt);
        entity.HasIndex(e => e.IsDeleted);
        
        // Configure properties
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.Type).IsRequired();
        entity.Property(e => e.Content).IsRequired().HasMaxLength(500);
        entity.Property(e => e.IsRead).HasDefaultValue(false);
        
        // Relationship to User
        entity.HasOne(e => e.User)
              .WithMany(u => u.Notifications)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
