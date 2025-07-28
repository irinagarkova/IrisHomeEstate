// Generic property deletion functionality
window.PropertyDeletion = {
    propertyToDelete: null,
    initialize: function (deleteUrl) {
        const self = this;

        window.deleteProperty = function (id, title) {
            self.propertyToDelete = id;
            const titleElement = document.getElementById('propertyTitle');
            if (titleElement) {
                titleElement.textContent = title;
            }

            const modalElement = document.getElementById('deleteModal');
            if (modalElement) {
                const modal = new bootstrap.Modal(modalElement);
                modal.show();
            }
        };

        const confirmBtn = document.getElementById('confirmDeleteBtn');
        if (confirmBtn) {
            confirmBtn.addEventListener('click', function () {
                if (self.propertyToDelete) {
                    const form = document.createElement('form');
                    form.method = 'POST';
                    form.action = deleteUrl;

                    const idInput = document.createElement('input');
                    idInput.type = 'hidden';
                    idInput.name = 'id';
                    idInput.value = self.propertyToDelete;
                    form.appendChild(idInput);

                    const token = document.querySelector('input[name="__RequestVerificationToken"]');
                    if (token) {
                        const tokenInput = document.createElement('input');
                        tokenInput.type = 'hidden';
                        tokenInput.name = '__RequestVerificationToken';
                        tokenInput.value = token.value;
                        form.appendChild(tokenInput);
                    }

                    document.body.appendChild(form);
                    form.submit();
                }
            });
        }
    }
};

// ========================================
// GLOBAL UI COMPONENTS
// ========================================

// Tab switching functionality (за Homepage и други)
function initializeTabs() {
    const tabs = document.querySelectorAll('.tab');
    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            tabs.forEach(t => t.classList.remove('active'));
            tab.classList.add('active');
        });
    });
}

// Smooth scrolling for anchor links
function initializeSmoothScrolling() {
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

// Back to Top functionality
function initializeBackToTop() {
    const backToTopBtn = document.getElementById('backToTopBtn');

    if (backToTopBtn) {
        function checkScroll() {
            if (window.scrollY > 300) {
                backToTopBtn.classList.add('show');
            } else {
                backToTopBtn.classList.remove('show');
            }
        }

        window.addEventListener('scroll', checkScroll);

        backToTopBtn.addEventListener('click', function (e) {
            e.preventDefault();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });

        // Initial check
        checkScroll();
    }
}

// ========================================
// INITIALIZATION
// ========================================
document.addEventListener('DOMContentLoaded', function () {
    // Initialize all global functionality
    initializeTabs();
    initializeSmoothScrolling();
    initializeBackToTop();
    initializeSearchForm();
});