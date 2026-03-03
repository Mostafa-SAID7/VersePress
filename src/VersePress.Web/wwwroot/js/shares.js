// Share tracking for VersePress
// Handles social media share buttons and tracking
// Requirements: 8.1, 19.5

(function() {
    'use strict';

    // Share Manager
    const ShareManager = {
        // Initialize share functionality
        init: function() {
            console.log('Initializing ShareManager...');

            // Setup share button handlers
            this.setupShareButtons();
        },

        // Setup share button click handlers
        setupShareButtons: function() {
            const shareButtons = document.querySelectorAll('.share-btn');

            shareButtons.forEach(button => {
                button.addEventListener('click', async (e) => {
                    e.preventDefault();

                    const platform = button.dataset.platform;
                    const blogPostId = button.dataset.blogPostId;
                    const shareUrl = button.dataset.shareUrl || window.location.href;
                    const shareTitle = button.dataset.shareTitle || document.title;

                    if (!platform || !blogPostId) {
                        console.error('Platform or blog post ID not found');
                        return;
                    }

                    // Record share event
                    this.recordShare(blogPostId, platform);

                    // Open share window
                    this.openShareWindow(platform, shareUrl, shareTitle);
                });
            });
        },

        // Record share event via AJAX
        recordShare: async function(blogPostId, platform) {
            try {
                const response = await fetch('/api/shares', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': this.getAntiForgeryToken()
                    },
                    body: JSON.stringify({
                        blogPostId: blogPostId,
                        platform: this.getPlatformEnum(platform)
                    })
                });

                if (!response.ok) {
                    console.error('Failed to record share');
                }

                console.log('Share recorded:', platform);

            } catch (error) {
                console.error('Error recording share:', error);
                // Don't show error to user - share tracking is non-blocking
            }
        },

        // Get platform enum value
        getPlatformEnum: function(platform) {
            const platformMap = {
                'twitter': 0,
                'facebook': 1,
                'linkedin': 2,
                'whatsapp': 3
            };

            return platformMap[platform.toLowerCase()] || 0;
        },

        // Open share window
        openShareWindow: function(platform, url, title) {
            const encodedUrl = encodeURIComponent(url);
            const encodedTitle = encodeURIComponent(title);

            let shareUrl = '';

            switch (platform.toLowerCase()) {
                case 'twitter':
                    shareUrl = `https://twitter.com/intent/tweet?url=${encodedUrl}&text=${encodedTitle}`;
                    break;
                case 'facebook':
                    shareUrl = `https://www.facebook.com/sharer/sharer.php?u=${encodedUrl}`;
                    break;
                case 'linkedin':
                    shareUrl = `https://www.linkedin.com/sharing/share-offsite/?url=${encodedUrl}`;
                    break;
                case 'whatsapp':
                    shareUrl = `https://wa.me/?text=${encodedTitle}%20${encodedUrl}`;
                    break;
                default:
                    console.error('Unknown platform:', platform);
                    return;
            }

            // Open in popup window
            const width = 600;
            const height = 400;
            const left = (screen.width - width) / 2;
            const top = (screen.height - height) / 2;

            window.open(
                shareUrl,
                'share',
                `width=${width},height=${height},left=${left},top=${top},toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes`
            );

            // Animate share button
            this.animateShareButton(platform);
        },

        // Animate share button
        animateShareButton: function(platform) {
            const button = document.querySelector(`.share-btn[data-platform="${platform}"]`);
            if (!button) {
                return;
            }

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
        }
    };

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            ShareManager.init();
        });
    } else {
        ShareManager.init();
    }

    // Export for global access
    window.ShareManager = ShareManager;

})();
