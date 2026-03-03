// Real-time notifications for VersePress
// Handles notification badge, toast popups, and real-time updates
// Requirements: 19.1, 19.2, 19.3, 19.4

(function() {
    'use strict';

    // Notification Manager
    const NotificationManager = {
        // Notification state
        notifications: [],
        unreadCount: 0,

        // UI elements
        notificationBell: null,
        notificationBadge: null,
        notificationDropdown: null,
        notificationList: null,

        // Initialize notification functionality
        init: function() {
            console.log('Initializing NotificationManager...');

            // Check if user is authenticated
            if (!this.isUserAuthenticated()) {
                console.log('User not authenticated, skipping notification initialization');
                return;
            }

            // Find UI elements
            this.notificationBell = document.querySelector('.notification-bell');
            if (!this.notificationBell) {
                console.log('Notification bell not found');
                return;
            }

            this.notificationBadge = this.notificationBell.querySelector('.notification-badge');
            this.notificationDropdown = document.querySelector('.notification-dropdown');
            this.notificationList = document.querySelector('.notification-list');

            // Load initial notifications
            this.loadNotifications();

            // Setup notification bell click handler
            this.setupNotificationBell();

            // Listen for real-time notification updates
            this.listenForNotificationUpdates();

            // Poll for unread count periodically (fallback)
            this.startUnreadCountPolling();
        },

        // Check if user is authenticated
        isUserAuthenticated: function() {
            const authMeta = document.querySelector('meta[name="user-authenticated"]');
            return authMeta && authMeta.content === 'true';
        },

        // Load notifications from API
        loadNotifications: async function(unreadOnly = false) {
            try {
                const response = await fetch(`/api/notifications?unreadOnly=${unreadOnly}`, {
                    headers: {
                        'RequestVerificationToken': this.getAntiForgeryToken()
                    }
                });

                if (!response.ok) {
                    throw new Error('Failed to load notifications');
                }

                const notifications = await response.json();
                this.notifications = notifications;

                // Update UI
                this.updateNotificationList();
                this.updateUnreadCount();

            } catch (error) {
                console.error('Error loading notifications:', error);
            }
        },

        // Setup notification bell click handler
        setupNotificationBell: function() {
            this.notificationBell.addEventListener('click', (e) => {
                e.preventDefault();
                this.toggleNotificationDropdown();
            });

            // Close dropdown when clicking outside
            document.addEventListener('click', (e) => {
                if (!this.notificationBell.contains(e.target) && 
                    this.notificationDropdown && 
                    !this.notificationDropdown.contains(e.target)) {
                    this.closeNotificationDropdown();
                }
            });
        },

        // Toggle notification dropdown
        toggleNotificationDropdown: function() {
            if (!this.notificationDropdown) {
                return;
            }

            const isVisible = this.notificationDropdown.classList.contains('show');

            if (isVisible) {
                this.closeNotificationDropdown();
            } else {
                this.openNotificationDropdown();
            }
        },

        // Open notification dropdown
        openNotificationDropdown: function() {
            if (!this.notificationDropdown) {
                return;
            }

            this.notificationDropdown.classList.add('show');

            // Load latest notifications
            this.loadNotifications();

            // Announce to screen readers
            if (window.AccessibilityManager) {
                window.AccessibilityManager.announceToScreenReader(
                    `Notifications panel opened. ${this.unreadCount} unread notifications.`,
                    'polite'
                );
            }
        },

        // Close notification dropdown
        closeNotificationDropdown: function() {
            if (!this.notificationDropdown) {
                return;
            }

            this.notificationDropdown.classList.remove('show');
        },

        // Update notification list UI
        updateNotificationList: function() {
            if (!this.notificationList) {
                return;
            }

            // Clear existing list
            this.notificationList.innerHTML = '';

            if (this.notifications.length === 0) {
                this.notificationList.innerHTML = `
                    <div class="notification-item text-center text-muted py-3">
                        No notifications
                    </div>
                `;
                return;
            }

            // Add notifications to list
            this.notifications.forEach(notification => {
                const notificationElement = this.createNotificationElement(notification);
                this.notificationList.appendChild(notificationElement);
            });
        },

        // Create notification HTML element
        createNotificationElement: function(notification) {
            const div = document.createElement('div');
            div.className = `notification-item ${!notification.isRead ? 'unread' : ''}`;
            div.dataset.notificationId = notification.id;

            const icon = this.getNotificationIcon(notification.type);
            const formattedDate = this.formatDate(notification.createdAt);

            div.innerHTML = `
                <div class="notification-icon">${icon}</div>
                <div class="notification-content">
                    <div class="notification-text">${this.escapeHtml(notification.content)}</div>
                    <div class="notification-time text-muted">${formattedDate}</div>
                </div>
                ${!notification.isRead ? `
                    <button class="btn btn-sm btn-link mark-read-btn" data-notification-id="${notification.id}" aria-label="Mark as read">
                        <i class="bi bi-check"></i>
                    </button>
                ` : ''}
            `;

            // Setup mark as read button
            const markReadBtn = div.querySelector('.mark-read-btn');
            if (markReadBtn) {
                markReadBtn.addEventListener('click', async (e) => {
                    e.stopPropagation();
                    await this.markAsRead(notification.id);
                });
            }

            // Make notification clickable to navigate to related content
            div.addEventListener('click', () => {
                this.handleNotificationClick(notification);
            });

            return div;
        },

        // Get notification icon based on type
        getNotificationIcon: function(type) {
            const icons = {
                'NewComment': '<i class="bi bi-chat-dots text-primary"></i>',
                'CommentReply': '<i class="bi bi-reply text-info"></i>',
                'NewReaction': '<i class="bi bi-heart text-danger"></i>'
            };

            return icons[type] || '<i class="bi bi-bell text-secondary"></i>';
        },

        // Handle notification click
        handleNotificationClick: function(notification) {
            // Mark as read if unread
            if (!notification.isRead) {
                this.markAsRead(notification.id);
            }

            // Navigate to related content if available
            if (notification.relatedEntityId) {
                // This would navigate to the blog post or comment
                // Implementation depends on your routing structure
                console.log('Navigate to:', notification.relatedEntityId);
            }

            // Close dropdown
            this.closeNotificationDropdown();
        },

        // Mark notification as read
        markAsRead: async function(notificationId) {
            try {
                const response = await fetch(`/api/notifications/${notificationId}/read`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': this.getAntiForgeryToken()
                    }
                });

                if (!response.ok) {
                    throw new Error('Failed to mark notification as read');
                }

                // Update local state
                const notification = this.notifications.find(n => n.id === notificationId);
                if (notification) {
                    notification.isRead = true;
                }

                // Update UI
                this.updateNotificationList();
                this.updateUnreadCount();

            } catch (error) {
                console.error('Error marking notification as read:', error);
            }
        },

        // Update unread count
        updateUnreadCount: function() {
            // Count unread notifications
            this.unreadCount = this.notifications.filter(n => !n.isRead).length;

            // Update badge
            if (this.notificationBadge) {
                if (this.unreadCount > 0) {
                    this.notificationBadge.textContent = this.unreadCount > 99 ? '99+' : this.unreadCount;
                    this.notificationBadge.style.display = 'inline-block';
                    
                    // Animate badge
                    this.animateBadge();
                } else {
                    this.notificationBadge.style.display = 'none';
                }
            }

            // Update page title
            this.updatePageTitle();
        },

        // Animate notification badge
        animateBadge: function() {
            if (!this.notificationBadge) {
                return;
            }

            // Check if animations should be disabled
            if (window.AccessibilityManager && window.AccessibilityManager.shouldDisableAnimations()) {
                return;
            }

            this.notificationBadge.classList.add('notification-badge');
            
            // Remove animation class after animation completes
            setTimeout(() => {
                this.notificationBadge.classList.remove('notification-badge');
            }, 500);
        },

        // Update page title with unread count
        updatePageTitle: function() {
            const baseTitle = document.title.replace(/^\(\d+\)\s*/, '');
            
            if (this.unreadCount > 0) {
                document.title = `(${this.unreadCount}) ${baseTitle}`;
            } else {
                document.title = baseTitle;
            }
        },

        // Listen for real-time notification updates from SignalR
        listenForNotificationUpdates: function() {
            // Listen for new notifications
            window.addEventListener('notificationReceived', (event) => {
                const notification = event.detail;
                console.log('Real-time notification received:', notification);

                // Add to notifications array
                this.notifications.unshift(notification);

                // Update UI
                this.updateNotificationList();
                this.updateUnreadCount();

                // Show toast notification
                this.showNotificationToast(notification);

                // Trigger Lordicon animation
                this.triggerBellAnimation();

                // Dispatch custom event for new notification
                window.dispatchEvent(new CustomEvent('newNotification'));
            });

            // Listen for notification read events
            window.addEventListener('notificationRead', (event) => {
                const { notificationId } = event.detail;
                console.log('Notification marked as read:', notificationId);

                // Update local state
                const notification = this.notifications.find(n => n.id === notificationId);
                if (notification) {
                    notification.isRead = true;
                }

                // Update UI
                this.updateNotificationList();
                this.updateUnreadCount();
            });
        },

        // Show notification toast popup
        showNotificationToast: function(notification) {
            const toast = document.createElement('div');
            toast.className = 'notification-toast';
            toast.setAttribute('role', 'alert');
            toast.setAttribute('aria-live', 'assertive');

            const icon = this.getNotificationIcon(notification.type);

            toast.innerHTML = `
                <div class="notification-toast-icon">${icon}</div>
                <div class="notification-toast-content">
                    <div class="notification-toast-title">New Notification</div>
                    <div class="notification-toast-text">${this.escapeHtml(notification.content)}</div>
                </div>
                <button class="notification-toast-close" aria-label="Close">
                    <i class="bi bi-x"></i>
                </button>
            `;

            document.body.appendChild(toast);

            // Setup close button
            const closeBtn = toast.querySelector('.notification-toast-close');
            closeBtn.addEventListener('click', () => {
                this.hideNotificationToast(toast);
            });

            // Show toast
            setTimeout(() => {
                toast.classList.add('show');
            }, 10);

            // Auto-hide after 5 seconds
            setTimeout(() => {
                this.hideNotificationToast(toast);
            }, 5000);

            // Make toast clickable
            toast.addEventListener('click', () => {
                this.handleNotificationClick(notification);
                this.hideNotificationToast(toast);
            });

            // Announce to screen readers
            if (window.AccessibilityManager) {
                window.AccessibilityManager.announceToScreenReader(
                    `New notification: ${notification.content}`,
                    'assertive'
                );
            }
        },

        // Hide notification toast
        hideNotificationToast: function(toast) {
            toast.classList.remove('show');
            setTimeout(() => {
                toast.remove();
            }, 300);
        },

        // Trigger bell animation
        triggerBellAnimation: function() {
            if (!this.notificationBell) {
                return;
            }

            // Check if animations should be disabled
            if (window.AccessibilityManager && window.AccessibilityManager.shouldDisableAnimations()) {
                return;
            }

            // Trigger Lordicon animation
            const lordIcon = this.notificationBell.querySelector('lord-icon');
            if (lordIcon) {
                lordIcon.setAttribute('trigger', 'loop');
                setTimeout(() => {
                    lordIcon.setAttribute('trigger', 'hover');
                }, 2000);
            }

            // Add shake animation
            this.notificationBell.classList.add('bell-shake');
            setTimeout(() => {
                this.notificationBell.classList.remove('bell-shake');
            }, 500);
        },

        // Start polling for unread count (fallback)
        startUnreadCountPolling: function() {
            // Poll every 30 seconds
            setInterval(async () => {
                try {
                    const response = await fetch('/api/notifications/unread-count', {
                        headers: {
                            'RequestVerificationToken': this.getAntiForgeryToken()
                        }
                    });

                    if (response.ok) {
                        const data = await response.json();
                        const serverUnreadCount = data.count;

                        // Update if different from local count
                        if (serverUnreadCount !== this.unreadCount) {
                            this.loadNotifications();
                        }
                    }
                } catch (error) {
                    console.error('Error polling unread count:', error);
                }
            }, 30000);
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
                return `${diffMins}m ago`;
            } else if (diffHours < 24) {
                return `${diffHours}h ago`;
            } else if (diffDays < 7) {
                return `${diffDays}d ago`;
            } else {
                return date.toLocaleDateString();
            }
        },

        // Escape HTML to prevent XSS
        escapeHtml: function(text) {
            const div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }
    };

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            NotificationManager.init();
        });
    } else {
        NotificationManager.init();
    }

    // Export for global access
    window.NotificationManager = NotificationManager;

})();
