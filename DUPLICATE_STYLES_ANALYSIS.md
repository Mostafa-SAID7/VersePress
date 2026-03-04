# Duplicate Styles Analysis

## Duplicates Found Between site.css and components.css

### 1. Form Control Styles (DUPLICATE)
**Location:** Both files
**Styles:**
- `.form-control, .form-select` - background, border, color, transition
- `.form-control:focus, .form-select:focus` - focus states
- `.form-check-input:focus` - checkbox/radio focus

**Action:** Keep in `site.css` (theme-related), remove detailed form styles from components.css or vice versa

### 2. Toast Notification Styles (DUPLICATE)
**Location:** Both files
**In site.css:**
- `.toast-notification` - Old toast system
- `.toast-success`, `.toast-error`, `.toast-info`

**In components.css:**
- `.toast-item` - New toaster component
- `.toast-icon`, `.toast-content`, `.toast-title`, `.toast-message`

**Action:** Remove old `.toast-notification` from site.css, keep new `.toast-item` in components.css

### 3. Mobile Form Optimizations (DUPLICATE)
**Location:** Both files
**Styles:**
- `font-size: 16px` for form controls on mobile

**Action:** Keep in components.css (component-specific)

---

## Recommendation

### Keep in site.css (Theme & Global):
- CSS variables (theme colors)
- Global element styles (body, navbar, footer, card)
- Theme transitions
- RTL support (global)
- Responsive breakpoints (global)
- Touch-friendly targets (global)

### Keep in components.css (Component-Specific):
- All form input component styles
- Toaster component styles (new system)
- Skeleton loader styles
- Progress bar styles
- Badge styles
- Tooltip styles
- Component-specific RTL
- Component-specific mobile responsive

### Remove from site.css:
- Old `.toast-notification` styles (replaced by new toaster)
- Duplicate form control focus styles (keep theme colors only)
- Duplicate mobile form font-size (in components.css)

