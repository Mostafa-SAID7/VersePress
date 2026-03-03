# Entity Relationship Diagram

## Database Schema

### Core Entities

#### BlogPost
- **Id** (Guid, PK)
- **Slug** (string, unique, indexed)
- **TitleEn** (string, required, max 200)
- **TitleAr** (string, required, max 200)
- **ContentEn** (string, required)
- **ContentAr** (string, required)
- **ExcerptEn** (string, optional)
- **ExcerptAr** (string, optional)
- **FeaturedImageUrl** (string, optional)
- **ViewCount** (int, default 0)
- **IsFeatured** (bool, default false)
- **PublishedAt** (DateTime, indexed)
- **AuthorId** (Guid, FK to User, indexed)
- **SeriesId** (Guid, FK to Series, optional)
- **ProjectId** (Guid, FK to Project, optional)
- **CreatedAt** (DateTime)
- **UpdatedAt** (DateTime)
- **IsDeleted** (bool, soft delete)

**Relationships:**
- One-to-Many with Comment
- One-to-Many with Reaction
- One-to-Many with Share
- Many-to-Many with Tag (via PostTag)
- Many-to-Many with Category (via PostCategory)
- Many-to-One with User (Author)
- Many-to-One with Series
- Many-to-One with Project

#### Comment
- **Id** (Guid, PK)
- **BlogPostId** (Guid, FK to BlogPost, indexed)
- **UserId** (Guid, FK to User)
- **Content** (string, required, max 2000)
- **ParentCommentId** (Guid, FK to Comment, nullable)
- **IsApproved** (bool, default false)
- **CreatedAt** (DateTime, indexed)
- **UpdatedAt** (DateTime)
- **IsDeleted** (bool, soft delete)

**Relationships:**
- Many-to-One with BlogPost
- Many-to-One with User
- Self-referencing (Parent/Child comments)

#### Reaction
- **Id** (Guid, PK)
- **BlogPostId** (Guid, FK to BlogPost, indexed)
- **UserId** (Guid, FK to User)
- **ReactionType** (enum: Like, Love, Celebrate, Insightful, Curious)
- **CreatedAt** (DateTime)
- **UpdatedAt** (DateTime)
- **IsDeleted** (bool, soft delete)

**Composite Index:** (BlogPostId, UserId) - unique

**Relationships:**
- Many-to-One with BlogPost
- Many-to-One with User

#### Share
- **Id** (Guid, PK)
- **BlogPostId** (Guid, FK to BlogPost)
- **Platform** (enum: Twitter, Facebook, LinkedIn, WhatsApp)
- **SharedAt** (DateTime)
- **CreatedAt** (DateTime)
- **UpdatedAt** (DateTime)
- **IsDeleted** (bool, soft delete)

**Relationships:**
- Many-to-One with BlogPost

#### Tag
- **Id** (Guid, PK)
- **NameEn** (string, required)
- **NameAr** (string, required)
- **Slug** (string, unique, indexed)
- **CreatedAt** (DateTime)
- **UpdatedAt** (DateTime)
- **IsDeleted** (bool, soft delete)

**Relationships:**
- Many-to-Many with BlogPost (via PostTag)

#### Category
- **Id** (Guid, PK)
- **NameEn** (string, required)
- **NameAr** (string, required)
- **Slug** (string, unique, indexed)
- **DescriptionEn** (string, optional)
- **DescriptionAr** (string, optional)
- **CreatedAt** (DateTime)
- **UpdatedAt** (DateTime)
- **IsDeleted** (bool, soft delete)

**Relationships:**
- Many-to-Many with BlogPost (via PostCategory)

#### Series
- **Id** (Guid, PK)
- **NameEn** (string, required)
- **NameAr** (string, required)
- **Slug** (string, unique, indexed)
- **DescriptionEn** (string, optional)
- **DescriptionAr** (string, optional)
- **CreatedAt** (DateTime)
- **UpdatedAt** (DateTime)
- **IsDeleted** (bool, soft delete)

**Relationships:**
- One-to-Many with BlogPost

#### Project
- **Id** (Guid, PK)
- **NameEn** (string, required)
- **NameAr** (string, required)
- **Slug** (string, unique, indexed)
- **DescriptionEn** (string, optional)
- **DescriptionAr** (string, optional)
- **CreatedAt** (DateTime)
- **UpdatedAt** (DateTime)
- **IsDeleted** (bool, soft delete)

**Relationships:**
- One-to-Many with BlogPost

#### Notification
- **Id** (Guid, PK)
- **UserId** (Guid, FK to User, indexed)
- **Type** (enum: NewComment, CommentReply, NewReaction)
- **Content** (string, required)
- **RelatedEntityId** (Guid, optional)
- **IsRead** (bool, default false)
- **CreatedAt** (DateTime)
- **UpdatedAt** (DateTime)
- **IsDeleted** (bool, soft delete)

**Relationships:**
- Many-to-One with User

#### User (ASP.NET Core Identity)
- **Id** (Guid, PK)
- **UserName** (string, unique)
- **Email** (string, unique)
- **FullName** (string)
- **Bio** (string, optional)
- **ProfileImageUrl** (string, optional)
- Plus standard Identity fields

**Relationships:**
- One-to-Many with BlogPost (as Author)
- One-to-Many with Comment
- One-to-Many with Reaction
- One-to-Many with Notification

### Junction Tables

#### PostTag
- **BlogPostId** (Guid, FK to BlogPost)
- **TagId** (Guid, FK to Tag)

**Composite PK:** (BlogPostId, TagId)

#### PostCategory
- **BlogPostId** (Guid, FK to BlogPost)
- **CategoryId** (Guid, FK to Category)

**Composite PK:** (BlogPostId, CategoryId)

## Database Indexes

### Performance-Critical Indexes
- `BlogPost.Slug` (unique)
- `BlogPost.PublishedAt`
- `BlogPost.AuthorId`
- `Comment.BlogPostId`
- `Comment.CreatedAt`
- `Reaction.BlogPostId`
- `Notification.UserId`
- `Tag.Slug` (unique)
- `Category.Slug` (unique)
- `Series.Slug` (unique)
- `Project.Slug` (unique)

### Composite Indexes
- `(Reaction.BlogPostId, Reaction.UserId)` - unique
- `(Comment.BlogPostId, Comment.IsApproved)`

## Cascade Delete Behaviors

- **BlogPost deletion** → CASCADE to Comments, Reactions, Shares
- **User deletion** → RESTRICT (prevent if has blog posts)
- **Comment deletion** → CASCADE to child comments
- **Tag/Category deletion** → CASCADE junction table entries

## Soft Delete Pattern

All entities inherit from `BaseEntity` which includes:
- `IsDeleted` (bool)
- Global query filter: `e => !e.IsDeleted`
- Automatic timestamp management (CreatedAt, UpdatedAt)

## Entity Framework Core Configuration

All entities are configured using Fluent API in `ApplicationDbContext.OnModelCreating`:
- Relationships and foreign keys
- Indexes and unique constraints
- Cascade delete behaviors
- Value conversions for enums
- Query filters for soft delete
- Default values
- Max length constraints
