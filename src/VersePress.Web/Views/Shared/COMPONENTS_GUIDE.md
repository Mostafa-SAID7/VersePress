# Reusable View Components Guide

This guide explains how to use the reusable view components in VersePress.

## Available Components

### 1. _BlogPostCard.cshtml
Displays a blog post in a card format with image, title, excerpt, and metadata.

**Usage:**
```cshtml
@foreach (var post in Model.Posts)
{
    <div class="col-md-4 mb-4">
        <partial name="_BlogPostCard" model="post" />
    </div>
}
```

**Features:**
- Featured badge for featured posts
- Fallback gradient background if no image
- Hover animation
- View count, comment count, and reaction count
- Responsive design

---

### 2. _StatCard.cshtml
Displays statistics in a colorful card with icon.

**Usage:**
```cshtml
<div class="col-md-3 mb-3">
    <partial name="_StatCard" model='("file-text", "Total Posts", Model.TotalPosts.ToString(), "bg-primary", "/admin/posts", "View All")' />
</div>
```

**Parameters:**
- Icon: Bootstrap icon name (without 'bi-' prefix)
- Title: Card title
- Value: Statistic value to display
- Color: bg-primary, bg-success, bg-info, bg-warning, bg-danger
- Link: Optional action link
- LinkText: Optional link text

---

### 3. _AlertMessage.cshtml
Displays alert messages with icons.

**Usage:**
```cshtml
@if (TempData["SuccessMessage"] != null)
{
    <partial name="_AlertMessage" model='("success", TempData["SuccessMessage"].ToString(), null)' />
}
```

**Parameters:**
- Type: success, error, warning, info
- Message: Alert message text
- Icon: Optional custom icon (uses default based on type if null)

---

### 4. _Pagination.cshtml
Displays pagination controls with previous/next and page numbers.

**Usage:**
```cshtml
<partial name="_Pagination" model='(Model.CurrentPage, Model.TotalPages, "Index", "Blog", new { category = "tech" })' />
```

**Parameters:**
- CurrentPage: Current page number
- TotalPages: Total number of pages
- Action: Controller action name
- Controller: Controller name
- RouteValues: Additional route parameters (anonymous object)

**Features:**
- Shows ellipsis for large page ranges
- Disabled state for first/last pages
- Preserves query parameters

---

### 5. _EmptyState.cshtml
Displays empty state with icon and optional action button.

**Usage:**
```cshtml
<partial name="_EmptyState" model='("inbox", "No Posts Yet", "You haven't created any posts. Start writing!", "Create Post", "/author/create")' />
```

**Parameters:**
- Icon: Bootstrap icon name
- Title: Empty state title
- Message: Description message
- ActionText: Button text (optional)
- ActionLink: Button link (optional)

---

### 6. _LoadingSpinner.cshtml
Displays loading spinner with message.

**Usage:**
```cshtml
<partial name="_LoadingSpinner" model="Loading posts..." />
```

**Parameter:**
- Message: Loading message (optional, defaults to "Loading...")

---

### 7. _Breadcrumb.cshtml
Displays breadcrumb navigation.

**Usage:**
```cshtml
@{
    var breadcrumbs = new List<(string, string)>
    {
        ("Blog", "/blog"),
        ("Technology", "/blog/category/technology"),
        ("Current Post", "")
    };
}
<partial name="_Breadcrumb" model="breadcrumbs" />
```

**Parameter:**
- List of tuples: (Text, Link)
- Last item is automatically marked as active

---

### 8. _PageHeader.cshtml
Displays page header with title, subtitle, and optional action button.

**Usage:**
```cshtml
<partial name="_PageHeader" model='("My Dashboard", "Manage your content and view statistics", "speedometer2", "New Post", "/author/create")' />
```

**Parameters:**
- Title: Page title
- Subtitle: Optional subtitle
- Icon: Optional Bootstrap icon
- ActionText: Optional button text
- ActionLink: Optional button link

---

### 9. _FormCard.cshtml
Wraps form sections in a styled card with header.

**Usage:**
```cshtml
<partial name="_FormCard" model='("English Content", "translate", "bg-primary")'>
    <div class="mb-3">
        <label>Title</label>
        <input type="text" class="form-control" />
    </div>
</partial>
```

**Parameters:**
- Title: Card header title
- Icon: Optional Bootstrap icon
- ColorClass: Header color (bg-primary, bg-success, etc.)

---

### 10. _ActionButtons.cshtml
Displays form action buttons (submit/cancel) with consistent styling.

**Usage:**
```cshtml
<partial name="_ActionButtons" model='("Save Changes", null, "Cancel", "/author/dashboard", "lg")' />
```

**Parameters:**
- PrimaryText: Submit button text
- PrimaryAction: Optional form action URL
- SecondaryText: Cancel button text
- SecondaryAction: Cancel button link
- Size: Button size (sm, md, lg)

---

## Best Practices

1. **Consistency**: Always use these components instead of duplicating HTML
2. **Customization**: Components include inline styles that can be overridden
3. **Accessibility**: All components include proper ARIA attributes
4. **Responsive**: All components are mobile-friendly
5. **Performance**: Components use lazy loading where appropriate

## Example: Complete Page Using Components

```cshtml
@model BlogListViewModel

@* Page Header *@
<partial name="_PageHeader" model='("Blog Posts", "Explore our latest articles", "book", "New Post", "/author/create")' />

@* Breadcrumb *@
<partial name="_Breadcrumb" model='new List<(string, string)> { ("Blog", "") }' />

@* Success Message *@
@if (TempData["SuccessMessage"] != null)
{
    <partial name="_AlertMessage" model='("success", TempData["SuccessMessage"].ToString(), null)' />
}

@* Blog Posts Grid *@
<div class="row">
    @if (Model.Posts.Any())
    {
        @foreach (var post in Model.Posts)
        {
            <div class="col-md-4 mb-4">
                <partial name="_BlogPostCard" model="post" />
            </div>
        }
    }
    else
    {
        <div class="col-12">
            <partial name="_EmptyState" model='("inbox", "No Posts Found", "Check back later for new content!", "", "")' />
        </div>
    }
</div>

@* Pagination *@
<partial name="_Pagination" model='(Model.CurrentPage, Model.TotalPages, "Index", "Blog", null)' />
```

---

## Styling

All components use:
- Bootstrap 5 classes
- Bootstrap Icons
- Custom CSS for animations and transitions
- CSS variables for theming support
- RTL-ready layouts

## Updates

When updating components:
1. Update this guide
2. Test in both light and dark themes
3. Test in both English and Arabic (RTL)
4. Verify accessibility with screen readers
5. Test on mobile devices
