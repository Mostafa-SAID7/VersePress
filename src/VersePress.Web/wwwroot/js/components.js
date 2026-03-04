// ============================================
// COMPONENTS JAVASCRIPT
// ============================================

(function() {
    'use strict';

    // Initialize all components when DOM is ready
    function initComponents() {
        initTooltips();
        initPopovers();
        initAlerts();
    }

    // Initialize Bootstrap tooltips
    function initTooltips() {
        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
        if (tooltipTriggerList.length > 0 && typeof bootstrap !== 'undefined') {
            [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
        }
    }

    // Initialize Bootstrap popovers
    function initPopovers() {
        const popoverTriggerList = document.querySelectorAll('[data-bs-toggle="popover"]');
        if (popoverTriggerList.length > 0 && typeof bootstrap !== 'undefined') {
            [...popoverTriggerList].map(popoverTriggerEl => new bootstrap.Popover(popoverTriggerEl));
        }
    }

    // Initialize auto-dismissing alerts
    function initAlerts() {
        const alerts = document.querySelectorAll('.alert[data-auto-dismiss]');
        alerts.forEach(alert => {
            const delay = parseInt(alert.dataset.autoDismiss) || 5000;
            setTimeout(() => {
                if (typeof bootstrap !== 'undefined') {
                    const bsAlert = new bootstrap.Alert(alert);
                    bsAlert.close();
                } else {
                    alert.remove();
                }
            }, delay);
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initComponents);
    } else {
        initComponents();
    }
})();
