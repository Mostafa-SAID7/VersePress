// Lordicon integration for VersePress
// Handles animated icons for reactions, notifications, and theme toggle

(function() {
    'use strict';

    // Check if user prefers reduced motion
    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    // Lordicon Manager
    const LordiconManager = {
        // Initialize Lordicon elements
        init: function() {
            if (prefersReducedMotion) {
                // Disable animations if user prefers reduced motion
                this.disableAnimations();
                return;
            }

            // Initialize reaction icons
            this.initReactionIcons();
            
            // Initialize notification icon
            this.initNotificationIcon();
            
            // Initialize theme toggle icon
            this.initThemeToggleIcon();
        },

        // Disable all Lordicon animations
        disableAnimations: function() {
            document.querySelectorAll('lord-icon').forEach(icon => {
                icon.setAttribute('trigger', 'none');
            });
        },

        // Initialize reaction button icons
        initReactionIcons: function() {
            const reactionButtons = document.querySelectorAll('.reaction-btn');
            
            reactionButtons.forEach(button => {
                const reactionType = button.dataset.reactionType;
                const iconConfig = this.getReactionIconConfig(reactionType);
                
                if (iconConfig) {
                    const lordIcon = this.createLordIcon(iconConfig);
                    
                    // Insert icon before text
                    const textNode = button.childNodes[0];
                    if (textNode) {
                        button.insertBefore(lordIcon, textNode);
                    } else {
                        button.appendChild(lordIcon);
                    }

                    // Trigger animation on click
                    button.addEventListener('click', () => {
                        if (!prefersReducedMotion) {
                            lordIcon.setAttribute('trigger', 'click');
                            setTimeout(() => {
                                lordIcon.setAttribute('trigger', 'hover');
                            }, 1000);
                        }
                    });
                }
            });
        },

        // Initialize notification icon
        initNotificationIcon: function() {
            const notificationBell = document.querySelector('.notification-bell');
            
            if (notificationBell) {
                const iconConfig = {
                    src: 'https://cdn.lordicon.com/vspbqszr.json',
                    trigger: 'hover',
                    colors: 'primary:#121331,secondary:#0d6efd',
                    style: 'width:28px;height:28px'
                };

                const lordIcon = this.createLordIcon(iconConfig);
                notificationBell.innerHTML = '';
                notificationBell.appendChild(lordIcon);

                // Animate on new notification
                window.addEventListener('newNotification', () => {
                    if (!prefersReducedMotion) {
                        lordIcon.setAttribute('trigger', 'loop');
                        setTimeout(() => {
                            lordIcon.setAttribute('trigger', 'hover');
                        }, 2000);
                    }
                });
            }
        },

        // Initialize theme toggle icon
        initThemeToggleIcon: function() {
            const themeToggle = document.querySelector('.theme-toggle-btn');
            
            if (themeToggle) {
                const currentTheme = document.body.dataset.theme || 'light';
                const iconConfig = this.getThemeIconConfig(currentTheme);

                const lordIcon = this.createLordIcon(iconConfig);
                
                // Replace existing content with icon
                const textContent = themeToggle.textContent;
                themeToggle.innerHTML = '';
                themeToggle.appendChild(lordIcon);
                
                const textSpan = document.createElement('span');
                textSpan.className = 'ms-2';
                textSpan.textContent = textContent.trim();
                themeToggle.appendChild(textSpan);

                // Animate on theme change
                themeToggle.addEventListener('click', () => {
                    if (!prefersReducedMotion) {
                        lordIcon.setAttribute('trigger', 'click');
                        
                        // Update icon after animation
                        setTimeout(() => {
                            const newTheme = document.body.dataset.theme || 'light';
                            const newConfig = this.getThemeIconConfig(newTheme);
                            lordIcon.setAttribute('src', newConfig.src);
                            lordIcon.setAttribute('colors', newConfig.colors);
                            lordIcon.setAttribute('trigger', 'hover');
                        }, 500);
                    }
                });
            }
        },

        // Create Lordicon element
        createLordIcon: function(config) {
            const lordIcon = document.createElement('lord-icon');
            lordIcon.setAttribute('src', config.src);
            lordIcon.setAttribute('trigger', config.trigger || 'hover');
            
            if (config.colors) {
                lordIcon.setAttribute('colors', config.colors);
            }
            
            if (config.style) {
                lordIcon.setAttribute('style', config.style);
            }

            if (config.state) {
                lordIcon.setAttribute('state', config.state);
            }

            return lordIcon;
        },

        // Get reaction icon configuration
        getReactionIconConfig: function(reactionType) {
            const configs = {
                'like': {
                    src: 'https://cdn.lordicon.com/ohfmmfhn.json',
                    trigger: 'hover',
                    colors: 'primary:#121331,secondary:#0d6efd',
                    style: 'width:24px;height:24px'
                },
                'love': {
                    src: 'https://cdn.lordicon.com/ulnswmkk.json',
                    trigger: 'hover',
                    colors: 'primary:#dc3545,secondary:#ff6b9d',
                    style: 'width:24px;height:24px'
                },
                'celebrate': {
                    src: 'https://cdn.lordicon.com/lupuorrc.json',
                    trigger: 'hover',
                    colors: 'primary:#ffc107,secondary:#fd7e14',
                    style: 'width:24px;height:24px'
                },
                'insightful': {
                    src: 'https://cdn.lordicon.com/wxnxiano.json',
                    trigger: 'hover',
                    colors: 'primary:#6f42c1,secondary:#9d72ff',
                    style: 'width:24px;height:24px'
                },
                'curious': {
                    src: 'https://cdn.lordicon.com/jnikqyih.json',
                    trigger: 'hover',
                    colors: 'primary:#20c997,secondary:#0dcaf0',
                    style: 'width:24px;height:24px'
                }
            };

            return configs[reactionType.toLowerCase()] || null;
        },

        // Get theme icon configuration
        getThemeIconConfig: function(theme) {
            if (theme === 'dark') {
                return {
                    src: 'https://cdn.lordicon.com/pwlnbxzy.json',
                    trigger: 'hover',
                    colors: 'primary:#ffc107,secondary:#fd7e14',
                    style: 'width:24px;height:24px'
                };
            } else {
                return {
                    src: 'https://cdn.lordicon.com/pwlnbxzy.json',
                    trigger: 'hover',
                    colors: 'primary:#121331,secondary:#0d6efd',
                    style: 'width:24px;height:24px'
                };
            }
        }
    };

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            LordiconManager.init();
        });
    } else {
        LordiconManager.init();
    }

    // Export for global access
    window.LordiconManager = LordiconManager;

})();
