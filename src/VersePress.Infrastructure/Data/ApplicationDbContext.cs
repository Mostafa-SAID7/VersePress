using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VersePress.Domain.Entities;

namespace VersePress.Infrastructure.Data;

/// <summary>
/// Application database context inheriting from IdentityDbContext for user management.
/// Implements soft delete pattern with global query filters and automatic timestamp management.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet properties for all entities
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Reaction> Reactions => Set<Reaction>();
    public DbSet<Share> Shares => Set<Share>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Series> Series => Set<Series>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<PostView> PostViews => Set<PostView>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure global query filter for soft delete on all entities inheriting from BaseEntity
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Create expression: e => !e.IsDeleted
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var condition = Expression.Not(property);
                var lambda = Expression.Lambda(condition, parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        // Apply Fluent API configurations
        ConfigureBlogPost(builder);
        ConfigureComment(builder);
        ConfigureReaction(builder);
        ConfigureTag(builder);
        ConfigureCategory(builder);
        ConfigureSeries(builder);
        ConfigureProject(builder);
        ConfigureShare(builder);
        ConfigureNotification(builder);
        ConfigurePostView(builder);
    }

    /// <summary>
    /// Configure BlogPost entity with indexes and relationships.
    /// </summary>
    private void ConfigureBlogPost(ModelBuilder builder)
    {
        builder.Entity<BlogPost>(entity =>
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
            
            // Many-to-many relationships configured separately
        });
    }

    /// <summary>
    /// Configure Comment entity with indexes and self-referencing relationship.
    /// </summary>
    private void ConfigureComment(ModelBuilder builder)
    {
        builder.Entity<Comment>(entity =>
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
        });
    }

    /// <summary>
    /// Configure Reaction entity with composite index and relationships.
    /// </summary>
    private void ConfigureReaction(ModelBuilder builder)
    {
        builder.Entity<Reaction>(entity =>
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
        });
    }

    /// <summary>
    /// Configure Tag entity with many-to-many relationship to BlogPost.
    /// </summary>
    private void ConfigureTag(ModelBuilder builder)
    {
        builder.Entity<Tag>(entity =>
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
        });
    }

    /// <summary>
    /// Configure Category entity with many-to-many relationship to BlogPost.
    /// </summary>
    private void ConfigureCategory(ModelBuilder builder)
    {
        builder.Entity<Category>(entity =>
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
        });
    }

    /// <summary>
    /// Configure Series entity with one-to-many relationship to BlogPost.
    /// </summary>
    private void ConfigureSeries(ModelBuilder builder)
    {
        builder.Entity<Series>(entity =>
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
        });
    }

    /// <summary>
    /// Configure Project entity with one-to-many relationship to BlogPost.
    /// </summary>
    private void ConfigureProject(ModelBuilder builder)
    {
        builder.Entity<Project>(entity =>
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
        });
    }

    /// <summary>
    /// Configure Share entity with relationship to BlogPost.
    /// </summary>
    private void ConfigureShare(ModelBuilder builder)
    {
        builder.Entity<Share>(entity =>
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
            
            // Relationship to BlogPost configured in BlogPost configuration
        });
    }

    /// <summary>
    /// Configure Notification entity with relationship to User.
    /// </summary>
    private void ConfigureNotification(ModelBuilder builder)
    {
        builder.Entity<Notification>(entity =>
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
        });
    }

    /// <summary>
    /// Configure PostView entity with composite index for session-based view tracking.
    /// </summary>
    private void ConfigurePostView(ModelBuilder builder)
    {
        builder.Entity<PostView>(entity =>
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
        });
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically handle timestamps and soft delete pattern.
    /// - Sets CreatedAt and UpdatedAt for new entities
    /// - Updates UpdatedAt for modified entities
    /// - Converts hard deletes to soft deletes by setting IsDeleted flag
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    // Convert hard delete to soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
