# Components Quick Reference Card

## 🎨 UI Components

```cshtml
@* Toaster - Include in _Layout.cshtml *@
<partial name="Components/_Toaster" />
@* JavaScript: Toaster.success('Message'), Toaster.error('Message') *@

@* Skeleton Loader *@
<partial name="Components/_Skeleton" model='("card", 3, null)' />
<partial name="Components/_Skeleton" model='("post", 1, null)' />
<partial name="Components/_Skeleton" model='("list", 5, null)' />

@* Progress Bar *@
<partial name="Components/_ProgressBar" model='(75, "primary", true, false)' />

@* Badge *@
<partial name="Components/_Badge" model='("New", "success", "star-fill")' />

@* Tooltip *@
<partial name="Components/_Tooltip" model='("<i class=\"bi bi-info\"></i>", "Help text", "top")' />

@* Alert *@
<partial name="Components/_AlertMessage" model='("success", "Saved!", null)' />

@* Empty State *@
<partial name="Components/_EmptyState" model='("inbox", "No Items", "Description", "Action", "/link")' />

@* Blog Card *@
<partial name="Components/_BlogPostCard" model="post" />

@* Stat Card *@
<partial name="Components/_StatCard" model='("file-text", "Total", "100", "bg-primary", "/link", "View")' />

@* Page Header *@
<partial name="Components/_PageHeader" model='("Title", "Subtitle", "icon", "Action", "/link")' />
```

---

## 📝 Form Input Components

```cshtml
@* Text Input (text, email, password, url, tel, number) *@
<partial name="Components/_TextInput" model='("Name", "Label", value, "placeholder", required, "text")' />

@* TextArea *@
<partial name="Components/_TextArea" model='("Content", "Label", value, "placeholder", required, 6)' />

@* Select Dropdown *@
@{
    var options = new List<(string, string)> { ("1", "Option 1"), ("2", "Option 2") };
}
<partial name="Components/_Select" model='("Field", "Label", options, selectedValue, required)' />

@* Checkbox *@
<partial name="Components/_Checkbox" model='("Field", "Label", isChecked, "Help text")' />

@* Switch Toggle *@
<partial name="Components/_Switch" model='("Field", "Label", isChecked, "Help text")' />

@* Radio Group *@
@{
    var radioOptions = new List<(string, string)> { ("1", "Option 1"), ("2", "Option 2") };
}
<partial name="Components/_Radio" model='("Field", "Label", radioOptions, selectedValue, required)' />

@* Date Picker *@
<partial name="Components/_DatePicker" model='("Date", "Label", dateValue, required, "2024-01-01", "2026-12-31")' />

@* File Upload *@
<partial name="Components/_FileUpload" model='("File", "Label", "image/*", required, showPreview)' />
```

---

## 🧭 Navigation Components

```cshtml
@* Breadcrumb *@
@{
    var breadcrumbs = new List<(string, string)> { ("Home", "/"), ("Blog", "/blog"), ("Post", "") };
}
<partial name="Components/_Breadcrumb" model="breadcrumbs" />

@* Pagination *@
<partial name="Components/_Pagination" model='(currentPage, totalPages, "Action", "Controller", routeValues)' />
```

---

## 📦 Form Container Components

```cshtml
@* Form Card *@
<partial name="Components/_FormCard" model='("Title", "icon", "bg-primary")'>
    @* Form inputs here *@
</partial>

@* Action Buttons *@
<partial name="Components/_ActionButtons" model='("Save", null, "Cancel", "/back", "lg")' />
```

---

## 🎭 Interactive Components

```cshtml
@* Modal *@
<partial name="Components/_Modal" model='("modalId", "Title", "md")'>
    @* Modal content *@
</partial>
@* JavaScript: new bootstrap.Modal(document.getElementById('modalId')).show() *@
```

---

## 💡 Quick Tips

### Toaster Usage
```javascript
// Success
Toaster.success('Post published successfully!');

// Error
Toaster.error('Failed to save changes');

// Warning
Toaster.warning('Please review your input');

// Info
Toaster.info('New notification received');

// Custom
Toaster.show('Message', 'info', 'Custom Title', 5000);
```

### Skeleton Types
- `card` - Blog post card
- `post` - Detailed post
- `list` - List item
- `text` - Text line
- `avatar` - Circle avatar
- `button` - Button shape
- `table` - Table row

### Input Types
- `text` - Regular text
- `email` - Email with validation
- `password` - Password (hidden)
- `url` - URL with validation
- `tel` - Telephone number
- `number` - Numeric input

### Color Variants
- `primary` - Blue
- `secondary` - Gray
- `success` - Green
- `danger` - Red
- `warning` - Yellow
- `info` - Cyan
- `light` - Light gray
- `dark` - Dark gray

---

## 📱 All Components Are:
- ✅ Mobile responsive
- ✅ Theme-aware (light/dark)
- ✅ RTL compatible (Arabic)
- ✅ Accessible (ARIA labels)
- ✅ Validated (form inputs)
- ✅ Touch-friendly (44px+ targets)

---

## 📚 Full Documentation
See `COMPONENTS_GUIDE.md` for detailed usage and examples.

