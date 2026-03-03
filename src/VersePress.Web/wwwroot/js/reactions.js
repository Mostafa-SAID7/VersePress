// Real-time reactions for VersePress
// Handles reaction buttons, AJAX operations, and real-time updates
// Requirements: 4.1, 4.2, 4.6

(function() {
    'use strict';

    // Reaction Manager
    const ReactionManager = {
        // Current user's reaction
        currentUserReaction: null,

        // Reaction counts cache
        reactionCounts: {},

        // Initialize reaction functionality
        init: function() {
            console.log('Initializing ReactionManager...');

            // Load initial reaction state
            this.loadInitialState();

            // Setup reaction button handlers
            this.setupReactionButtons();

            // Listen for real-time reaction updates
            this.listenForReactionUpdates();
        },

        // Load initial reaction state from page
        loadInitialState: function() {
            // Get current user's reaction from data attribute
            const reactionContainer = document.querySelector('.reaction-buttons');
            if (reactionContainer) {
                const userReaction = reactionContainer.dataset.userReaction;
                if (userReaction && userReaction !== '') {
                    this.currentUserReaction = userReaction;
                }

                // Load reaction counts
                const reactionButtons = document.querySelectorAll('.reaction-btn');
                reactionButtons.forEach(button => {
                    const reactionType = button.dataset.reactionType;
                    const count = parseInt(button.dataset.count || '0', 10);
                    this.reactionCounts[reactionType] = count;
                });
            }
        },

        // Setup reaction button click handlers
        setupReactionButtons: function() {
            const reactionButtons = document.querySelectorAll('.reaction-btn');

            reactionButtons.forEach(button => {
                button.addEventListener('click', async (e) => {
                    e.preventDefault();
                    
                    const reactionType = button.dataset.reactionType;
                    const blogPostId = button.dataset.blogPostId;

                    if (!blogPostId) {
                        console.error('Blog post ID not found');
                        return;
                    }

                    // Check if user is authenticated
                    if (!this.isUserAuthenticated()) {
                        this.showAuthenticationRequired();
                        return;
                    }

                    // Disable button during request
                    button.disabled = true;

                    try {
                        // Toggle reaction
                        if (this.currentUserReaction === reactionType) {
                            // Remove reaction
                            await this.removeReaction(blogPostId);
                        } else {
                            // Add or update reaction
                            await this.addReaction(blogPostId, reactionType);
                        }

                        // Animate button
                        this.animateReactionButton(button);

                    } catch (error) {
                        console.error('Error toggling reaction:', error);
                        this.showError('Failed to update reaction. Please try again.');
                    } finally {
                        button.disabled = false;
                    }
                });
            });
        },

        // Check if user is authenticated
        isUserAuthenticated: function() {
            const authMeta = document.querySelector('meta[name="user-authenticated"]');
            return authMeta && authMeta.content === 'true';
        },

        // Show authentication required message
        showAuthenticationRequired: function() {
            this.showToast('Please log in to react to posts', 'info');
        },

        // Add or update reaction
        addReaction: async function(blogPostId, reactionType) {
            const response = await fetch('/api/reactions', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    blogPostId: blogPostId,
                    reactionType: reactionType
                })
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.error || 'Failed to add reaction');
            }

            const result = await response.json();

            // Update local state
            const previousReaction = this.currentUserReaction;
            this.currentUserReaction = reactionType;

            // Update UI immediately (optimistic update)
            this.updateReactionUI(reactionType, previousReaction);

            // Show success feedback
            this.showToast('Reaction added!', 'success');

            return result;
        },

        // Remove reaction
        removeReaction: async function(blogPostId) {
            const response = await fetch(`/api/reactions/${blogPostId}`, {
                method: 'DELETE',
                headers: {
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.error || 'Failed to remove reaction');
            }

            // Update local state
            const previousReaction = this.currentUserReaction;
            this.currentUserReaction = null;

            // Update UI immediately (optimistic update)
            this.updateReactionUI(null, previousReaction);

            // Show success feedback
            this.showToast('Reaction removed', 'info');
        },

        // Update reaction UI
        updateReactionUI: function(newReaction, previousReaction) {
            // Update counts
            if (previousReaction) {
                this.reactionCounts[previousReaction] = Math.max(0, (this.reactionCounts[previousReaction] || 0) - 1);
            }
            if (newReaction) {
                this.reactionCounts[newReaction] = (this.reactionCounts[newReaction] || 0) + 1;
            }

            // Update button states and counts
            const reactionButtons = document.querySelectorAll('.reaction-btn');
            reactionButtons.forEach(button => {
                const reactionType = button.dataset.reactionType;
                const count = this.reactionCounts[reactionType] || 0;

                // Update count display
                const countSpan = button.querySelector('.reaction-count');
                if (countSpan) {
                    countSpan.textContent = count;
                    this.animateCountChange(countSpan);
                }

                // Update active state
                if (reactionType === newReaction) {
                    button.classList.add('active', 'btn-primary');
                    button.classList.remove('btn-outline-primary');
                } else {
                    button.classList.remove('active', 'btn-primary');
                    button.classList.add('btn-outline-primary');
                }
            });
        },

        // Listen for real-time reaction updates from SignalR
        listenForReactionUpdates: function() {
            window.addEventListener('reactionUpdated', (event) => {
                const reaction = event.detail;
                console.log('Real-time reaction update received:', reaction);

                // Update reaction counts from server
                if (reaction.reactionCounts) {
                    this.reactionCounts = reaction.reactionCounts;
                    this.updateAllReactionCounts();
                }
            });
        },

        // Update all reaction counts in UI
        updateAllReactionCounts: function() {
            const reactionButtons = document.querySelectorAll('.reaction-btn');
            reactionButtons.forEach(button => {
                const reactionType = button.dataset.reactionType;
                const count = this.reactionCounts[reactionType] || 0;

                const countSpan = button.querySelector('.reaction-count');
                if (countSpan) {
                    const oldCount = parseInt(countSpan.textContent || '0', 10);
                    if (oldCount !== count) {
                        countSpan.textContent = count;
                        this.animateCountChange(countSpan);
                    }
                }
            });
        },

        // Animate reaction button
        animateReactionButton: function(button) {
            // Check if animations should be disabled
            if (window.AccessibilityManager && window.AccessibilityManager.shouldDisableAnimations()) {
                return;
            }

            // Add animation class
            button.classList.add('feedback-success');

            // Remove animation class after animation completes
            setTimeout(() => {
                button.classList.remove('feedback-success');
            }, 600);

            // Trigger Lordicon animation if present
            const lordIcon = button.querySelector('lord-icon');
            if (lordIcon) {
                lordIcon.setAttribute('trigger', 'click');
                setTimeout(() => {
                    lordIcon.setAttribute('trigger', 'hover');
                }, 1000);
            }
        },

        // Animate count change
        animateCountChange: function(countElement) {
            // Check if animations should be disabled
            if (window.AccessibilityManager && window.AccessibilityManager.shouldDisableAnimations()) {
                return;
            }

            // Add pulse animation
            countElement.style.transform = 'scale(1.3)';
            countElement.style.transition = 'transform 0.2s ease';

            setTimeout(() => {
                countElement.style.transform = 'scale(1)';
            }, 200);
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

        // Show toast notification
        showToast: function(message, type = 'info') {
            // Create toast element
            const toast = document.createElement('div');
            toast.className = `toast-notification toast-${type}`;
            toast.textContent = message;
            toast.setAttribute('role', 'alert');
            toast.setAttribute('aria-live', 'polite');

            document.body.appendChild(toast);

            // Show toast
            setTimeout(() => {
                toast.classList.add('show');
            }, 10);

            // Hide and remove toast after 3 seconds
            setTimeout(() => {
                toast.classList.remove('show');
                setTimeout(() => {
                    toast.remove();
                }, 300);
            }, 3000);

            // Announce to screen readers
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
            ReactionManager.init();
        });
    } else {
        ReactionManager.init();
    }

    // Export for global access
    window.ReactionManager = ReactionManager;

})();
