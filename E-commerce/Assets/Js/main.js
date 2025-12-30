/**
 * E-commerce Platform - Main JavaScript
 * Interactive features and enhancements
 */

(function() {
    'use strict';

    // DOM Ready
    document.addEventListener('DOMContentLoaded', function() {
        initScrollToTop();
        initMobileMenu();
        initHeaderScroll();
        initImageLazyLoad();
        initSmoothScroll();
        initFormValidation();
        initProductCardAnimations();
        initCounterAnimations();
    });

    /**
     * Scroll to Top Button
     */
    function initScrollToTop() {
        // Create button if it doesn't exist
        let scrollBtn = document.querySelector('.scroll-to-top');
        if (!scrollBtn) {
            scrollBtn = document.createElement('button');
            scrollBtn.className = 'scroll-to-top';
            scrollBtn.innerHTML = '<i class="fas fa-arrow-up"></i>';
            scrollBtn.setAttribute('aria-label', 'Retour en haut');
            document.body.appendChild(scrollBtn);
        }

        // Show/hide button on scroll
        window.addEventListener('scroll', function() {
            if (window.pageYOffset > 300) {
                scrollBtn.classList.add('show');
            } else {
                scrollBtn.classList.remove('show');
            }
        });

        // Scroll to top on click
        scrollBtn.addEventListener('click', function() {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }

    /**
     * Mobile Menu Toggle
     */
    function initMobileMenu() {
        // Create mobile menu toggle if it doesn't exist
        const nav = document.querySelector('nav');
        if (!nav) return;

        let menuToggle = document.querySelector('.mobile-menu-toggle');
        if (!menuToggle) {
            const navWrapper = document.querySelector('.nav-wrapper');
            if (navWrapper) {
                menuToggle = document.createElement('button');
                menuToggle.className = 'mobile-menu-toggle';
                menuToggle.innerHTML = '<i class="fas fa-bars"></i>';
                menuToggle.setAttribute('aria-label', 'Menu');
                navWrapper.insertBefore(menuToggle, nav);
            }
        }

        if (menuToggle) {
            menuToggle.addEventListener('click', function() {
                nav.classList.toggle('active');
                const icon = menuToggle.querySelector('i');
                if (nav.classList.contains('active')) {
                    icon.classList.remove('fa-bars');
                    icon.classList.add('fa-times');
                } else {
                    icon.classList.remove('fa-times');
                    icon.classList.add('fa-bars');
                }
            });

            // Close menu when clicking outside
            document.addEventListener('click', function(e) {
                if (!nav.contains(e.target) && !menuToggle.contains(e.target)) {
                    nav.classList.remove('active');
                    const icon = menuToggle.querySelector('i');
                    icon.classList.remove('fa-times');
                    icon.classList.add('fa-bars');
                }
            });

            // Close menu when clicking on a link
            const navLinks = nav.querySelectorAll('a');
            navLinks.forEach(function(link) {
                link.addEventListener('click', function() {
                    nav.classList.remove('active');
                    const icon = menuToggle.querySelector('i');
                    icon.classList.remove('fa-times');
                    icon.classList.add('fa-bars');
                });
            });
        }
    }

    /**
     * Header Scroll Effect
     */
    function initHeaderScroll() {
        const header = document.querySelector('header');
        if (!header) return;

        let lastScroll = 0;
        window.addEventListener('scroll', function() {
            const currentScroll = window.pageYOffset;
            
            if (currentScroll > 50) {
                header.classList.add('scrolled');
            } else {
                header.classList.remove('scrolled');
            }
            
            lastScroll = currentScroll;
        });
    }

    /**
     * Lazy Load Images
     */
    function initImageLazyLoad() {
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver(function(entries, observer) {
                entries.forEach(function(entry) {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        if (img.dataset.src) {
                            img.src = img.dataset.src;
                            img.classList.add('fade-in');
                            img.removeAttribute('data-src');
                            observer.unobserve(img);
                        }
                    }
                });
            });

            const lazyImages = document.querySelectorAll('img[data-src]');
            lazyImages.forEach(function(img) {
                imageObserver.observe(img);
            });
        }
    }

    /**
     * Smooth Scroll for Anchor Links
     */
    function initSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(function(anchor) {
            anchor.addEventListener('click', function(e) {
                const href = this.getAttribute('href');
                if (href === '#' || href === '#!') {
                    e.preventDefault();
                    return;
                }
                
                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }

    /**
     * Form Validation Feedback
     */
    function initFormValidation() {
        const forms = document.querySelectorAll('form');
        forms.forEach(function(form) {
            const inputs = form.querySelectorAll('input[required], textarea[required], select[required]');
            
            inputs.forEach(function(input) {
                input.addEventListener('blur', function() {
                    validateField(input);
                });
                
                input.addEventListener('input', function() {
                    if (input.classList.contains('error')) {
                        validateField(input);
                    }
                });
            });
        });
    }

    function validateField(field) {
        const value = field.value.trim();
        const isValid = field.checkValidity();
        
        // Remove previous error styling
        field.classList.remove('error', 'valid');
        
        if (value !== '') {
            if (isValid) {
                field.classList.add('valid');
            } else {
                field.classList.add('error');
            }
        }
    }

    /**
     * Product Card Animations
     */
    function initProductCardAnimations() {
        if ('IntersectionObserver' in window) {
            const cardObserver = new IntersectionObserver(function(entries) {
                entries.forEach(function(entry, index) {
                    if (entry.isIntersecting) {
                        setTimeout(function() {
                            entry.target.style.opacity = '1';
                            entry.target.style.transform = 'translateY(0)';
                        }, index * 100);
                        cardObserver.unobserve(entry.target);
                    }
                });
            }, {
                threshold: 0.1
            });

            const cards = document.querySelectorAll('.product-card, .category-card');
            cards.forEach(function(card) {
                card.style.opacity = '0';
                card.style.transform = 'translateY(30px)';
                card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
                cardObserver.observe(card);
            });
        }
    }

    /**
     * Counter Animations
     */
    function initCounterAnimations() {
        function animateCounter(element, target, duration = 2000) {
            const start = 0;
            const increment = target / (duration / 16);
            let current = start;

            const timer = setInterval(function() {
                current += increment;
                if (current >= target) {
                    element.textContent = Math.round(target);
                    clearInterval(timer);
                } else {
                    element.textContent = Math.round(current);
                }
            }, 16);
        }

        if ('IntersectionObserver' in window) {
            const counterObserver = new IntersectionObserver(function(entries) {
                entries.forEach(function(entry) {
                    if (entry.isIntersecting) {
                        const element = entry.target;
                        const target = parseInt(element.getAttribute('data-target') || element.textContent);
                        if (!isNaN(target)) {
                            element.textContent = '0';
                            animateCounter(element, target);
                            counterObserver.unobserve(element);
                        }
                    }
                });
            });

            document.querySelectorAll('[data-counter]').forEach(function(counter) {
                counterObserver.observe(counter);
            });
        }
    }

    /**
     * Add to Cart Animation
     */
    window.addToCartAnimation = function(button) {
        if (!button) return;
        
        const cartIcon = document.querySelector('.nav-links .fa-shopping-cart');
        if (!cartIcon) return;

        const buttonRect = button.getBoundingClientRect();
        const cartRect = cartIcon.getBoundingClientRect();

        // Create animation element
        const animElement = document.createElement('div');
        animElement.style.cssText = `
            position: fixed;
            left: ${buttonRect.left + buttonRect.width / 2}px;
            top: ${buttonRect.top + buttonRect.height / 2}px;
            width: 30px;
            height: 30px;
            background: var(--primary-color);
            border-radius: 50%;
            pointer-events: none;
            z-index: 10000;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 16px;
            transition: all 0.6s cubic-bezier(0.4, 0, 0.2, 1);
        `;
        animElement.innerHTML = '<i class="fas fa-shopping-cart"></i>';
        document.body.appendChild(animElement);

        // Animate to cart
        setTimeout(function() {
            animElement.style.left = cartRect.left + cartRect.width / 2 + 'px';
            animElement.style.top = cartRect.top + cartRect.height / 2 + 'px';
            animElement.style.transform = 'scale(0.3)';
            animElement.style.opacity = '0';
        }, 10);

        // Remove element after animation
        setTimeout(function() {
            animElement.remove();
        }, 600);

        // Animate cart badge
        const cartBadge = cartIcon.parentElement.querySelector('.badge-count');
        if (cartBadge) {
            cartBadge.style.animation = 'none';
            setTimeout(function() {
                cartBadge.style.animation = 'pulse-badge 0.5s ease';
            }, 10);
        }
    };

    /**
     * Image Zoom Modal
     */
    window.initImageZoom = function(image) {
        if (!image) return;

        image.addEventListener('click', function() {
            const modal = document.createElement('div');
            modal.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0, 0, 0, 0.9);
                z-index: 10000;
                display: flex;
                align-items: center;
                justify-content: center;
                cursor: zoom-out;
                animation: fadeIn 0.3s ease;
            `;

            const img = document.createElement('img');
            img.src = image.src;
            img.style.cssText = `
                max-width: 90%;
                max-height: 90%;
                object-fit: contain;
                animation: zoomIn 0.3s ease;
            `;

            modal.appendChild(img);
            document.body.appendChild(modal);
            document.body.style.overflow = 'hidden';

            modal.addEventListener('click', function() {
                modal.style.animation = 'fadeOut 0.3s ease';
                setTimeout(function() {
                    modal.remove();
                    document.body.style.overflow = '';
                }, 300);
            });
        });
    };

    // Initialize image zoom on product images
    document.addEventListener('DOMContentLoaded', function() {
        document.querySelectorAll('.product-card img, .product-gallery img').forEach(function(img) {
            window.initImageZoom(img);
        });
    });

    /**
     * Search Autocomplete Enhancement
     */
    window.initSearchAutocomplete = function(inputElement) {
        if (!inputElement) return;

        let timeout;
        inputElement.addEventListener('input', function() {
            clearTimeout(timeout);
            const query = this.value.trim();

            if (query.length < 2) {
                hideAutocomplete();
                return;
            }

            timeout = setTimeout(function() {
                // Here you would typically make an AJAX call to get suggestions
                // For now, we'll just show a loading state
                console.log('Search query:', query);
            }, 300);
        });

        function hideAutocomplete() {
            const dropdown = document.querySelector('.search-autocomplete');
            if (dropdown) {
                dropdown.remove();
            }
        }
    };

    /**
     * Quantity Input Controls
     */
    window.initQuantityControls = function(container) {
        if (!container) return;

        const decreaseBtn = container.querySelector('.qty-decrease');
        const increaseBtn = container.querySelector('.qty-increase');
        const input = container.querySelector('.qty-input');

        if (decreaseBtn && input) {
            decreaseBtn.addEventListener('click', function() {
                const current = parseInt(input.value) || 1;
                if (current > 1) {
                    input.value = current - 1;
                    triggerInputChange(input);
                }
            });
        }

        if (increaseBtn && input) {
            increaseBtn.addEventListener('click', function() {
                const current = parseInt(input.value) || 1;
                const max = parseInt(input.getAttribute('max')) || 999;
                if (current < max) {
                    input.value = current + 1;
                    triggerInputChange(input);
                }
            });
        }
    };

    function triggerInputChange(input) {
        const event = new Event('change', { bubbles: true });
        input.dispatchEvent(event);
    }

    // Initialize quantity controls on page load
    document.addEventListener('DOMContentLoaded', function() {
        document.querySelectorAll('.qty-controls').forEach(function(container) {
            window.initQuantityControls(container);
        });
    });

})();

