# Project Structure

## Overview

VersePress follows Clean Architecture principles with clear separation of concerns across four layers.

## Solution Structure

```
VersePress/
├── src/
│   ├── VersePress.Domain/           # Domain Layer
│   ├── VersePress.Application/      # Application Layer
│   ├── VersePress.Infrastructure/   # Infrastructure Layer
│   └── VersePress.Web/              # Web Layer (Presentation)
├── tests/
│   └── VersePress.Tests/            # Unit & Integration Tests
├── docs/                            # Documentation
├── .github/workflows/               # CI/CD Pipelines
├── .kiro/specs/                     # Specifications
└── screenshots/                     # UI Screenshots

```

## Layer Details

### 1. Domain Layer (`VersePress.Domain`)

**Purpose**: Core business logic and entities

**Contents**:
- `Entities/`: Domain entities (BlogPost, Comment, Reaction, etc.)
- `Enums/`: Enumerations (ReactionType, NotificationType, Platform)
- `Interfaces/`: Repository contracts

**Dependencies**: None (pure domain logic)

**Key Files**:
- `Entities/BaseEntity.cs` - Base class with soft delete
- `Entities/BlogPost.cs` - Main content entity
- `Entities/Comment.cs` - Comment system
- `Entities/Reaction.cs` - Reaction system
- `Enums/ReactionType.cs` - Reaction types

### 2. Application Layer (`VersePress.Application`)

**Purpose**: Application business logic and use cases

**Contents**:
- `Services/`: Business logic services
- `Interfaces/`: Service contracts
- `DTOs/`: Data Transfer Objects
- `Commands/`: Command DTOs for operations
- `Validators/`: FluentValidation validators

**Dependencies**: Domain Layer only

**Key Services**:
- `BlogPostService` - Blog post management
- `CommentService` - Comment management
- `ReactionService` - Reaction management
- `NotificationService` - Notification system
- `SearchService` - Search functionality
- `AnalyticsService` - Analytics and statistics
- `SeoService` - SEO optimization
- `ViewCounterService` - View tracking
- `ShareTrackingService` - Share tracking

### 3. Infrastructure Layer (`VersePress.Infrastructure`)

**Purpose**: External concerns and data access

**Contents**:
- `Data/`: DbContext and configurations
- `Repositories/`: Repository implementations
- `Services/`: External service implementations
- `HealthChecks/`: Health check implementations
- `Migrations/`: EF Core migrations

**Dependencies**: Domain, Application layers

**Key Components**:
- `ApplicationDbContext` - EF Core DbContext
- `UnitOfWork` - Transaction coordinator
- `Repository<T>` - Generic repository
- `BlogPostRepository` - Specialized repository
- `EmailService` - Email functionality
- `DatabaseSeeder` - Sample data seeding

### 4. Web Layer (`VersePress.Web`)

**Purpose**: User interface and HTTP concerns

**Contents**:
- `Controllers/`: MVC controllers
- `Views/`: Razor views
- `Models/`: ViewModels
- `wwwroot/`: Static files (CSS, JS, images)
- `Middleware/`: Custom middleware
- `Hubs/`: SignalR hubs
- `Resources/`: Localization resources
- `Configuration/`: Configuration classes

**Dependencies**: All other layers

**Key Controllers**:
- `HomeController` - Homepage and contact
- `BlogController` - Blog post display
- `AuthorController` - Author management
- `AdminController` - Admin dashboard
- `AccountController` - Authentication
- API Controllers for AJAX operations

**SignalR Hubs**:
- `NotificationHub` - Real-time notifications
- `InteractionHub` - Real-time reactions/comments

**Middleware**:
- `ExceptionHandlingMiddleware` - Global error handling
- `LocalizationMiddleware` - Language switching
- `ThemeMiddleware` - Theme persistence
- `ContactFormRateLimitMiddleware` - Rate limiting
- `PerformanceMonitoringMiddleware` - Performance tracking

## Key Directories

### `/wwwroot` (Static Files)

```
wwwroot/
├── css/
│   └── site.css                    # Main stylesheet
├── js/
│   ├── site.js                     # Main JavaScript
│   ├── signalr-client.js          # SignalR connections
│   ├── reactions.js                # Reaction system
│   ├── comments.js                 # Comment system
│   ├── notifications.js            # Notification system
│   ├── shares.js                   # Share tracking
│   ├── accessibility.js            # Accessibility features
│   └── lordicon.js                 # Icon animations
├── animations/
│   └── loading.json                # Lottie animations
└── lib/                            # Third-party libraries
```

### `/Views` (Razor Views)

```
Views/
├── Shared/
│   ├── _Layout.cshtml              # Main layout
│   ├── _LanguageSwitcher.cshtml    # Language toggle
│   ├── _ThemeToggle.cshtml         # Theme toggle
│   ├── _MetaTags.cshtml            # SEO meta tags
│   ├── _OpenGraph.cshtml           # OpenGraph tags
│   └── _JsonLd.cshtml              # Structured data
├── Home/
│   ├── Index.cshtml                # Homepage
│   ├── About.cshtml                # About page
│   └── Contact.cshtml              # Contact form
├── Blog/
│   ├── Details.cshtml              # Blog post detail
│   ├── ByTag.cshtml                # Posts by tag
│   ├── ByCategory.cshtml           # Posts by category
│   └── Search.cshtml               # Search results
├── Author/
│   ├── Dashboard.cshtml            # Author dashboard
│   ├── Create.cshtml               # Create post
│   └── Edit.cshtml                 # Edit post
└── Admin/
    ├── Dashboard.cshtml            # Admin dashboard
    ├── Posts.cshtml                # Manage posts
    └── Comments.cshtml             # Moderate comments
```

## Configuration Files

- `appsettings.json` - Default configuration
- `appsettings.Development.json` - Development settings
- `appsettings.Production.json` - Production settings
- `.env.example` - Environment variable template
- `Program.cs` - Application entry point and configuration

## Database

- **Provider**: SQL Server
- **ORM**: Entity Framework Core
- **Migrations**: Code-first approach
- **Seeding**: Automatic in development

## Testing Structure

```
tests/VersePress.Tests/
├── Services/                       # Service tests
│   ├── BlogPostServiceTests.cs
│   └── CommentServiceTests.cs
├── Repositories/                   # Repository tests
│   └── BlogPostRepositoryTests.cs
├── Validators/                     # Validator tests
└── Controllers/                    # Controller tests
```

## Documentation

```
docs/
├── CHANGELOG.md                    # Version history
├── CODE_OF_CONDUCT.md              # Community guidelines
├── CONTRIBUTING.md                 # Contribution guide
├── DEPLOYMENT.md                   # Deployment instructions
├── ERD.md                          # Database schema
├── FEATURES.md                     # Feature list
├── PROJECT_SETUP.md                # Setup guide
├── SECURITY.md                     # Security policy
├── STRUCTURE.md                    # This file
├── TECHNOLOGIES.md                 # Tech stack details
└── USE_CASES.md                    # Use case scenarios
```

## Dependency Flow

```
┌─────────────────────────────────────────┐
│           Web Layer (MVC)               │
│  Controllers, Views, ViewModels         │
└─────────────────┬───────────────────────┘
                  │ depends on
┌─────────────────▼───────────────────────┐
│        Application Layer                │
│  Services, DTOs, Interfaces             │
└─────────────────┬───────────────────────┘
                  │ depends on
┌─────────────────▼───────────────────────┐
│      Infrastructure Layer               │
│  Repositories, DbContext, External APIs │
└─────────────────┬───────────────────────┘
                  │ depends on
┌─────────────────▼───────────────────────┐
│          Domain Layer                   │
│  Entities, Enums, Domain Logic          │
└─────────────────────────────────────────┘
```

## Design Patterns Used

- **Repository Pattern**: Data access abstraction
- **Unit of Work Pattern**: Transaction management
- **Dependency Injection**: Loose coupling
- **CQRS**: Command/Query separation
- **Factory Pattern**: Object creation
- **Strategy Pattern**: Algorithm selection
- **Observer Pattern**: Event handling (SignalR)
- **Middleware Pattern**: Request pipeline
- **Soft Delete Pattern**: Data preservation

## Naming Conventions

- **Namespaces**: `VersePress.{Layer}.{Feature}`
- **Classes**: PascalCase
- **Interfaces**: IPascalCase
- **Methods**: PascalCase
- **Variables**: camelCase
- **Constants**: UPPER_SNAKE_CASE
- **Private fields**: _camelCase

## Code Organization Principles

1. **Single Responsibility**: Each class has one reason to change
2. **Open/Closed**: Open for extension, closed for modification
3. **Liskov Substitution**: Derived classes are substitutable
4. **Interface Segregation**: Many specific interfaces over one general
5. **Dependency Inversion**: Depend on abstractions, not concretions
