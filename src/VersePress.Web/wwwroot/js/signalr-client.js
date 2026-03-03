// SignalR client connections for VersePress
// Manages real-time connections to NotificationHub and InteractionHub
// Requirements: 4.2, 5.4, 19.1

(function() {
    'use strict';

    // SignalR Connection Manager
    const SignalRManager = {
        // Connection instances
        notificationConnection: null,
        interactionConnection: null,

        // Connection state
        isNotificationConnected: false,
        isInteractionConnected: false,

        // Current blog post ID for interaction hub
        currentBlogPostId: null,

        // Reconnection settings
        reconnectAttempts: 0,
        maxReconnectAttempts: 5,
        reconnectDelay: 2000,

        // Initialize SignalR connections
        init: function() {
            console.log('Initializing SignalR connections...');
            
            // Initialize notification hub (for authenticated users)
            if (this.isUserAuthenticated()) {
                this.initNotificationHub();
            }

            // Initialize interaction hub (for all users)
            this.initInteractionHub();

            // Setup connection status UI
            this.setupConnectionStatusUI();
        },

        // Check if user is authenticated
        isUserAuthenticated: function() {
            // Check for authentication cookie or meta tag
            const authMeta = document.querySelector('meta[name="user-authenticated"]');
            return authMeta && authMeta.content === 'true';
        },

        // Initialize NotificationHub connection
        initNotificationHub: function() {
            try {
                // Build connection
                this.notificationConnection = new signalR.HubConnectionBuilder()
                    .withUrl('/hubs/notifications')
                    .withAutomaticReconnect({
                        nextRetryDelayInMilliseconds: retryContext => {
                            // Exponential backoff: 0, 2, 10, 30 seconds
                            if (retryContext.previousRetryCount === 0) return 0;
                            if (retryContext.previousRetryCount === 1) return 2000;
                            if (retryContext.previousRetryCount === 2) return 10000;
                            return 30000;
                        }
                    })
                    .configureLogging(signalR.LogLevel.Information)
                    .build();

                // Setup event handlers
                this.setupNotificationHandlers();

                // Start connection
                this.startNotificationConnection();

            } catch (error) {
                console.error('Error initializing NotificationHub:', error);
                this.updateConnectionStatus('notification', 'error');
            }
        },

        // Initialize InteractionHub connection
        initInteractionHub: function() {
            try {
                // Build connection
                this.interactionConnection = new signalR.HubConnectionBuilder()
                    .withUrl('/hubs/interactions')
                    .withAutomaticReconnect({
                        nextRetryDelayInMilliseconds: retryContext => {
                            if (retryContext.previousRetryCount === 0) return 0;
                            if (retryContext.previousRetryCount === 1) return 2000;
                            if (retryContext.previousRetryCount === 2) return 10000;
                            return 30000;
                        }
                    })
                    .configureLogging(signalR.LogLevel.Information)
                    .build();

                // Setup event handlers
                this.setupInteractionHandlers();

                // Start connection
                this.startInteractionConnection();

            } catch (error) {
                console.error('Error initializing InteractionHub:', error);
                this.updateConnectionStatus('interaction', 'error');
            }
        },

        // Setup NotificationHub event handlers
        setupNotificationHandlers: function() {
            const conn = this.notificationConnection;

            // Connection lifecycle events
            conn.onclose(error => {
                this.isNotificationConnected = false;
                this.updateConnectionStatus('notification', 'disconnected');
                console.log('NotificationHub disconnected', error);
                
                if (error) {
                    console.error('NotificationHub connection closed with error:', error);
                }
            });

            conn.onreconnecting(error => {
                this.isNotificationConnected = false;
                this.updateConnectionStatus('notification', 'reconnecting');
                console.log('NotificationHub reconnecting...', error);
            });

            conn.onreconnected(connectionId => {
                this.isNotificationConnected = true;
                this.updateConnectionStatus('notification', 'connected');
                console.log('NotificationHub reconnected:', connectionId);
            });

            // Receive notification event
            conn.on('ReceiveNotification', (notification) => {
                console.log('Notification received:', notification);
                this.handleNotificationReceived(notification);
            });

            // Notification read event
            conn.on('NotificationRead', (notificationId) => {
                console.log('Notification marked as read:', notificationId);
                this.handleNotificationRead(notificationId);
            });
        },

        // Setup InteractionHub event handlers
        setupInteractionHandlers: function() {
            const conn = this.interactionConnection;

            // Connection lifecycle events
            conn.onclose(error => {
                this.isInteractionConnected = false;
                this.updateConnectionStatus('interaction', 'disconnected');
                console.log('InteractionHub disconnected', error);
                
                if (error) {
                    console.error('InteractionHub connection closed with error:', error);
                }
            });

            conn.onreconnecting(error => {
                this.isInteractionConnected = false;
                this.updateConnectionStatus('interaction', 'reconnecting');
                console.log('InteractionHub reconnecting...', error);
            });

            conn.onreconnected(connectionId => {
                this.isInteractionConnected = true;
                this.updateConnectionStatus('interaction', 'connected');
                console.log('InteractionHub reconnected:', connectionId);
                
                // Rejoin post group if we were viewing a post
                if (this.currentBlogPostId) {
                    this.joinPostGroup(this.currentBlogPostId);
                }
            });

            // Reaction updated event
            conn.on('ReactionUpdated', (reaction) => {
                console.log('Reaction updated:', reaction);
                this.handleReactionUpdated(reaction);
            });

            // Comment added event
            conn.on('CommentAdded', (comment) => {
                console.log('Comment added:', comment);
                this.handleCommentAdded(comment);
            });
        },

        // Start NotificationHub connection
        startNotificationConnection: async function() {
            try {
                await this.notificationConnection.start();
                this.isNotificationConnected = true;
                this.updateConnectionStatus('notification', 'connected');
                console.log('NotificationHub connected successfully');
            } catch (error) {
                console.error('Error starting NotificationHub:', error);
                this.updateConnectionStatus('notification', 'error');
                
                // Retry connection
                this.scheduleReconnect('notification');
            }
        },

        // Start InteractionHub connection
        startInteractionConnection: async function() {
            try {
                await this.interactionConnection.start();
                this.isInteractionConnected = true;
                this.updateConnectionStatus('interaction', 'connected');
                console.log('InteractionHub connected successfully');
                
                // Join post group if on a blog post page
                const postIdMeta = document.querySelector('meta[name="blog-post-id"]');
                if (postIdMeta && postIdMeta.content) {
                    this.joinPostGroup(postIdMeta.content);
                }
            } catch (error) {
                console.error('Error starting InteractionHub:', error);
                this.updateConnectionStatus('interaction', 'error');
                
                // Retry connection
                this.scheduleReconnect('interaction');
            }
        },

        // Schedule reconnection attempt
        scheduleReconnect: function(hubType) {
            if (this.reconnectAttempts >= this.maxReconnectAttempts) {
                console.error(`Max reconnection attempts reached for ${hubType}`);
                this.updateConnectionStatus(hubType, 'failed');
                return;
            }

            this.reconnectAttempts++;
            const delay = this.reconnectDelay * this.reconnectAttempts;

            console.log(`Scheduling ${hubType} reconnection attempt ${this.reconnectAttempts} in ${delay}ms`);

            setTimeout(() => {
                if (hubType === 'notification') {
                    this.startNotificationConnection();
                } else {
                    this.startInteractionConnection();
                }
            }, delay);
        },

        // Join a blog post group for real-time updates
        joinPostGroup: async function(blogPostId) {
            if (!this.isInteractionConnected) {
                console.warn('Cannot join post group: InteractionHub not connected');
                return;
            }

            try {
                await this.interactionConnection.invoke('JoinPostGroup', blogPostId);
                this.currentBlogPostId = blogPostId;
                console.log('Joined post group:', blogPostId);
            } catch (error) {
                console.error('Error joining post group:', error);
            }
        },

        // Leave a blog post group
        leavePostGroup: async function(blogPostId) {
            if (!this.isInteractionConnected) {
                return;
            }

            try {
                await this.interactionConnection.invoke('LeavePostGroup', blogPostId);
                if (this.currentBlogPostId === blogPostId) {
                    this.currentBlogPostId = null;
                }
                console.log('Left post group:', blogPostId);
            } catch (error) {
                console.error('Error leaving post group:', error);
            }
        },

        // Handle notification received
        handleNotificationReceived: function(notification) {
            // Dispatch custom event for notification components to handle
            const event = new CustomEvent('notificationReceived', {
                detail: notification
            });
            window.dispatchEvent(event);
        },

        // Handle notification read
        handleNotificationRead: function(notificationId) {
            // Dispatch custom event
            const event = new CustomEvent('notificationRead', {
                detail: { notificationId }
            });
            window.dispatchEvent(event);
        },

        // Handle reaction updated
        handleReactionUpdated: function(reaction) {
            // Dispatch custom event for reaction components to handle
            const event = new CustomEvent('reactionUpdated', {
                detail: reaction
            });
            window.dispatchEvent(event);
        },

        // Handle comment added
        handleCommentAdded: function(comment) {
            // Dispatch custom event for comment components to handle
            const event = new CustomEvent('commentAdded', {
                detail: comment
            });
            window.dispatchEvent(event);
        },

        // Setup connection status UI
        setupConnectionStatusUI: function() {
            // Create status indicator if it doesn't exist
            if (!document.getElementById('signalr-status')) {
                const statusDiv = document.createElement('div');
                statusDiv.id = 'signalr-status';
                statusDiv.className = 'signalr-status';
                statusDiv.setAttribute('role', 'status');
                statusDiv.setAttribute('aria-live', 'polite');
                document.body.appendChild(statusDiv);
            }
        },

        // Update connection status in UI
        updateConnectionStatus: function(hubType, status) {
            const statusDiv = document.getElementById('signalr-status');
            if (!statusDiv) return;

            // Update status indicator
            const statusClass = `signalr-status-${status}`;
            statusDiv.className = `signalr-status ${statusClass}`;

            // Update status text
            let statusText = '';
            switch (status) {
                case 'connected':
                    statusText = '';
                    statusDiv.style.display = 'none';
                    break;
                case 'disconnected':
                    statusText = 'Disconnected - Attempting to reconnect...';
                    statusDiv.style.display = 'block';
                    break;
                case 'reconnecting':
                    statusText = 'Reconnecting...';
                    statusDiv.style.display = 'block';
                    break;
                case 'error':
                    statusText = 'Connection error - Please refresh the page';
                    statusDiv.style.display = 'block';
                    break;
                case 'failed':
                    statusText = 'Connection failed - Please refresh the page';
                    statusDiv.style.display = 'block';
                    break;
            }

            statusDiv.textContent = statusText;

            // Announce to screen readers
            if (window.AccessibilityManager && statusText) {
                window.AccessibilityManager.announceToScreenReader(statusText, 'polite');
            }
        },

        // Disconnect all connections
        disconnect: async function() {
            try {
                if (this.notificationConnection) {
                    await this.notificationConnection.stop();
                }
                if (this.interactionConnection) {
                    await this.interactionConnection.stop();
                }
                console.log('SignalR connections closed');
            } catch (error) {
                console.error('Error disconnecting SignalR:', error);
            }
        }
    };

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            SignalRManager.init();
        });
    } else {
        SignalRManager.init();
    }

    // Cleanup on page unload
    window.addEventListener('beforeunload', function() {
        SignalRManager.disconnect();
    });

    // Export for global access
    window.SignalRManager = SignalRManager;

})();
