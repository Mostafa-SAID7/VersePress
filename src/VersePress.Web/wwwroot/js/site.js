// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Mobile performance optimizations
(function() {
    'use strict';

    // Detect if user is on mobile device
    const isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    const isSlowConnection = navigator.connection && (navigator.connection.effectiveType === 'slow-2g' || navigator.connection.effectiveType === '2g');

    // Lazy load images
    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    if (img.dataset.src) {
                        img.src = img.dataset.src;
                        img.removeAttribute('data-src');
                    }
                    if (img.dataset.srcset) {
                        img.srcset = img.dataset.srcset;
                        img.removeAttribute('data-srcset');
                    }
                    img.classList.remove('lazy');
                    observer.unobserve(img);
                }
            });
        }, {
            rootMargin: '50px 0px',
            threshold: 0.01
        });

        // Observe all images with lazy class
        document.addEventListener('DOMContentLoaded', () => {
            document.querySelectorAll('img.lazy').forEach(img => {
                imageObserver.observe(img);
            });
        });
    }

    // Optimize animations on mobile
    if (isMobile || isSlowConnection) {
        document.documentElement.classList.add('reduce-motion');
    }

    // Debounce function for scroll and resize events
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    // Optimize scroll events on mobile
    if (isMobile) {
        let ticking = false;
        window.addEventListener('scroll', () => {
            if (!ticking) {
                window.requestAnimationFrame(() => {
                    // Handle scroll events here
                    ticking = false;
                });
                ticking = true;
            }
        }, { passive: true });
    }

    // Preload critical resources on fast connections
    if (!isSlowConnection && 'requestIdleCallback' in window) {
        requestIdleCallback(() => {
            // Preload fonts or other critical resources
            const link = document.createElement('link');
            link.rel = 'preload';
            link.as = 'font';
            link.crossOrigin = 'anonymous';
            // Add font preloading if needed
        });
    }

    // Reduce image quality on slow connections
    if (isSlowConnection) {
        document.querySelectorAll('img').forEach(img => {
            if (img.dataset.lowQualitySrc) {
                img.src = img.dataset.lowQualitySrc;
            }
        });
    }

    // Touch event optimization
    if ('ontouchstart' in window) {
        document.body.classList.add('touch-device');
        
        // Add touch feedback to buttons
        document.addEventListener('touchstart', (e) => {
            if (e.target.matches('.btn, .nav-link, .card')) {
                e.target.classList.add('touch-active');
            }
        }, { passive: true });

        document.addEventListener('touchend', (e) => {
            if (e.target.matches('.btn, .nav-link, .card')) {
                setTimeout(() => {
                    e.target.classList.remove('touch-active');
                }, 150);
            }
        }, { passive: true });
    }

    // Service Worker registration for offline support (optional)
    if ('serviceWorker' in navigator && !isMobile) {
        // Only register on desktop to save mobile resources
        // window.addEventListener('load', () => {
        //     navigator.serviceWorker.register('/sw.js');
        // });
    }

    // Performance monitoring
    if (window.performance && window.performance.timing) {
        window.addEventListener('load', () => {
            setTimeout(() => {
                const perfData = window.performance.timing;
                const pageLoadTime = perfData.loadEventEnd - perfData.navigationStart;
                
                // Log performance metrics (can be sent to analytics)
                if (pageLoadTime > 3000) {
                    console.warn('Page load time exceeded 3 seconds:', pageLoadTime + 'ms');
                }
            }, 0);
        });
    }

    // Optimize form inputs on mobile
    if (isMobile) {
        document.addEventListener('DOMContentLoaded', () => {
            // Prevent zoom on input focus for iOS
            document.querySelectorAll('input, textarea, select').forEach(input => {
                if (input.style.fontSize === '' || parseFloat(input.style.fontSize) < 16) {
                    input.style.fontSize = '16px';
                }
            });
        });
    }

})();


// ============================================
// MOBILE SIDEBAR
// ============================================

(function() {
    'use strict';

    function initMobileSidebar() {
        const mobileSidebarToggle = document.getElementById('mobileSidebarToggle');
        const mobileSidebar = document.getElementById('mobileSidebar');
        const mobileSidebarOverlay = document.getElementById('mobileSidebarOverlay');
        const mobileSidebarClose = document.getElementById('mobileSidebarClose');

        if (!mobileSidebarToggle || !mobileSidebar || !mobileSidebarOverlay) {
            return;
        }

        function openSidebar() {
            mobileSidebar.classList.add('show');
            mobileSidebarOverlay.classList.add('show');
            document.body.classList.add('mobile-sidebar-open');
        }

        function closeSidebar() {
            mobileSidebar.classList.remove('show');
            mobileSidebarOverlay.classList.remove('show');
            document.body.classList.remove('mobile-sidebar-open');
        }

        // Toggle sidebar
        mobileSidebarToggle.addEventListener('click', function(e) {
            e.preventDefault();
            openSidebar();
        });

        // Close sidebar
        if (mobileSidebarClose) {
            mobileSidebarClose.addEventListener('click', function(e) {
                e.preventDefault();
                closeSidebar();
            });
        }

        // Close on overlay click
        mobileSidebarOverlay.addEventListener('click', function() {
            closeSidebar();
        });

        // Close on escape key
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape' && mobileSidebar.classList.contains('show')) {
                closeSidebar();
            }
        });

        // Close sidebar when clicking on a link
        const sidebarLinks = mobileSidebar.querySelectorAll('.mobile-sidebar-link');
        sidebarLinks.forEach(function(link) {
            link.addEventListener('click', function() {
                closeSidebar();
            });
        });

        // Mark active link
        const currentPath = window.location.pathname;
        sidebarLinks.forEach(function(link) {
            if (link.getAttribute('href') === currentPath) {
                link.classList.add('active');
            }
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initMobileSidebar);
    } else {
        initMobileSidebar();
    }
})();
