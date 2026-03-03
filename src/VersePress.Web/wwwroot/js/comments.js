// Real-time comments for VersePress
// Handles comment submission, AJAX operations, and real-time updates
// Requirements: 5.1, 5.4

(function() {
    'use strict';

    // Comment Manager
    const CommentManager = {
        // Comment form reference
        commentForm: null,
        commentTextarea: null,
        submitButton: null,

        // Loading state
        isSubmitting: false,

        // Initialize comment functionality
        init: function() {
            console.log('Initializing CommentManager...');

            // Find comment form
            this.commentForm = document.querySelector('.comment-form');
            if (!this.commentForm) {
                console.log('No comment form found on this page');
                return;
            }

            this.commentTextarea = this.commentForm.querySelector('textarea[name="content"]');
            this.submitButton = this.commentForm.querySelector('button[type="submit"]');

            // Setup form submission handler
            this.setupCommentForm();

            // Setup reply button handlers
            this.setupReplyButtons();

            // Listen for real-time comment updates
            this.listenForCommentUpdates();
        },

        // Setup comment form submission
        setupCommentForm: function() {
            this.commentForm.addEventListener('submit', async (e) => {
                e.preventDefault();

                // Check if user is authenticated
                if (!this.isUserAuthenticated()) {
                    this.showAuthenticationRequired();
                    return;
                }

                // Prevent double submission
                if (this.isSubmitting) {
                    return;
                }

                // Get form data
                const content = this.commentTextarea.value.trim();
                const blogPostId = this.commentForm.dataset.blogPostId;
                const parentCommentId = this.commentForm.dataset.parentCommentId || null;

                // Validate content
                if (!content) {
                    this.showError('Please enter a comment');
                    return;
                }

                if (content.length < 1 || content.length > 2000) {
                    this.showError('Comment must be between 1 and 2000 characters');
                    return;
                }

                if (!blogPostId) {
                    this.showError('Blog post ID not found');
                    return;
                }

                // Show loading state
                this.setLoadingState(true);

                try {
                    // Submit comment
                    await this.submitComment(blogPostId, content, parentCommentId);

                    // Clear form
                    this.commentTextarea.value = '';

                    // Reset parent comment if replying
                    if (parentCommentId) {
                        this.cancelReply();
                    }

                    // Show success message
                    this.showToast('Comment submitted! It will appear after approval.', 'success');

                } catch (error) {
                    console.error('Error submitting comment:', error);
                    this.showError('Failed to submit comment. Please try again.');
                } finally {
                    this.setLoadingState(false);
                }
            });
        },

        // Setup reply button handlers
        setupReplyButtons: function() {
            const replyButtons = document.querySelectorAll('.reply-btn');

            replyButtons.forEach(button => {
                button.addEventListener('click', (e) => {
                    e.preventDefault();
                    const commentId = button.dataset.commentId;
                    const authorName = button.dataset.authorName;
                    this.showReplyForm(commentId, authorName);
                });
            });
        },

        // Show reply form for a specific comment
        showReplyForm: function(commentId, authorName) {
            // Set parent comment ID
            this.commentForm.dataset.parentCommentId = commentId;

            // Update form UI to show reply context
            let replyIndicator = this.commentForm.querySelector('.reply-indicator');
            if (!replyIndicator) {
                replyIndicator = document.createElement('div');
                replyIndicator.className = 'reply-indicator alert alert-info d-flex justify-content-between align-items-center';
                this.commentForm.insertBefore(replyIndicator, this.commentTextarea);
            }

            replyIndicator.innerHTML = `
                <span>Replying to <strong>${this.escapeHtml(authorName)}</strong></span>
                <button type="button" class="btn btn-sm btn-outline-secondary cancel-reply-btn">Cancel</button>
            `;

            // Setup cancel button
            const cancelButton = replyIndicator.querySelector('.cancel-reply-btn');
            cancelButton.addEventListener('click', () => {
                this.cancelReply();
            });

            // Focus textarea
            this.commentTextarea.focus();

            // Scroll to form
            this.commentForm.scrollIntoView({ behavior: 'smooth', block: 'center' });
        },

        // Cancel reply
        cancelReply: function() {
            delete this.commentForm.dataset.parentCommentId;

            const replyIndicator = this.commentForm.querySelector('.reply-indicator');
            if (replyIndicator) {
                replyIndicator.remove();
            }
        },

        // Submit comment via AJAX
        submitComment: async function(blogPostId, content, parentCommentId) {
            const response = await fetch('/api/comments', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    blogPostId: blogPostId,
                    content: content,
                    parentCommentId: parentCommentId
                })
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.error || 'Failed to submit comment');
            }

            const result = await response.json();
            return result;
        },

        // Listen for real-time comment updates from SignalR
        listenForCommentUpdates: function() {
            window.addEventListener('commentAdded', (event) => {
                const comment = event.detail;
                console.log('Real-time comment update received:', comment);

                // Only show approved comments
                if (comment.isApproved) {
                    this.addCommentToUI(comment);
                }
            });
        },

        // Add comment to UI
        addCommentToUI: function(comment) {
            const commentsList = document.querySelector('.comments-list');
            if (!commentsList) {
                console.warn('Comments list not found');
                return;
            }

            // Check if comment already exists
            const existingComment = document.querySelector(`[data-comment-id="${comment.id}"]`);
            if (existingComment) {
                console.log('Comment already exists in UI');
                return;
            }

            // Create comment element
            const commentElement = this.createCommentElement(comment);

            // Add to appropriate location
            if (comment.parentCommentId) {
                // Find parent comment and add as reply
                const parentComment = document.querySelector(`[data-comment-id="${comment.parentCommentId}"]`);
                if (parentComment) {
                    let repliesContainer = parentComment.querySelector('.comment-replies');
                    if (!repliesContainer) {
                        repliesContainer = document.createElement('div');
                        repliesContainer.className = 'comment-replies ms-4';
                        parentComment.appendChild(repliesContainer);
                    }
                    repliesContainer.appendChild(commentElement);
                } else {
                    // Parent not found, add to main list
                    commentsList.appendChild(commentElement);
                }
            } else {
                // Add to main list
                commentsList.appendChild(commentElement);
            }

            // Highlight new comment
            this.highlightNewComment(commentElement);

            // Update comment count
            this.updateCommentCount(1);

            // Announce to screen readers
            if (window.AccessibilityManager) {
                window.AccessibilityManager.announceToScreenReader(
                    `New comment from ${comment.authorName}`,
                    'polite'
                );
            }
        },

        // Create comment HTML element
        createCommentElement: function(comment) {
            const div = document.createElement('div');
            div.className = 'comment mb-3 p-3 border rounded';
            div.dataset.commentId = comment.id;

            const formattedDate = this.formatDate(comment.createdAt);

            div.innerHTML = `
                <div class="comment-header d-flex justify-content-between align-items-start mb-2">
                    <div>
                        <strong class="comment-author">${this.escapeHtml(comment.authorName)}</strong>
                        <small class="text-muted ms-2">${formattedDate}</small>
                    </div>
                    ${this.isUserAuthenticated() ? `
                        <button class="btn btn-sm btn-link reply-btn" data-comment-id="${comment.id}" data-author-name="${this.escapeHtml(comment.authorName)}">
                            Reply
                        </button>
                    ` : ''}
                </div>
                <div class="comment-content">
                    ${this.escapeHtml(comment.content)}
                </div>
            `;

            // Setup reply button if present
            const replyBtn = div.querySelector('.reply-btn');
            if (replyBtn) {
                replyBtn.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.showReplyForm(comment.id, comment.authorName);
                });
            }

            return div;
        },

        // Highlight new comment
        highlightNewComment: function(commentElement) {
            // Check if animations should be disabled
            if (window.AccessibilityManager && window.AccessibilityManager.shouldDisableAnimations()) {
                return;
            }

            // Add highlight class
            commentElement.classList.add('new-comment-highlight');

            // Scroll to comment
            commentElement.scrollIntoView({ behavior: 'smooth', block: 'nearest' });

            // Remove highlight after animation
            setTimeout(() => {
                commentElement.classList.remove('new-comment-highlight');
            }, 3000);
        },

        // Update comment count display
        updateCommentCount: function(increment) {
            const countElement = document.querySelector('.comment-count');
            if (countElement) {
                const currentCount = parseInt(countElement.textContent || '0', 10);
                const newCount = currentCount + increment;
                countElement.textContent = newCount;

                // Animate count change
                if (window.AccessibilityManager && !window.AccessibilityManager.shouldDisableAnimations()) {
                    countElement.style.transform = 'scale(1.2)';
                    countElement.style.transition = 'transform 0.2s ease';
                    setTimeout(() => {
                        countElement.style.transform = 'scale(1)';
                    }, 200);
                }
            }
        },

        // Set loading state
        setLoadingState: function(loading) {
            this.isSubmitting = loading;

            if (loading) {
                this.submitButton.disabled = true;
                this.commentTextarea.disabled = true;

                // Show loading indicator
                const originalText = this.submitButton.textContent;
                this.submitButton.dataset.originalText = originalText;
                this.submitButton.innerHTML = `
                    <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    Submitting...
                `;

                // Show Lottie loading animation if available
                if (window.LottieManager && !window.AccessibilityManager?.shouldDisableAnimations()) {
                    const loadingContainer = document.createElement('div');
                    loadingContainer.className = 'lottie-loading-container btn-loading-indicator';
                    this.submitButton.appendChild(loadingContainer);
                    window.LottieManager.showLoading(loadingContainer);
                }
            } else {
                this.submitButton.disabled = false;
                this.commentTextarea.disabled = false;

                // Restore button text
                const originalText = this.submitButton.dataset.originalText || 'Submit Comment';
                this.submitButton.textContent = originalText;
            }
        },

        // Check if user is authenticated
        isUserAuthenticated: function() {
            const authMeta = document.querySelector('meta[name="user-authenticated"]');
            return authMeta && authMeta.content === 'true';
        },

        // Show authentication required message
        showAuthenticationRequired: function() {
            this.showToast('Please log in to comment', 'info');
        },

        // Get anti-forgery token
        getAntiForgeryToken: function() {
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            if (tokenInput) {
                return tokenInput.value;
            }

            const tokenMeta = document.querySelector('meta[name="RequestVerificationToken"]');
            if (tokenMeta) {
                return tokenMeta.content;
            }

            return '';
        },

        // Format date
        formatDate: function(dateString) {
            const date = new Date(dateString);
            const now = new Date();
            const diffMs = now - date;
            const diffMins = Math.floor(diffMs / 60000);
            const diffHours = Math.floor(diffMs / 3600000);
            const diffDays = Math.floor(diffMs / 86400000);

            if (diffMins < 1) {
                return 'Just now';
            } else if (diffMins < 60) {
                return `${diffMins} minute${diffMins > 1 ? 's' : ''} ago`;
            } else if (diffHours < 24) {
                return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
            } else if (diffDays < 7) {
                return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
            } else {
                return date.toLocaleDateString();
            }
        },

        // Escape HTML to prevent XSS
        escapeHtml: function(text) {
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        },

        // Show toast notification
        showToast: function(message, type = 'info') {
            const toast = document.createElement('div');
            toast.className = `toast-notification toast-${type}`;
            toast.textContent = message;
            toast.setAttribute('role', 'alert');
            toast.setAttribute('aria-live', 'polite');

            document.body.appendChild(toast);

            setTimeout(() => {
                toast.classList.add('show');
            }, 10);

            setTimeout(() => {
                toast.classList.remove('show');
                setTimeout(() => {
                    toast.remove();
                }, 300);
            }, 3000);

            if (window.AccessibilityManager) {
                window.AccessibilityManager.announceToScreenReader(message, 'polite');
            }
        },

        // Show error message
        showError: function(message) {
            this.showToast(message, 'error');
        }
    };

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            CommentManager.init();
        });
    } else {
        CommentManager.init();
    }

    // Export for global access
    window.CommentManager = CommentManager;

})();
