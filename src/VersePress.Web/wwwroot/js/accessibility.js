// Accessibility utilities for VersePress
// Handles prefers-reduced-motion and animation accessibility

(function() {
    'use strict';

    // Accessibility Manager
    const AccessibilityManager = {
        // Check if user prefers reduced motion
        prefersReducedMotion: false,

        // Initialize accessibility features
        init: function() {
            // Check initial preference
            this.checkReducedMotionPreference();

            // Listen for changes in motion preference
            this.watchMotionPreference();

            // Apply accessibility settings
            this.applyAccessibilitySettings();

            // Add ARIA labels to animated elements
            this.addAriaLabels();

            // Handle keyboard navigation for animated elements
            this.setupKeyboardNavigation();
        },

        // Check user's motion preference
        checkReducedMotionPreference: function() {
            const mediaQuery = window.matchMedia('(prefers-reduced-motion: reduce)');
            this.prefersReducedMotion = mediaQuery.matches;

            if (this.prefersReducedMotion) {
                document.documentElement.classList.add('reduce-motion');
                console.log('Reduced motion preference detected - animations disabled');
            } else {
                document.documentElement.classList.remove('reduce-motion');
            }

            return this.prefersReducedMotion;
        },

        // Watch for changes in motion preference
        watchMotionPreference: function() {
            const mediaQuery = window.matchMedia('(prefers-reduced-motion: reduce)');
            
            // Modern browsers
            if (mediaQuery.addEventListener) {
                mediaQuery.addEventListener('change', (e) => {
                    this.prefersReducedMotion = e.matches;
                    this.applyAccessibilitySettings();
                });
            } 
            // Older browsers
            else if (mediaQuery.addListener) {
                mediaQuery.addListener((e) => {
                    this.prefersReducedMotion = e.matches;
                    this.applyAccessibilitySettings();
                });
            }
        },

        // Apply accessibility settings based on preferences
        applyAccessibilitySettings: function() {
            if (this.prefersReducedMotion) {
                // Disable Lottie animations
                this.disableLottieAnimations();

                // Disable Lordicon animations
                this.disableLordiconAnimations();

                // Disable CSS animations
                this.disableCSSAnimations();

                // Use instant transitions
                this.useInstantTransitions();
            } else {
                // Re-enable animations if preference changes
                this.enableAnimations();
            }
        },

        // Disable Lottie animations
        disableLottieAnimations: function() {
            if (window.LottieManager) {
                // Stop all active Lottie animations
                document.querySelectorAll('.lottie-loading-container').forEach(container => {
                    container.innerHTML = '<div class="loading-spinner" role="status" aria-label="Loading"></div>';
                });
            }
        },

        // Disable Lordicon animations
        disableLordiconAnimations: function() {
            document.querySelectorAll('lord-icon').forEach(icon => {
                icon.setAttribute('trigger', 'none');
                icon.style.animation = 'none';
            });
        },

        // Disable CSS animations
        disableCSSAnimations: function() {
            document.documentElement.classList.add('reduce-motion');
            
            // Add inline style to ensure animations are disabled
            if (!document.getElementById('reduce-motion-style')) {
                const style = document.createElement('style');
                style.id = 'reduce-motion-style';
                style.textContent = `
                    .reduce-motion *,
                    .reduce-motion *::before,
                    .reduce-motion *::after {
                        animation-duration: 0.01ms !important;
                        animation-iteration-count: 1 !important;
                        transition-duration: 0.01ms !important;
                        scroll-behavior: auto !important;
                    }
                `;
                document.head.appendChild(style);
            }
        },

        // Use instant transitions
        useInstantTransitions: function() {
            document.body.style.setProperty('--transition-duration', '0.01ms');
        },

        // Enable animations
        enableAnimations: function() {
            document.documentElement.classList.remove('reduce-motion');
            
            // Remove reduce motion style
            const style = document.getElementById('reduce-motion-style');
            if (style) {
                style.remove();
            }

            // Restore normal transitions
            document.body.style.removeProperty('--transition-duration');

            // Re-enable Lordicon animations
            document.querySelectorAll('lord-icon').forEach(icon => {
                icon.setAttribute('trigger', 'hover');
            });
        },

        // Add ARIA labels to animated elements
        addAriaLabels: function() {
            // Add labels to loading animations
            document.querySelectorAll('.lottie-loading-container, .loading-spinner').forEach(element => {
                if (!element.getAttribute('role')) {
                    element.setAttribute('role', 'status');
                }
                if (!element.getAttribute('aria-label')) {
                    element.setAttribute('aria-label', 'Loading');
                }
                if (!element.getAttribute('aria-live')) {
                    element.setAttribute('aria-live', 'polite');
                }
            });

            // Add labels to reaction buttons
            document.querySelectorAll('.reaction-btn').forEach(button => {
                const reactionType = button.dataset.reactionType;
                if (reactionType && !button.getAttribute('aria-label')) {
                    button.setAttribute('aria-label', `React with ${reactionType}`);
                }
            });

            // Add labels to theme toggle
            const themeToggle = document.querySelector('.theme-toggle-btn');
            if (themeToggle && !themeToggle.getAttribute('aria-label')) {
                const currentTheme = document.body.dataset.theme || 'light';
                const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
                themeToggle.setAttribute('aria-label', `Switch to ${newTheme} theme`);
            }

            // Add labels to notification bell
            const notificationBell = document.querySelector('.notification-bell');
            if (notificationBell && !notificationBell.getAttribute('aria-label')) {
                notificationBell.setAttribute('aria-label', 'View notifications');
            }
        },

        // Setup keyboard navigation for animated elements
        setupKeyboardNavigation: function() {
            // Ensure all interactive animated elements are keyboard accessible
            document.querySelectorAll('.reaction-btn, .theme-toggle-btn, .notification-bell').forEach(element => {
                // Ensure element is focusable
                if (!element.hasAttribute('tabindex') && element.tagName !== 'BUTTON' && element.tagName !== 'A') {
                    element.setAttribute('tabindex', '0');
                }

                // Add keyboard event listeners
                element.addEventListener('keydown', (e) => {
                    // Activate on Enter or Space
                    if (e.key === 'Enter' || e.key === ' ') {
                        e.preventDefault();
                        element.click();
                    }
                });

                // Add focus visible styles
                element.addEventListener('focus', () => {
                    element.classList.add('keyboard-focus');
                });

                element.addEventListener('blur', () => {
                    element.classList.remove('keyboard-focus');
                });
            });
        },

        // Announce dynamic content changes to screen readers
        announceToScreenReader: function(message, priority = 'polite') {
            const announcement = document.createElement('div');
            announcement.setAttribute('role', 'status');
            announcement.setAttribute('aria-live', priority);
            announcement.setAttribute('aria-atomic', 'true');
            announcement.className = 'sr-only';
            announcement.textContent = message;

            document.body.appendChild(announcement);

            // Remove after announcement
            setTimeout(() => {
                announcement.remove();
            }, 1000);
        },

        // Check if animations should be disabled
        shouldDisableAnimations: function() {
            return this.prefersReducedMotion;
        }
    };

    // Screen reader only utility class
    if (!document.querySelector('style[data-sr-only]')) {
        const srStyle = document.createElement('style');
        srStyle.setAttribute('data-sr-only', 'true');
        srStyle.textContent = `
            .sr-only {
                position: absolute;
                width: 1px;
                height: 1px;
                padding: 0;
                margin: -1px;
                overflow: hidden;
                clip: rect(0, 0, 0, 0);
                white-space: nowrap;
                border-width: 0;
            }

            .keyboard-focus {
                outline: 2px solid #0d6efd;
                outline-offset: 2px;
            }
        `;
        document.head.appendChild(srStyle);
    }

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            AccessibilityManager.init();
        });
    } else {
        AccessibilityManager.init();
    }

    // Export for global access
    window.AccessibilityManager = AccessibilityManager;

    // Integrate with existing animation managers
    window.addEventListener('load', function() {
        // Override animation functions if reduced motion is preferred
        if (AccessibilityManager.shouldDisableAnimations()) {
            // Override Lottie animations
            if (window.LottieManager) {
                const originalShowLoading = window.LottieManager.showLoading;
                window.LottieManager.showLoading = function(targetElement) {
                    if (!targetElement) return null;
                    const loadingContainer = document.createElement('div');
                    loadingContainer.className = 'loading-spinner';
                    loadingContainer.setAttribute('role', 'status');
                    loadingContainer.setAttribute('aria-live', 'polite');
                    loadingContainer.setAttribute('aria-label', 'Loading');
                    targetElement.appendChild(loadingContainer);
                    return null;
                };
            }

            // Override Lordicon animations
            if (window.LordiconManager) {
                window.LordiconManager.disableAnimations();
            }
        }
    });

})();
