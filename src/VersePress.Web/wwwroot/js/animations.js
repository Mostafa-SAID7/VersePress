// Animation utilities for VersePress
// Handles Lottie animations and visual feedback with accessibility support

(function() {
    'use strict';

    // Check if user prefers reduced motion
    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    // Lottie Animation Manager
    const LottieManager = {
        // Loading animation instance
        loadingAnimation: null,

        // Initialize Lottie loading animation
        initLoadingAnimation: function(container) {
            if (prefersReducedMotion || !window.lottie) {
                // Fallback to simple spinner if reduced motion or Lottie not loaded
                container.innerHTML = '<div class="loading-spinner"></div>';
                return null;
            }

            try {
                const animation = lottie.loadAnimation({
                    container: container,
                    renderer: 'svg',
                    loop: true,
                    autoplay: true,
                    path: '/animations/loading.json' // Path to Lottie JSON file
                });
                return animation;
            } catch (error) {
                console.warn('Failed to load Lottie animation:', error);
                container.innerHTML = '<div class="loading-spinner"></div>';
                return null;
            }
        },

        // Show loading animation
        showLoading: function(targetElement) {
            if (!targetElement) return null;

            const loadingContainer = document.createElement('div');
            loadingContainer.className = 'lottie-loading-container';
            loadingContainer.setAttribute('role', 'status');
            loadingContainer.setAttribute('aria-live', 'polite');
            loadingContainer.setAttribute('aria-label', 'Loading');
            
            targetElement.appendChild(loadingContainer);
            
            return this.initLoadingAnimation(loadingContainer);
        },

        // Hide loading animation
        hideLoading: function(animation, container) {
            if (animation) {
                animation.destroy();
            }
            if (container) {
                container.remove();
            }
        }
    };

    // Visual Feedback Manager
    const FeedbackManager = {
        // Show success feedback
        showSuccess: function(element, message) {
            if (prefersReducedMotion) {
                this.showSimpleFeedback(element, message, 'success');
                return;
            }

            element.classList.add('feedback-success');
            setTimeout(() => {
                element.classList.remove('feedback-success');
            }, 600);

            if (message) {
                this.showToast(message, 'success');
            }
        },

        // Show error feedback
        showError: function(element, message) {
            if (prefersReducedMotion) {
                this.showSimpleFeedback(element, message, 'error');
                return;
            }

            element.classList.add('feedback-error');
            setTimeout(() => {
                element.classList.remove('feedback-error');
            }, 600);

            if (message) {
                this.showToast(message, 'error');
            }
        },

        // Simple feedback for reduced motion
        showSimpleFeedback: function(element, message, type) {
            const originalBg = element.style.backgroundColor;
            element.style.backgroundColor = type === 'success' ? '#d4edda' : '#f8d7da';
            
            setTimeout(() => {
                element.style.backgroundColor = originalBg;
            }, 300);

            if (message) {
                this.showToast(message, type);
            }
        },

        // Show toast notification
        showToast: function(message, type = 'info') {
            const toast = document.createElement('div');
            toast.className = `toast-notification toast-${type}`;
            toast.setAttribute('role', 'alert');
            toast.setAttribute('aria-live', 'assertive');
            toast.textContent = message;

            document.body.appendChild(toast);

            // Trigger animation
            setTimeout(() => {
                toast.classList.add('show');
            }, 10);

            // Remove after 3 seconds
            setTimeout(() => {
                toast.classList.remove('show');
                setTimeout(() => {
                    toast.remove();
                }, 300);
            }, 3000);
        }
    };

    // Comment submission handler with loading animation
    window.submitCommentWithAnimation = async function(form, event) {
        if (event) {
            event.preventDefault();
        }

        const submitButton = form.querySelector('button[type="submit"]');
        const originalText = submitButton.textContent;
        
        // Disable button and show loading
        submitButton.disabled = true;
        
        let loadingAnimation = null;
        let loadingContainer = null;

        if (!prefersReducedMotion) {
            loadingContainer = document.createElement('span');
            loadingContainer.className = 'btn-loading-indicator';
            submitButton.appendChild(loadingContainer);
            loadingAnimation = LottieManager.initLoadingAnimation(loadingContainer);
        } else {
            submitButton.textContent = 'Submitting...';
        }

        try {
            // Submit form via AJAX
            const formData = new FormData(form);
            const response = await fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (response.ok) {
                FeedbackManager.showSuccess(form, 'Comment submitted successfully!');
                form.reset();
            } else {
                FeedbackManager.showError(form, 'Failed to submit comment. Please try again.');
            }
        } catch (error) {
            console.error('Comment submission error:', error);
            FeedbackManager.showError(form, 'An error occurred. Please try again.');
        } finally {
            // Restore button state
            submitButton.disabled = false;
            submitButton.textContent = originalText;
            
            if (loadingAnimation) {
                LottieManager.hideLoading(loadingAnimation, loadingContainer);
            }
        }
    };

    // Reaction update handler with loading animation
    window.updateReactionWithAnimation = async function(button, postId, reactionType) {
        const originalContent = button.innerHTML;
        
        // Disable button
        button.disabled = true;
        
        let loadingAnimation = null;
        let loadingContainer = null;

        if (!prefersReducedMotion) {
            loadingContainer = document.createElement('span');
            loadingContainer.className = 'btn-loading-indicator-small';
            button.innerHTML = '';
            button.appendChild(loadingContainer);
            loadingAnimation = LottieManager.initLoadingAnimation(loadingContainer);
        }

        try {
            const response = await fetch(`/api/reactions/${postId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify({ reactionType: reactionType })
            });

            if (response.ok) {
                const data = await response.json();
                FeedbackManager.showSuccess(button, null);
                
                // Update reaction count
                if (data.count !== undefined) {
                    const countElement = button.querySelector('.reaction-count');
                    if (countElement) {
                        countElement.textContent = data.count;
                    }
                }
            } else {
                FeedbackManager.showError(button, 'Failed to update reaction.');
            }
        } catch (error) {
            console.error('Reaction update error:', error);
            FeedbackManager.showError(button, 'An error occurred.');
        } finally {
            // Restore button state
            button.disabled = false;
            button.innerHTML = originalContent;
            
            if (loadingAnimation) {
                LottieManager.hideLoading(loadingAnimation, loadingContainer);
            }
        }
    };

    // Export managers for global access
    window.LottieManager = LottieManager;
    window.FeedbackManager = FeedbackManager;

    // Initialize on DOM ready
    document.addEventListener('DOMContentLoaded', function() {
        // Add event listeners for comment forms
        document.querySelectorAll('.comment-form').forEach(form => {
            form.addEventListener('submit', function(e) {
                e.preventDefault();
                submitCommentWithAnimation(this, e);
            });
        });

        // Add event listeners for reaction buttons
        document.querySelectorAll('.reaction-btn').forEach(button => {
            button.addEventListener('click', function(e) {
                e.preventDefault();
                const postId = this.dataset.postId;
                const reactionType = this.dataset.reactionType;
                updateReactionWithAnimation(this, postId, reactionType);
            });
        });
    });

})();
