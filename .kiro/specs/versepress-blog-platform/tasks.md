# Implementation Plan: VersePress Blog Platform

## Overview

This implementation plan breaks down the VersePress bilingual blog platform into discrete coding tasks following Clean Architecture principles. The platform will be built using ASP.NET Core 9 MVC with four layers: Domain, Application, Infrastructure, and Web. Tasks are organized to build incrementally from core domain models through to complete features with real-time capabilities.

## Tasks

- [x] 1. Set up solution structure and core domain entities
  - Create solution with four projects: VersePress.Domain, VersePress.Application, VersePress.Infrastructure, VersePress.Web
  - Configure project references following Clean Architecture dependency rules
  - Add NuGet packages: EF Core, ASP.NET Core Identity, FluentValidation, Serilog, xUnit
  - Create domain entities: BlogPost, Comment, Reaction, Share, Tag, Category, Series, Project, Notification
  - Define enums: ReactionType, NotificationType, Platform
  - _Requirements: 1.1, 1.7, 4.3, 8.2, 9.1, 19.3, 28.1, 28.2_

- [x] 2. Implement domain interfaces and repository contracts
  - [x] 2.1 Create generic IRepository<T> interface with async CRUD operations
    - Define GetByIdAsync, GetAllAsync, AddAsync, UpdateAsync, DeleteAsync, ExistsAsync methods
    - _Requirements: 18.1, 18.4, 18.6_
  
  - [x] 2.2 Create specialized repository interfaces
    - Implement IBlogPostRepository with GetBySlugAsync, GetPublishedPostsAsync, GetFeaturedPostsAsync, GetPostsByAuthorAsync, SearchPostsAsync, SlugExistsAsync
    - Implement ICommentRepository with GetCommentsByPostAsync, GetPendingCommentsAsync, GetPendingCommentCountAsync
    - Implement IReactionRepository with GetUserReactionAsync, GetReactionCountsAsync
    - Implement INotificationRepository with GetUserNotificationsAsync, GetUnreadCountAsync, MarkAsReadAsync
    - _Requirements: 1.4, 5.3, 6.1, 7.1, 13.2, 14.1, 19.1, 22.1_


- [x] 3. Implement Application layer DTOs and validators
  - [x] 3.1 Create DTOs for data transfer
    - Create BlogPostDto, CommentDto, ReactionDto, ShareDto, TagDto, CategoryDto, SeriesDto, ProjectDto, NotificationDto
    - Include computed fields (reaction counts, comment counts) in BlogPostDto
    - _Requirements: 4.5, 5.1, 7.3, 8.3, 13.1, 19.4_
  
  - [x] 3.2 Create command DTOs
    - Create CreateBlogPostCommand, UpdateBlogPostCommand, DeleteBlogPostCommand
    - Create CreateCommentCommand, ApproveCommentCommand, RejectCommentCommand
    - Create AddReactionCommand, RemoveReactionCommand
    - Create CreateTagCommand, CreateCategoryCommand, CreateSeriesCommand, CreateProjectCommand
    - Create SubmitContactFormCommand
    - _Requirements: 1.2, 5.1, 6.2, 6.3, 10.1, 15.1, 15.2, 28.1, 28.2, 29.2_
  
  - [x] 3.3 Implement FluentValidation validators
    - Create BlogPostValidator: validate TitleEn/TitleAr (5-200 chars), ContentEn/ContentAr (min 100 chars), Slug pattern ^[a-z0-9-]+$
    - Create CommentValidator: validate Content (1-2000 chars), BlogPostId exists, ParentCommentId exists if provided
    - Create ContactFormValidator: validate Name (2-100 chars), Email format, Subject (5-200 chars), Message (10-5000 chars)
    - Create TagValidator, CategoryValidator, SeriesValidator, ProjectValidator for bilingual name validation
    - _Requirements: 10.2, 21.1, 21.2, 21.3, 21.4, 21.5, 21.6, 21.8, 29.2, 29.3_

- [x] 4. Implement Infrastructure layer - Database configuration
  - [x] 4.1 Create ApplicationDbContext inheriting from IdentityDbContext
    - Configure DbSet properties for all entities
    - Override OnModelCreating for Fluent API configurations
    - _Requirements: 9.1, 18.2_
  
  - [x] 4.2 Configure entity relationships using Fluent API
    - Configure BlogPost: indexes on Slug (unique), PublishedAt, AuthorId; relationships to Author, Comments, Reactions, Shares, Tags, Categories, Series, Project
    - Configure Comment: indexes on BlogPostId, CreatedAt; self-referencing relationship for ParentComment/Replies
    - Configure Reaction: composite index on (BlogPostId, UserId); relationships to BlogPost and User
    - Configure many-to-many relationships: BlogPost-Tag, BlogPost-Category using junction entities
    - Configure cascade delete behaviors: BlogPost deletion cascades to Comments/Reactions/Shares; User deletion restricted if has posts
    - _Requirements: 1.1, 1.4, 1.7, 5.2, 5.3, 12.8, 15.3, 15.4, 18.2, 18.3, 28.3, 28.4_
  
  - [x] 4.3 Create database migrations
    - Generate initial migration with all entities and configurations
    - Configure automatic migration application on startup in development environment
    - _Requirements: 18.8_


- [x] 5. Implement repository pattern in Infrastructure layer
  - [x] 5.1 Create generic Repository<T> base class
    - Implement IRepository<T> with async EF Core operations
    - Inject ApplicationDbContext via constructor
    - _Requirements: 18.1, 18.4, 18.5, 18.6_
  
  - [x] 5.2 Implement specialized repositories
    - Create BlogPostRepository implementing IBlogPostRepository with all query methods including eager loading of related entities
    - Create CommentRepository implementing ICommentRepository with nested comment loading and approval filtering
    - Create ReactionRepository implementing IReactionRepository with aggregation methods
    - Create NotificationRepository implementing INotificationRepository with user-specific queries
    - Create generic repositories for Tag, Category, Series, Project, Share
    - _Requirements: 1.4, 5.3, 5.6, 6.1, 7.1, 13.2, 14.1, 14.2, 19.1, 22.1, 22.2, 23.2_
  
  - [x] 5.3 Write unit tests for repository implementations
    - Test CRUD operations with in-memory database
    - Test specialized query methods with sample data
    - Test cascade delete behaviors
    - _Requirements: 18.1, 26.6_

- [x] 6. Implement Application layer services
  - [x] 6.1 Create BlogPostService
    - Implement CreateBlogPostAsync: validate input, generate unique slug, save to repository
    - Implement UpdateBlogPostAsync: validate ownership, update fields, save changes
    - Implement DeleteBlogPostAsync: verify ownership, delete via repository
    - Implement GetBlogPostBySlugAsync, GetPublishedPostsAsync, GetFeaturedPostsAsync
    - Implement ToggleFeaturedAsync for marking posts as featured
    - _Requirements: 1.2, 1.3, 1.4, 1.5, 1.6, 10.1, 10.3, 10.4, 10.5, 10.6, 10.7, 22.1, 22.2_
  
  - [x] 6.2 Create CommentService
    - Implement CreateCommentAsync: validate input, save comment with IsApproved=false, trigger notification
    - Implement ApproveCommentAsync: set IsApproved=true, broadcast via SignalR
    - Implement RejectCommentAsync: delete comment from database
    - Implement GetCommentsByPostAsync with nested structure loading
    - _Requirements: 5.1, 5.2, 5.4, 5.5, 5.6, 6.1, 6.2, 6.3, 6.5, 19.2_
  
  - [x] 6.3 Create ReactionService
    - Implement AddReactionAsync: check for existing reaction, replace or create new, broadcast update
    - Implement RemoveReactionAsync: delete reaction, broadcast update
    - Implement GetReactionCountsAsync: aggregate reactions by type
    - Implement GetUserReactionAsync: retrieve user's current reaction for a post
    - _Requirements: 4.1, 4.2, 4.4, 4.5, 4.6, 19.3_


  - [x] 6.4 Create NotificationService
    - Implement CreateNotificationAsync: save notification to database, send via SignalR hub
    - Implement GetUserNotificationsAsync: retrieve notifications with filtering by read status
    - Implement MarkAsReadAsync: update notification read status
    - Implement GetUnreadCountAsync: count unread notifications for user
    - _Requirements: 19.1, 19.2, 19.3, 19.4, 19.5, 19.6_
  
  - [x] 6.5 Create SearchService
    - Implement SearchPostsAsync: query BlogPost titles and content in both languages, rank by relevance
    - Implement query sanitization to prevent SQL injection
    - Add timeout handling (5 seconds)
    - _Requirements: 14.1, 14.2, 14.6, 21.8_
  
  - [x] 6.6 Create AnalyticsService
    - Implement GetDashboardStatsAsync: aggregate counts for posts, comments, users, reactions
    - Implement GetTopPostsByViewsAsync, GetTopPostsByReactionsAsync, GetTopPostsByCommentsAsync
    - Implement GetRecentSharesAsync with platform breakdown
    - Implement GetPublicationTrendsAsync for chart data
    - _Requirements: 13.1, 13.3, 13.4, 13.5, 13.6, 13.7_
  
  - [x] 6.7 Create SeoService
    - Implement GenerateMetaTagsAsync: create title, description, keywords for blog posts
    - Implement GenerateOpenGraphTagsAsync: create OG tags for social sharing
    - Implement GenerateJsonLdAsync: create structured data for search engines
    - Implement GenerateSitemapAsync: build XML sitemap of all published posts
    - Implement GenerateRssFeedAsync: create RSS feed with latest posts
    - Include bilingual support with hreflang tags
    - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5, 11.6, 11.7, 11.8_
  
  - [x] 6.8 Create ViewCounterService
    - Implement IncrementViewCountAsync: check session uniqueness (24-hour window), increment count asynchronously
    - Use distributed cache or database to track viewed posts per session
    - _Requirements: 7.1, 7.2, 7.4, 7.5_
  
  - [x] 6.9 Create ShareTrackingService
    - Implement RecordShareAsync: save Share entity with platform and timestamp
    - Implement GetShareCountsAsync: aggregate shares by platform
    - Execute asynchronously without blocking
    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_
  
  - [x] 6.10 Write unit tests for application services
    - Test service methods with mocked repositories
    - Test validation logic and error handling
    - Test business rule enforcement (ownership, permissions)
    - Achieve 80% code coverage for Application layer
    - _Requirements: 26.6, 26.7_


- [x] 7. Implement SignalR hubs for real-time features
  - [x] 7.1 Create NotificationHub
    - Implement SendNotification method for targeted user messaging
    - Implement MarkAsRead method for notification updates
    - Configure user groups by UserId for efficient routing
    - _Requirements: 19.1, 19.2, 19.3, 19.5_
  
  - [x] 7.2 Create InteractionHub
    - Implement BroadcastReaction method for real-time reaction updates
    - Implement BroadcastComment method for real-time comment updates
    - Configure client groups by BlogPostId for efficient broadcasting
    - Ensure broadcasts complete within 500ms
    - _Requirements: 4.2, 4.6, 5.4_
  
  - [x] 7.3 Configure SignalR in Program.cs
    - Add SignalR services with JSON serialization options
    - Map hub endpoints: /hubs/notifications, /hubs/interactions
    - Configure CORS for SignalR connections
    - _Requirements: 4.2, 5.4, 19.1_

- [x] 8. Implement ASP.NET Core Identity and authentication
  - [x] 8.1 Configure Identity services
    - Add Identity services with custom User entity
    - Configure password requirements and lockout settings
    - Add role management for Author and Admin roles
    - _Requirements: 9.1, 9.2_
  
  - [x] 8.2 Create AccountController
    - Implement Register action: create user account, assign Author role by default
    - Implement Login action: authenticate user, create session
    - Implement Logout action: sign out user
    - Implement Profile action: display and update user profile
    - _Requirements: 9.1, 9.3, 9.8_
  
  - [x] 8.3 Implement authorization policies
    - Create AuthorPolicy requiring Author or Admin role
    - Create AdminPolicy requiring Admin role
    - Apply policies to controllers and actions
    - _Requirements: 9.2, 9.3, 9.5, 9.6, 9.7, 13.8_

- [ ] 9. Checkpoint - Ensure core infrastructure is working
  - Verify database migrations apply successfully
  - Verify Identity authentication works (register, login, logout)
  - Verify repository operations execute correctly
  - Verify SignalR hubs connect successfully
  - Ask the user if questions arise


- [x] 10. Implement Web layer - Controllers and ViewModels
  - [x] 10.1 Create HomeController
    - Implement Index action: fetch featured posts (max 3) and recent posts (10 per page), return HomeViewModel
    - Implement About action: return static about page
    - Implement Contact GET action: display contact form
    - Implement Contact POST action: validate form, send email notification, display confirmation
    - Implement Error action: handle 404 and 500 errors with custom pages
    - _Requirements: 22.1, 22.2, 22.3, 22.6, 24.1, 24.2, 29.1, 29.4, 29.5_
  
  - [x] 10.2 Create BlogController
    - Implement Details action: fetch post by slug, load comments, reactions, related posts, return BlogPostDetailViewModel
    - Implement ByTag action: fetch posts by tag slug, paginate results
    - Implement ByCategory action: fetch posts by category slug, paginate results
    - Implement BySeries action: fetch posts in series, display in order with prev/next navigation
    - Implement ByProject action: fetch posts in project, display in order
    - Implement Search action: execute search query, display results with highlighting
    - Increment view count asynchronously on Details action
    - _Requirements: 7.1, 14.1, 14.3, 14.4, 15.5, 15.6, 22.5, 28.5, 28.6, 28.8_
  
  - [x] 10.3 Create AuthorController with [Authorize(Policy = "AuthorPolicy")]
    - Implement Profile action: display author info, posts, stats (total posts, total views)
    - Implement Dashboard action: display author's own posts with edit/delete options
    - Implement Create GET action: display blog post creation form
    - Implement Create POST action: validate input, call BlogPostService.CreateBlogPostAsync, redirect to Details
    - Implement Edit GET action: verify ownership, display edit form
    - Implement Edit POST action: verify ownership, call BlogPostService.UpdateBlogPostAsync, redirect to Details
    - Implement Delete POST action: verify ownership, call BlogPostService.DeleteBlogPostAsync, redirect to Dashboard
    - _Requirements: 10.1, 10.3, 10.5, 10.6, 10.7, 23.1, 23.2, 23.3, 23.4_
  
  - [x] 10.4 Create AdminController with [Authorize(Policy = "AdminPolicy")]
    - Implement Dashboard action: fetch analytics via AnalyticsService, return AdminDashboardViewModel
    - Implement Posts action: display all posts with edit/delete options
    - Implement Comments action: display pending comments with approve/reject buttons
    - Implement ApproveComment POST action: call CommentService.ApproveCommentAsync
    - Implement RejectComment POST action: call CommentService.RejectCommentAsync
    - Implement Users action: display all users with role management
    - Implement Analytics action: display detailed charts and metrics
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 9.6, 9.7, 10.8, 13.1, 13.2, 13.3, 13.4, 13.5, 13.6, 13.7, 13.8_


  - [x] 10.5 Create API controllers for AJAX operations
    - Create CommentApiController: POST /api/comments (create comment), returns JSON
    - Create ReactionApiController: POST /api/reactions (add/update reaction), DELETE /api/reactions (remove reaction), returns JSON
    - Create ShareApiController: POST /api/shares (record share event), returns JSON
    - Create NotificationApiController: GET /api/notifications (get user notifications), POST /api/notifications/{id}/read (mark as read), returns JSON
    - Apply [Authorize] attribute to require authentication
    - _Requirements: 4.1, 5.1, 8.1, 9.4, 19.1, 19.5_
  
  - [x] 10.6 Create ViewModels
    - Create HomeViewModel with FeaturedPosts, RecentPosts, CurrentPage, TotalPages
    - Create BlogPostDetailViewModel with Post, Comments, ReactionCounts, UserReaction, RelatedPosts, PreviousInSeries, NextInSeries
    - Create AdminDashboardViewModel with all analytics properties
    - Create AuthorProfileViewModel with author info and posts
    - Create SearchResultsViewModel with query, results, highlighting
    - _Requirements: 13.1, 22.1, 22.2, 22.3, 23.1, 23.2, 28.8_

- [x] 11. Implement localization and RTL support
  - [x] 11.1 Configure localization services
    - Add localization services with supported cultures: en-US, ar-SA
    - Create resource files for UI strings: Resources.en-US.resx, Resources.ar-SA.resx
    - Configure RequestLocalizationOptions with cookie provider
    - _Requirements: 2.1, 2.6_
  
  - [x] 11.2 Create LocalizationMiddleware
    - Detect language from cookie or Accept-Language header
    - Set CurrentCulture and CurrentUICulture
    - Apply RTL layout class for Arabic (ar-SA)
    - Persist language selection in cookie
    - _Requirements: 2.1, 2.2, 2.3, 2.5_
  
  - [x] 11.3 Implement language switcher component
    - Create partial view with language toggle (EN/AR)
    - Update cookie on language change
    - Reload page to apply new language
    - _Requirements: 2.1, 2.5, 2.6_
  
  - [x] 11.4 Update views to display bilingual content
    - Display BlogPost content based on current culture (TitleEn/TitleAr, ContentEn/ContentAr)
    - Display Tag, Category, Series, Project names based on current culture
    - Apply RTL CSS classes when culture is ar-SA
    - _Requirements: 2.2, 2.3, 2.4, 15.7, 19.7, 23.5, 28.7_


- [x] 12. Implement theme persistence
  - [x] 12.1 Create ThemeMiddleware
    - Read theme preference from cookie (default: light)
    - Inject theme class into ViewBag for layout
    - _Requirements: 3.3, 3.4_
  
  - [x] 12.2 Create theme toggle component
    - Create partial view with dark/light toggle button
    - Use JavaScript to update cookie and apply theme class immediately
    - Persist theme preference in localStorage as backup
    - _Requirements: 3.1, 3.2, 3.5_
  
  - [x] 12.3 Implement theme CSS
    - Create CSS variables for light and dark themes
    - Apply theme classes to body element
    - Ensure smooth transitions (300ms) between themes
    - _Requirements: 3.2, 25.5_

- [x] 13. Implement SEO features
  - [x] 13.1 Create SEO partial views
    - Create _MetaTags.cshtml: render title, description, keywords, canonical URL
    - Create _OpenGraph.cshtml: render OG tags for social sharing
    - Create _JsonLd.cshtml: render structured data script
    - Include hreflang tags for bilingual content
    - _Requirements: 11.1, 11.2, 11.3, 11.7, 11.8_
  
  - [x] 13.2 Create SitemapController
    - Implement Index action: call SeoService.GenerateSitemapAsync, return XML content type
    - Cache sitemap for 1 hour
    - _Requirements: 11.4, 11.6_
  
  - [x] 13.3 Create RssController
    - Implement Index action: call SeoService.GenerateRssFeedAsync, return XML content type
    - Cache RSS feed for 15 minutes
    - _Requirements: 11.5_
  
  - [x] 13.4 Update blog post views to include SEO partials
    - Include _MetaTags, _OpenGraph, _JsonLd in Details view
    - Pass blog post data to SEO partials
    - _Requirements: 11.1, 11.2, 11.3, 11.7, 11.8_


- [ ] 14. Implement performance optimizations
  - [ ] 14.1 Configure output caching
    - Add output caching middleware
    - Apply caching to homepage (5 minutes), blog list pages (5 minutes)
    - Vary cache by culture and theme
    - _Requirements: 12.2_
  
  - [ ] 14.2 Configure response compression
    - Add response compression middleware with Gzip and Brotli
    - Enable for all responses
    - _Requirements: 12.3_
  
  - [ ] 14.3 Implement image lazy loading
    - Add loading="lazy" attribute to all images below the fold
    - Use responsive image srcset for different screen sizes
    - _Requirements: 12.4_
  
  - [ ] 14.4 Configure static file caching
    - Set cache headers for static assets (CSS, JS, images) to 1 year
    - Use versioned file names or query strings for cache busting
    - _Requirements: 12.6_
  
  - [ ] 14.5 Minify CSS and JavaScript
    - Configure bundling and minification for production
    - Use WebOptimizer or similar tool
    - _Requirements: 12.5_
  
  - [ ] 14.6 Optimize database queries
    - Ensure all queries use async/await
    - Add .AsNoTracking() for read-only queries
    - Use projection (Select) to load only required fields
    - Verify indexes are used via query execution plans
    - _Requirements: 12.7, 12.8_

- [ ] 15. Implement error handling and logging
  - [ ] 15.1 Configure Serilog
    - Add Serilog with console and file sinks
    - Configure structured logging with enrichers (timestamp, machine name, environment)
    - Set log levels: Debug for development, Information for production
    - _Requirements: 17.1, 17.7_
  
  - [ ] 15.2 Create ExceptionHandlingMiddleware
    - Catch unhandled exceptions
    - Log exception with full stack trace and request context
    - Return custom error page (500) to user
    - _Requirements: 17.2, 17.3_
  
  - [ ] 15.3 Implement logging throughout application
    - Log database queries with execution time
    - Log authentication attempts with success/failure status
    - Log SignalR connection events
    - Log slow requests (>1000ms)
    - _Requirements: 17.4, 17.5, 17.6_
  
  - [ ] 15.4 Create custom error pages
    - Create 404.cshtml for not found errors
    - Create 500.cshtml for server errors
    - Maintain localization and theme on error pages
    - Include navigation links to homepage
    - _Requirements: 24.1, 24.2, 24.3, 24.4, 24.5_


- [ ] 16. Implement health monitoring
  - [ ] 16.1 Configure health checks
    - Add health check services
    - Add database health check (verify connectivity)
    - Add SignalR health check (verify hub availability)
    - Set timeout to 3 seconds
    - _Requirements: 20.1, 20.2, 20.3, 20.6_
  
  - [ ] 16.2 Create health check endpoint
    - Map /health endpoint
    - Return HTTP 200 with "Healthy" when all checks pass
    - Return HTTP 503 with failure details when any check fails
    - _Requirements: 20.1, 20.4, 20.5_

- [ ] 17. Implement responsive design with Bootstrap 5
  - [ ] 17.1 Create responsive layout
    - Create _Layout.cshtml with Bootstrap 5 grid system
    - Implement responsive navigation with mobile hamburger menu
    - Ensure all interactive elements are touch-friendly (min 44x44px)
    - _Requirements: 16.1, 16.5_
  
  - [ ] 17.2 Implement responsive breakpoints
    - Mobile layout for width < 768px
    - Tablet layout for width 768px - 1024px
    - Desktop layout for width > 1024px
    - Test all pages at different breakpoints
    - _Requirements: 16.2, 16.3, 16.4_
  
  - [ ] 17.3 Optimize for mobile performance
    - Reduce image sizes for mobile devices
    - Minimize JavaScript execution on mobile
    - Test Lighthouse score on mobile (target ≥ 95)
    - _Requirements: 12.1, 16.6_

- [ ] 18. Implement animations and visual feedback
  - [ ] 18.1 Integrate Lottie animations
    - Add Lottie library
    - Create loading animation component for async operations
    - Display loading animation during comment submission, reaction updates
    - _Requirements: 25.1, 25.4_
  
  - [ ] 18.2 Integrate Lordicon animations
    - Add Lordicon library
    - Use animated icons for reactions, notifications, theme toggle
    - Animate reaction button on click
    - _Requirements: 25.2, 25.3_
  
  - [ ] 18.3 Implement accessibility for animations
    - Respect prefers-reduced-motion media query
    - Disable animations when user prefers reduced motion
    - _Requirements: 25.6_


- [ ] 19. Implement client-side JavaScript for real-time features
  - [ ] 19.1 Create SignalR client connections
    - Create signalr-client.js: establish connections to NotificationHub and InteractionHub
    - Handle connection lifecycle (connect, disconnect, reconnect)
    - Display connection status to user
    - _Requirements: 4.2, 5.4, 19.1_
  
  - [ ] 19.2 Implement real-time reaction updates
    - Listen for BroadcastReaction events from InteractionHub
    - Update reaction counts in UI without page reload
    - Animate reaction count changes
    - _Requirements: 4.2, 4.6_
  
  - [ ] 19.3 Implement real-time comment updates
    - Listen for BroadcastComment events from InteractionHub
    - Append new comments to comment list without page reload
    - Highlight newly added comments
    - _Requirements: 5.4_
  
  - [ ] 19.4 Implement real-time notifications
    - Listen for SendNotification events from NotificationHub
    - Display notification badge with unread count
    - Show notification toast/popup for new notifications
    - Update notification list in real-time
    - _Requirements: 19.1, 19.2, 19.3, 19.4_
  
  - [ ] 19.5 Implement AJAX operations
    - Create functions for posting comments via CommentApiController
    - Create functions for adding/removing reactions via ReactionApiController
    - Create functions for recording shares via ShareApiController
    - Create functions for marking notifications as read via NotificationApiController
    - Handle errors and display user-friendly messages
    - _Requirements: 4.1, 5.1, 8.1, 19.5_

- [ ] 20. Checkpoint - Ensure all features are integrated
  - Verify real-time reactions work across multiple browser tabs
  - Verify real-time comments appear immediately
  - Verify notifications are delivered in real-time
  - Verify localization switches correctly between EN/AR with RTL
  - Verify theme persistence works across sessions
  - Verify SEO meta tags are generated correctly
  - Ask the user if questions arise


- [ ] 21. Implement configuration management
  - [ ] 21.1 Create configuration files
    - Create appsettings.json with default configuration
    - Create appsettings.Development.json with development-specific settings
    - Create appsettings.Production.json with production-specific settings
    - _Requirements: 27.1, 27.2_
  
  - [ ] 21.2 Configure required settings
    - Database connection string
    - SignalR configuration (backplane for scale-out if needed)
    - Logging levels per environment
    - Email service settings for contact form
    - Authentication cookie settings
    - _Requirements: 27.6_
  
  - [ ] 21.3 Implement configuration validation
    - Validate required configuration keys on startup
    - Fail fast with clear error message if configuration is missing
    - Log missing configuration keys
    - _Requirements: 27.4, 27.5_
  
  - [ ] 21.4 Use environment variables for sensitive data
    - Load database connection string from environment variable in production
    - Load email service credentials from environment variables
    - Never commit sensitive data to source control
    - _Requirements: 27.3_

- [ ] 22. Implement database seeding for development
  - [ ] 22.1 Create DatabaseSeeder class
    - Check if data already exists before seeding
    - Only run in development environment
    - _Requirements: 30.5, 30.6_
  
  - [ ] 22.2 Seed sample data
    - Create sample users with Author and Admin roles (passwords: Test@123)
    - Create sample blog posts with bilingual content (at least 10 posts)
    - Create sample tags (Technology, Programming, Web Development, etc.)
    - Create sample categories (Tutorials, News, Opinion, etc.)
    - Create sample series and projects
    - Create sample comments and reactions
    - _Requirements: 30.1, 30.2, 30.3, 30.4_
  
  - [ ] 22.3 Call seeder on application startup
    - Invoke DatabaseSeeder in Program.cs after migration
    - Only in development environment
    - _Requirements: 30.1, 30.5, 30.6_


- [ ] 23. Implement contact form with email notification
  - [ ] 23.1 Create email service interface and implementation
    - Define IEmailService with SendEmailAsync method
    - Implement EmailService using SMTP or SendGrid
    - Configure email settings from appsettings.json
    - _Requirements: 29.4_
  
  - [ ] 23.2 Implement contact form submission
    - Validate contact form input using ContactFormValidator
    - Send email to administrators via EmailService
    - Display confirmation message to visitor
    - _Requirements: 29.1, 29.2, 29.3, 29.4, 29.5_
  
  - [ ] 23.3 Implement rate limiting for contact form
    - Track submissions by IP address
    - Limit to 3 submissions per hour per IP
    - Display error message when limit exceeded
    - _Requirements: 29.6_
  
  - [ ] 23.4 Localize contact form
    - Display form labels and messages in current language
    - Validate input based on current culture
    - _Requirements: 29.7_

- [ ] 24. Implement tag, category, series, and project management
  - [ ] 24.1 Create management controllers for Admin
    - Create TagController with CRUD actions (Create, Edit, Delete, List)
    - Create CategoryController with CRUD actions
    - Create SeriesController with CRUD actions
    - Create ProjectController with CRUD actions
    - Apply [Authorize(Policy = "AdminPolicy")] to all actions
    - _Requirements: 15.1, 15.2, 28.1, 28.2_
  
  - [ ] 24.2 Create management views
    - Create forms for creating/editing tags, categories, series, projects with bilingual fields
    - Create list views showing all items with edit/delete buttons
    - _Requirements: 15.1, 15.2, 28.1, 28.2_
  
  - [ ] 24.3 Implement tag/category assignment in blog post editor
    - Add multi-select dropdowns for tags and categories in Create/Edit post forms
    - Save associations when post is saved
    - _Requirements: 10.5, 15.3, 15.4_
  
  - [ ] 24.4 Implement series/project assignment in blog post editor
    - Add dropdowns for series and project selection in Create/Edit post forms
    - Save associations when post is saved
    - _Requirements: 28.3, 28.4_


- [ ] 25. Implement comprehensive unit tests
  - [ ] 25.1 Write unit tests for Domain entities
    - Test entity creation and property validation
    - Test navigation property relationships
    - _Requirements: 26.6_
  
  - [ ] 25.2 Write unit tests for Application services
    - Test BlogPostService methods with mocked repositories
    - Test CommentService methods including approval workflow
    - Test ReactionService methods including replacement logic
    - Test NotificationService methods
    - Test SearchService with various query inputs
    - Test AnalyticsService aggregation methods
    - Test SeoService metadata generation
    - Achieve 80% code coverage for Application layer
    - _Requirements: 26.6, 26.7_
  
  - [ ] 25.3 Write unit tests for validators
    - Test BlogPostValidator with valid and invalid inputs
    - Test CommentValidator with edge cases
    - Test ContactFormValidator with various email formats
    - _Requirements: 21.7, 26.6_
  
  - [ ] 25.4 Write integration tests for repositories
    - Test repository operations with in-memory database
    - Test complex queries and eager loading
    - Test cascade delete behaviors
    - _Requirements: 26.6_
  
  - [ ] 25.5 Write integration tests for controllers
    - Test controller actions with mocked services
    - Test authorization policies
    - Test model binding and validation
    - _Requirements: 26.6_

- [ ] 26. Implement CI/CD with GitHub Actions
  - [ ] 26.1 Create GitHub Actions workflow file
    - Create .github/workflows/ci-cd.yml
    - Define triggers: push to main branch, pull requests
    - _Requirements: 26.1, 26.2_
  
  - [ ] 26.2 Configure build and test jobs
    - Set up .NET 9 SDK
    - Restore NuGet packages
    - Build solution
    - Run all unit tests
    - Fail workflow if any test fails
    - _Requirements: 26.2, 26.3, 26.4_
  
  - [ ] 26.3 Configure deployment job
    - Deploy to Azure App Service when tests pass
    - Use Azure credentials from GitHub secrets
    - Deploy only from main branch
    - _Requirements: 26.5_
  
  - [ ] 26.4 Configure Azure App Service
    - Create App Service in Azure portal
    - Configure connection string in Application Settings
    - Configure environment variables for production
    - Enable Application Insights for monitoring
    - _Requirements: 27.3_


- [ ] 27. Final integration and testing
  - [ ] 27.1 Perform end-to-end testing
    - Test complete user flows: registration, login, create post, comment, react
    - Test admin flows: approve comments, manage posts, view analytics
    - Test localization switching with RTL layout
    - Test theme persistence across sessions
    - Test real-time features across multiple browser tabs
    - _Requirements: All_
  
  - [ ] 27.2 Run Lighthouse audits
    - Test homepage on desktop and mobile
    - Test blog post detail page on desktop and mobile
    - Verify performance score ≥ 95
    - Verify accessibility score ≥ 95
    - Verify SEO score ≥ 95
    - Fix any issues identified
    - _Requirements: 12.1, 16.6_
  
  - [ ] 27.3 Test security
    - Verify XSS protection (input sanitization)
    - Verify CSRF protection (anti-forgery tokens)
    - Verify SQL injection protection (parameterized queries)
    - Verify authentication and authorization work correctly
    - Test rate limiting on contact form
    - _Requirements: 21.8, 29.6_
  
  - [ ] 27.4 Optimize and finalize
    - Review and optimize slow database queries
    - Review and optimize large bundle sizes
    - Verify all error handling works correctly
    - Verify all logging is in place
    - Verify health checks work correctly
    - _Requirements: 12.7, 12.8, 17.1, 17.2, 17.3, 20.1, 20.2, 20.3_

- [ ] 28. Final checkpoint - Production readiness
  - Verify all tests pass
  - Verify Lighthouse scores meet targets (≥ 95)
  - Verify application runs correctly in production configuration
  - Verify database migrations apply successfully
  - Verify CI/CD pipeline deploys successfully
  - Verify health check endpoint returns healthy status
  - Ask the user if questions arise

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP delivery
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation throughout implementation
- The implementation follows Clean Architecture with clear separation of concerns
- All database operations use async/await for optimal performance
- Real-time features use SignalR for efficient bidirectional communication
- The platform is designed for Azure deployment with CI/CD automation
- Comprehensive testing ensures 80% code coverage for Application layer
- SEO optimization includes meta tags, OpenGraph, JSON-LD, sitemap, and RSS
- Bilingual support with RTL layout provides excellent user experience for both English and Arabic audiences
