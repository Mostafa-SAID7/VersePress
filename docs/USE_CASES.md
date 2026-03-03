# Use Cases

## User Personas

### 1. Blog Visitor (Unauthenticated)
**Goal**: Read blog content and explore the platform

**Capabilities**:
- Browse blog posts
- Read full articles
- View posts by tag, category, series, or project
- Search for content
- Switch between English and Arabic
- Toggle dark/light theme
- View author profiles
- Submit contact form

**Typical Flow**:
1. Visit homepage
2. Browse featured and recent posts
3. Click on interesting post
4. Read content in preferred language
5. Explore related posts
6. Submit contact form if needed

### 2. Registered User (Authenticated)
**Goal**: Engage with content through comments and reactions

**Capabilities**:
- All visitor capabilities
- Add reactions to posts
- Comment on posts
- Reply to comments
- Receive real-time notifications
- View notification history

**Typical Flow**:
1. Login to account
2. Browse and read posts
3. React to posts (like, love, celebrate, etc.)
4. Leave comments
5. Reply to other comments
6. Receive notifications for interactions

### 3. Author
**Goal**: Create and manage blog content

**Capabilities**:
- All registered user capabilities
- Create new blog posts
- Edit own posts
- Delete own posts
- Manage post metadata (tags, categories, series, projects)
- View post statistics (views, reactions, comments)
- Access author dashboard
- Manage profile

**Typical Flow**:
1. Login to author account
2. Navigate to author dashboard
3. Create new post with bilingual content
4. Add tags and categories
5. Assign to series or project
6. Publish post
7. Monitor engagement (views, reactions, comments)
8. Respond to comments

### 4. Administrator
**Goal**: Manage the entire platform

**Capabilities**:
- All author capabilities
- Moderate all comments (approve/reject)
- Manage all posts (edit/delete any post)
- Manage users and roles
- View analytics dashboard
- Manage tags, categories, series, projects
- Access admin dashboard

**Typical Flow**:
1. Login to admin account
2. Review admin dashboard analytics
3. Moderate pending comments
4. Manage user accounts
5. Review platform statistics
6. Manage organizational elements

## Detailed Use Cases

### UC1: Reading a Blog Post

**Actor**: Blog Visitor

**Preconditions**: None

**Main Flow**:
1. User navigates to homepage
2. System displays featured and recent posts
3. User clicks on a post
4. System displays full post with:
   - Title and content in selected language
   - Author information
   - Publication date
   - View count
   - Reaction counts
   - Comments section
   - Related posts
5. User reads the content
6. System increments view count (once per session)

**Postconditions**: View count updated

### UC2: Adding a Reaction

**Actor**: Registered User

**Preconditions**: User is logged in

**Main Flow**:
1. User views a blog post
2. User clicks on a reaction button (like, love, etc.)
3. System validates authentication
4. System saves reaction to database
5. System broadcasts update via SignalR
6. System updates reaction count in real-time
7. System sends notification to post author

**Alternative Flow**:
- If user already reacted, system replaces previous reaction
- If user clicks same reaction, system removes it

**Postconditions**: Reaction saved, author notified

### UC3: Commenting on a Post

**Actor**: Registered User

**Preconditions**: User is logged in

**Main Flow**:
1. User views a blog post
2. User types comment in text area
3. User clicks submit
4. System validates input (1-2000 characters)
5. System saves comment with IsApproved=false
6. System displays success message
7. System sends notification to post author
8. Admin receives notification for moderation

**Alternative Flow**:
- If validation fails, system displays error message
- If rate limit exceeded, system displays warning

**Postconditions**: Comment saved, pending approval

### UC4: Creating a Blog Post

**Actor**: Author

**Preconditions**: User is logged in with Author role

**Main Flow**:
1. Author navigates to dashboard
2. Author clicks "Create New Post"
3. System displays post creation form
4. Author enters:
   - English title and content
   - Arabic title and content
   - Excerpt (optional)
   - Featured image URL
   - Tags and categories
   - Series or project (optional)
5. Author clicks "Publish"
6. System validates input
7. System generates unique slug
8. System saves post to database
9. System redirects to post detail page

**Alternative Flow**:
- If validation fails, system displays errors
- If slug exists, system generates alternative

**Postconditions**: Post published and visible

### UC5: Moderating Comments

**Actor**: Administrator

**Preconditions**: User is logged in with Admin role

**Main Flow**:
1. Admin navigates to admin dashboard
2. System displays pending comments count
3. Admin clicks "Moderate Comments"
4. System displays list of pending comments
5. Admin reviews comment
6. Admin clicks "Approve" or "Reject"
7. If approved:
   - System sets IsApproved=true
   - System broadcasts comment via SignalR
   - System sends notification to commenter
8. If rejected:
   - System deletes comment
   - No notification sent

**Postconditions**: Comment approved or rejected

### UC6: Switching Language

**Actor**: Any User

**Preconditions**: None

**Main Flow**:
1. User clicks language switcher
2. User selects desired language (EN/AR)
3. System updates cookie
4. System reloads page
5. System displays content in selected language
6. If Arabic selected, system applies RTL layout

**Postconditions**: Language preference saved

### UC7: Toggling Theme

**Actor**: Any User

**Preconditions**: None

**Main Flow**:
1. User clicks theme toggle button
2. System switches between dark/light theme
3. System updates localStorage
4. System updates cookie
5. System applies theme CSS immediately
6. System animates transition (300ms)

**Postconditions**: Theme preference saved

### UC8: Searching for Content

**Actor**: Any User

**Preconditions**: None

**Main Flow**:
1. User enters search query
2. User clicks search button
3. System searches titles and content in both languages
4. System ranks results by relevance
5. System displays results with:
   - Title
   - Excerpt
   - Publication date
   - Highlighted search terms
6. User clicks on result
7. System navigates to post

**Alternative Flow**:
- If no results, system displays "No results found"
- If query too short, system displays validation message

**Postconditions**: Search results displayed

### UC9: Submitting Contact Form

**Actor**: Any User

**Preconditions**: None (rate limit: 3/hour per IP)

**Main Flow**:
1. User navigates to contact page
2. User fills form:
   - Name (2-100 characters)
   - Email (valid format)
   - Subject (5-200 characters)
   - Message (10-5000 characters)
3. User clicks submit
4. System validates input
5. System checks rate limit
6. System sends email to admin
7. System displays success message
8. System redirects to homepage

**Alternative Flow**:
- If validation fails, system displays errors
- If rate limit exceeded, system displays 429 error

**Postconditions**: Email sent to admin

### UC10: Viewing Analytics

**Actor**: Administrator

**Preconditions**: User is logged in with Admin role

**Main Flow**:
1. Admin navigates to admin dashboard
2. System displays:
   - Total posts, comments, users, reactions
   - Top posts by views
   - Top posts by reactions
   - Top posts by comments
   - Recent shares by platform
   - Publication trends chart
   - Pending comments count
3. Admin reviews statistics
4. Admin makes data-driven decisions

**Postconditions**: Analytics viewed

## Integration Scenarios

### Scenario 1: Real-Time Collaboration
Multiple users viewing the same post see reactions and comments appear in real-time without refreshing.

### Scenario 2: Bilingual Content Discovery
User switches language and all content (posts, UI, navigation) updates to selected language with appropriate layout direction.

### Scenario 3: Content Moderation Workflow
1. User submits comment
2. Author receives notification
3. Admin receives moderation request
4. Admin approves comment
5. Comment appears in real-time
6. Commenter receives approval notification

### Scenario 4: Author Publishing Workflow
1. Author creates post with bilingual content
2. Author assigns tags, categories, series
3. Author publishes post
4. Post appears on homepage
5. Visitors can read and interact
6. Author monitors engagement

### Scenario 5: Mobile User Experience
1. User accesses site on mobile device
2. System displays responsive layout
3. User navigates with touch gestures
4. User switches theme for better readability
5. User interacts with touch-friendly buttons
6. System maintains performance (Lighthouse ≥ 95)

## Edge Cases

- User loses internet connection during comment submission
- Multiple users react to same post simultaneously
- Admin deletes post while users are viewing it
- User switches language while submitting form
- Database connection fails during operation
- SignalR connection drops and reconnects
- Rate limit reached on contact form
- Slug collision when creating post
- User tries to edit another author's post
- Comment nesting exceeds reasonable depth

## Success Metrics

- User engagement (reactions, comments)
- Content reach (views, shares)
- Platform performance (Lighthouse score)
- User satisfaction (contact form submissions)
- Content quality (moderation approval rate)
- Platform stability (uptime, error rate)
