<%@ Page Title="Détails du Produit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ProductDetails.aspx.cs" Inherits="Ecommerce.Pages.Public.ProductDetails" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .product-details-container {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 4rem;
                margin-top: 2rem;
                margin-bottom: 3rem;
                align-items: start;
            }

            .product-gallery {
                border-radius: 16px;
                overflow: hidden;
                border: 1px solid #e2e8f0;
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
                transition: all 0.3s ease;
                position: relative;
            }

            .product-gallery:hover {
                box-shadow: 0 8px 30px rgba(0, 0, 0, 0.12);
                transform: translateY(-2px);
            }

            .main-image-container {
                position: relative;
                width: 100%;
                background: linear-gradient(135deg, #f8fafc 0%, #ffffff 100%);
                display: flex;
                align-items: center;
                justify-content: center;
                min-height: 550px;
                padding: 2rem;
                overflow: hidden;
            }

            .main-image {
                width: 100%;
                max-width: 100%;
                height: auto;
                max-height: 550px;
                object-fit: contain;
                background: transparent;
                cursor: zoom-in;
                transition: opacity 0.3s ease !important;
                border-radius: 8px;
                opacity: 1;
                transform: scale(1) !important;
                will-change: opacity;
            }

            .main-image:hover {
                transform: scale(1) !important;
                transition: opacity 0.3s ease !important;
            }

            /* Override any zoom effects from global CSS - very specific selectors */
            .product-gallery .main-image,
            .product-gallery .main-image:hover,
            .product-gallery:hover .main-image,
            .main-image-container .main-image,
            .main-image-container:hover .main-image,
            .image-zoom-container .main-image,
            .image-zoom-container:hover .main-image,
            .product-gallery img.main-image,
            .product-gallery:hover img.main-image {
                transform: scale(1) !important;
                transition: opacity 0.3s ease !important;
            }
            
            /* Prevent any transform on image load or any event */
            .main-image[src],
            .main-image[src]:hover,
            .main-image[src]:focus,
            .main-image[src]:active,
            .product-gallery img[class*="main-image"],
            .product-gallery:hover img[class*="main-image"] {
                transform: scale(1) !important;
                transition: opacity 0.3s ease !important;
            }
            
            /* Completely disable any zoom effect from global CSS */
            .product-gallery .main-image-container img,
            .product-gallery .main-image-container:hover img,
            .product-gallery .main-image-container img:hover {
                transform: scale(1) !important;
                transition: opacity 0.3s ease !important;
            }

            .image-thumbnails {
                display: flex;
                gap: 1rem;
                margin-top: 1.5rem;
                flex-wrap: wrap;
                padding: 1.5rem;
                background: #f8fafc;
                border-top: 1px solid #e2e8f0;
                justify-content: center;
                align-items: center;
            }

            .thumbnail-wrapper {
                position: relative;
                display: inline-block;
            }

            .thumbnail {
                width: 100px;
                height: 100px;
                object-fit: cover;
                border: 3px solid #e2e8f0;
                border-radius: 12px;
                cursor: pointer;
                transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
                background: #ffffff;
                box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
                position: relative;
                overflow: hidden;
            }

            .thumbnail::before {
                content: '';
                position: absolute;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: rgba(40, 167, 69, 0);
                transition: background 0.3s ease;
                border-radius: 12px;
                z-index: 1;
            }

            .thumbnail:hover {
                border-color: var(--primary-color);
                transform: translateY(-5px) scale(1.05);
                box-shadow: 0 8px 20px rgba(40, 167, 69, 0.3);
                z-index: 2;
            }

            .thumbnail:hover::before {
                background: rgba(40, 167, 69, 0.1);
            }

            .thumbnail.active {
                border-color: var(--primary-color);
                border-width: 4px;
                box-shadow: 0 0 0 3px rgba(40, 167, 69, 0.2), 0 4px 12px rgba(40, 167, 69, 0.3);
                transform: translateY(-3px);
            }

            .thumbnail.active::after {
                content: '✓';
                position: absolute;
                top: 5px;
                right: 5px;
                background: var(--primary-color);
                color: white;
                width: 24px;
                height: 24px;
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 12px;
                font-weight: bold;
                z-index: 3;
                box-shadow: 0 2px 6px rgba(0, 0, 0, 0.2);
            }

            .image-zoom-indicator {
                position: absolute;
                top: 20px;
                right: 20px;
                background: linear-gradient(135deg, rgba(40, 167, 69, 0.9), rgba(34, 139, 58, 0.9));
                color: white;
                padding: 10px 16px;
                border-radius: 25px;
                font-size: 13px;
                font-weight: 600;
                display: flex;
                align-items: center;
                gap: 8px;
                opacity: 0;
                transition: all 0.3s ease;
                z-index: 10;
                box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
                backdrop-filter: blur(10px);
            }

            .product-gallery:hover .image-zoom-indicator {
                opacity: 1;
                transform: translateY(0);
            }

            .image-zoom-indicator i {
                font-size: 14px;
            }

            .product-header {
                margin-bottom: 1rem;
            }

            .product-info h1 {
                font-size: 2.5rem;
                margin-bottom: 0.5rem;
                color: var(--text-dark);
                line-height: 1.2;
            }

            .product-sku {
                display: inline-block;
                font-size: 0.85rem;
                color: var(--text-light);
                background: var(--bg-light);
                padding: 0.3rem 0.8rem;
                border-radius: 20px;
                font-weight: 500;
                letter-spacing: 0.5px;
                margin-top: 0.5rem;
            }

            .product-price {
                font-size: 2rem;
                color: var(--primary-color);
                font-weight: 700;
                margin-bottom: 1rem;
            }

            .product-price-compare {
                font-size: 1.2rem;
                color: var(--text-light);
                text-decoration: line-through;
                margin-left: 1rem;
            }

            .product-description {
                color: var(--text-dark);
                margin-bottom: 2rem;
                line-height: 1.9;
                font-size: 1.05rem;
                padding: 1.5rem;
                background: var(--bg-light);
                border-radius: 8px;
                border-left: 4px solid var(--primary-color);
            }

            .product-meta {
                display: flex;
                gap: 2rem;
                margin-bottom: 2rem;
                padding: 1.5rem;
                background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
                border-radius: 10px;
                border: 1px solid var(--border-color);
            }

            .meta-item {
                display: flex;
                flex-direction: column;
                gap: 0.5rem;
                flex: 1;
            }

            .meta-item strong {
                color: var(--text-dark);
                font-size: 0.85rem;
                text-transform: uppercase;
                letter-spacing: 0.5px;
                font-weight: 600;
            }

            .meta-item span {
                color: var(--text-dark);
                font-size: 1rem;
                font-weight: 500;
            }

            .meta-item i {
                color: var(--primary-color);
                margin-right: 0.3rem;
            }

            .options-group {
                margin-bottom: 1.5rem;
            }

            .options-group label {
                display: block;
                margin-bottom: 0.75rem;
                font-weight: 600;
                color: var(--text-dark);
                font-size: 1rem;
                text-transform: uppercase;
                letter-spacing: 0.5px;
                font-size: 0.85rem;
            }

            .form-select {
                width: 100%;
                padding: 12px 15px;
                border: 2px solid var(--border-color);
                border-radius: 8px;
                font-size: 15px;
                background: var(--bg-white);
                cursor: pointer;
                transition: border-color 0.3s ease;
                font-weight: 500;
            }

            .form-select:focus {
                outline: none;
                border-color: var(--primary-color);
                box-shadow: 0 0 0 3px rgba(40, 167, 69, 0.1);
            }

            .qty-input {
                width: 120px;
                padding: 12px 15px;
                border: 2px solid var(--border-color);
                border-radius: 8px;
                font-size: 16px;
                text-align: center;
                font-weight: 600;
                transition: border-color 0.3s ease;
            }

            .qty-input:focus {
                outline: none;
                border-color: var(--primary-color);
                box-shadow: 0 0 0 3px rgba(40, 167, 69, 0.1);
            }

            .actions {
                display: flex;
                gap: 1rem;
                margin-top: 2rem;
                align-items: center;
            }

            .actions .btn {
                transition: all 0.3s ease;
                border-radius: 8px;
                font-weight: 600;
                text-transform: uppercase;
                letter-spacing: 0.5px;
                font-size: 14px;
            }

            .actions .btn:hover {
                transform: translateY(-3px);
                box-shadow: 0 8px 20px rgba(40, 167, 69, 0.4);
            }

            .actions .btn:active {
                transform: translateY(-1px);
            }

            .actions .btn-outline {
                border: 2px solid var(--primary-color);
                color: var(--primary-color);
                background: transparent;
            }

            .actions .btn-outline:hover {
                background: var(--primary-color);
                color: white;
            }

            .actions .btn-outline.active {
                background: var(--primary-color);
                color: white;
                border-color: var(--primary-color);
            }

            .actions .btn-outline.active:hover {
                background: var(--danger-color);
                border-color: var(--danger-color);
            }

            .stock-status {
                padding: 0.6rem 1.2rem;
                border-radius: 25px;
                display: inline-block;
                margin-bottom: 1.5rem;
                font-weight: 600;
                font-size: 0.9rem;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                width: fit-content;
                max-width: 100%;
            }

            .stock-in {
                background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
                color: #155724;
                border: 1px solid #c3e6cb;
            }

            .stock-out {
                background: linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%);
                color: #721c24;
                border: 1px solid #f5c6cb;
            }

            .reviews-section {
                margin-top: 4rem;
                padding-top: 3rem;
                border-top: 2px solid var(--border-color);
            }

            .reviews-section h3 {
                font-size: 2rem;
                color: var(--text-dark);
                margin-bottom: 2rem;
                font-weight: 700;
            }

            .review-item {
                padding: 1.5rem;
                background: var(--bg-white);
                border-radius: 10px;
                margin-bottom: 1rem;
                border: 1px solid var(--border-color);
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
                transition: box-shadow 0.3s ease;
            }

            .review-item:hover {
                box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
                transform: translateX(5px);
                transition: var(--transition);
            }

            .review-header {
                display: flex;
                justify-content: space-between;
                margin-bottom: 0.5rem;
            }

            .review-rating {
                color: var(--warning-color);
            }

            @media (max-width: 768px) {
                .product-details-container {
                    grid-template-columns: 1fr;
                    gap: 2rem;
                }

                .main-image-container {
                    min-height: 400px;
                    padding: 1rem;
                }

                .main-image {
                    max-height: 400px;
                }

                .image-thumbnails {
                    padding: 1rem;
                    gap: 0.75rem;
                }

                .thumbnail {
                    width: 80px;
                    height: 80px;
                }

                .image-zoom-indicator {
                    top: 10px;
                    right: 10px;
                    padding: 8px 12px;
                    font-size: 11px;
                }
            }

            @media (max-width: 480px) {
                .main-image-container {
                    min-height: 300px;
                    padding: 0.5rem;
                }

                .main-image {
                    max-height: 300px;
                }

                .thumbnail {
                    width: 70px;
                    height: 70px;
                }

                .image-thumbnails {
                    gap: 0.5rem;
                    padding: 0.75rem;
                }
            }

            /* Animations for zoom modal */
            @keyframes fadeIn {
                from {
                    opacity: 0;
                }
                to {
                    opacity: 1;
                }
            }

            @keyframes fadeOut {
                from {
                    opacity: 1;
                }
                to {
                    opacity: 0;
                }
            }

            @keyframes zoomIn {
                from {
                    transform: scale(0.8);
                    opacity: 0;
                }
                to {
                    transform: scale(1);
                    opacity: 1;
                }
            }

            /* Responsive navigation buttons */
            @media (max-width: 768px) {
                .zoom-nav-btn {
                    width: 40px !important;
                    height: 40px !important;
                    font-size: 16px !important;
                }

                .zoom-close-btn {
                    width: 40px !important;
                    height: 40px !important;
                    font-size: 18px !important;
                }
            }
        </style>
       <script>
           // Variable globale pour contrôler si on doit bloquer le zoom temporairement
           let blockZoomTemporarily = false;
           let allImagesGlobal = [];

           // Initialize image zoom on page load with navigation
           document.addEventListener('DOMContentLoaded', function () {
               const mainImage = document.querySelector('.main-image');
               if (mainImage) {
                   // Get all product images (main + thumbnails)
                   allImagesGlobal = [];
                   allImagesGlobal.push({
                       src: mainImage.src,
                       element: mainImage
                   });

                   // Get thumbnails
                   document.querySelectorAll('.thumbnail').forEach(function (thumb) {
                       allImagesGlobal.push({
                           src: thumb.src,
                           element: thumb
                       });
                   });

                   // Initialize zoom with navigation
                   initImageZoomWithNavigation(mainImage, allImagesGlobal);

                   // Ensure no automatic zoom
                   mainImage.style.transform = 'scale(1)';
                   mainImage.style.transition = 'opacity 0.3s ease';
               }
           });

           // Function to initialize image zoom with navigation between images
           function initImageZoomWithNavigation(image, allImages) {
               if (!image || !allImages || allImages.length === 0) return;

               image.addEventListener('click', function (e) {
                   // IMPORTANT: Vérifier si le zoom est temporairement bloqué
                   if (blockZoomTemporarily) {
                       console.log('Zoom bloqué temporairement');
                       return;
                   }

                   e.preventDefault();
                   e.stopPropagation();

                   // Find current image index
                   let currentIndex = 0;
                   for (let i = 0; i < allImages.length; i++) {
                       if (allImages[i].src === image.src) {
                           currentIndex = i;
                           break;
                       }
                   }

                   // Create modal
                   const modal = document.createElement('div');
                   modal.className = 'image-zoom-modal';
                   modal.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0, 0, 0, 0.95);
                z-index: 10000;
                display: flex;
                align-items: center;
                justify-content: center;
                cursor: zoom-out;
                animation: fadeIn 0.3s ease;
            `;

                   // Create image container
                   const imgContainer = document.createElement('div');
                   imgContainer.style.cssText = `
                position: relative;
                width: 90%;
                height: 90%;
                display: flex;
                align-items: center;
                justify-content: center;
            `;

                   // Create image element
                   const img = document.createElement('img');
                   img.src = allImages[currentIndex].src;
                   img.style.cssText = `
                max-width: 100%;
                max-height: 100%;
                object-fit: contain;
                animation: zoomIn 0.3s ease;
            `;

                   imgContainer.appendChild(img);
                   modal.appendChild(imgContainer);

                   // Navigation buttons
                   if (allImages.length > 1) {
                       // Previous button
                       const prevBtn = document.createElement('button');
                       prevBtn.innerHTML = '<i class="fas fa-chevron-left"></i>';
                       prevBtn.className = 'zoom-nav-btn zoom-nav-prev';
                       prevBtn.style.cssText = `
                    position: absolute;
                    left: 20px;
                    top: 50%;
                    transform: translateY(-50%);
                    background: rgba(255, 255, 255, 0.2);
                    border: 2px solid rgba(255, 255, 255, 0.5);
                    color: white;
                    width: 50px;
                    height: 50px;
                    border-radius: 50%;
                    cursor: pointer;
                    font-size: 20px;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    transition: all 0.3s ease;
                    z-index: 10001;
                `;
                       prevBtn.onmouseenter = function () {
                           this.style.background = 'rgba(255, 255, 255, 0.3)';
                           this.style.transform = 'translateY(-50%) scale(1.1)';
                       };
                       prevBtn.onmouseleave = function () {
                           this.style.background = 'rgba(255, 255, 255, 0.2)';
                           this.style.transform = 'translateY(-50%) scale(1)';
                       };
                       prevBtn.onclick = function (e) {
                           e.stopPropagation();
                           currentIndex = (currentIndex - 1 + allImages.length) % allImages.length;
                           img.src = allImages[currentIndex].src;
                           updateImageCounter();
                       };
                       modal.appendChild(prevBtn);

                       // Next button
                       const nextBtn = document.createElement('button');
                       nextBtn.innerHTML = '<i class="fas fa-chevron-right"></i>';
                       nextBtn.className = 'zoom-nav-btn zoom-nav-next';
                       nextBtn.style.cssText = `
                    position: absolute;
                    right: 20px;
                    top: 50%;
                    transform: translateY(-50%);
                    background: rgba(255, 255, 255, 0.2);
                    border: 2px solid rgba(255, 255, 255, 0.5);
                    color: white;
                    width: 50px;
                    height: 50px;
                    border-radius: 50%;
                    cursor: pointer;
                    font-size: 20px;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    transition: all 0.3s ease;
                    z-index: 10001;
                `;
                       nextBtn.onmouseenter = function () {
                           this.style.background = 'rgba(255, 255, 255, 0.3)';
                           this.style.transform = 'translateY(-50%) scale(1.1)';
                       };
                       nextBtn.onmouseleave = function () {
                           this.style.background = 'rgba(255, 255, 255, 0.2)';
                           this.style.transform = 'translateY(-50%) scale(1)';
                       };
                       nextBtn.onclick = function (e) {
                           e.stopPropagation();
                           currentIndex = (currentIndex + 1) % allImages.length;
                           img.src = allImages[currentIndex].src;
                           updateImageCounter();
                       };
                       modal.appendChild(nextBtn);

                       // Image counter
                       const counter = document.createElement('div');
                       counter.className = 'zoom-image-counter';
                       counter.style.cssText = `
                    position: absolute;
                    bottom: 30px;
                    left: 50%;
                    transform: translateX(-50%);
                    background: rgba(0, 0, 0, 0.6);
                    color: white;
                    padding: 10px 20px;
                    border-radius: 25px;
                    font-size: 14px;
                    font-weight: 600;
                    z-index: 10001;
                `;
                       function updateImageCounter() {
                           counter.textContent = (currentIndex + 1) + ' / ' + allImages.length;
                       }
                       updateImageCounter();
                       modal.appendChild(counter);
                   }

                   // Close button
                   const closeBtn = document.createElement('button');
                   closeBtn.innerHTML = '<i class="fas fa-times"></i>';
                   closeBtn.className = 'zoom-close-btn';
                   closeBtn.style.cssText = `
                position: absolute;
                top: 20px;
                right: 20px;
                background: rgba(255, 255, 255, 0.2);
                border: 2px solid rgba(255, 255, 255, 0.5);
                color: white;
                width: 45px;
                height: 45px;
                border-radius: 50%;
                cursor: pointer;
                font-size: 20px;
                display: flex;
                align-items: center;
                justify-content: center;
                transition: all 0.3s ease;
                z-index: 10001;
            `;
                   closeBtn.onmouseenter = function () {
                       this.style.background = 'rgba(255, 0, 0, 0.5)';
                       this.style.transform = 'scale(1.1)';
                   };
                   closeBtn.onmouseleave = function () {
                       this.style.background = 'rgba(255, 255, 255, 0.2)';
                       this.style.transform = 'scale(1)';
                   };
                   closeBtn.onclick = function (e) {
                       e.stopPropagation();
                       closeModal();
                   };
                   modal.appendChild(closeBtn);

                   // Add to body
                   document.body.appendChild(modal);
                   document.body.style.overflow = 'hidden';

                   // Close on background click
                   modal.onclick = function (e) {
                       if (e.target === modal) {
                           closeModal();
                       }
                   };

                   // Keyboard navigation
                   function handleKeyPress(e) {
                       if (e.key === 'ArrowLeft') {
                           currentIndex = (currentIndex - 1 + allImages.length) % allImages.length;
                           img.src = allImages[currentIndex].src;
                           updateImageCounter();
                       } else if (e.key === 'ArrowRight') {
                           currentIndex = (currentIndex + 1) % allImages.length;
                           img.src = allImages[currentIndex].src;
                           updateImageCounter();
                       } else if (e.key === 'Escape') {
                           closeModal();
                       }
                   }

                   window.addEventListener('keydown', handleKeyPress);

                   // Close function
                   function closeModal() {
                       window.removeEventListener('keydown', handleKeyPress);
                       modal.style.animation = 'fadeOut 0.3s ease';
                       setTimeout(function () {
                           modal.remove();
                           document.body.style.overflow = '';
                       }, 300);
                   }
               });
           }

           // Function to change main image when thumbnail is clicked
           function changeMainImage(imageSrc, clickedElement) {
               const mainImage = document.querySelector('.main-image');

               if (mainImage) {
                   // BLOQUER LE ZOOM pendant le changement d'image
                   blockZoomTemporarily = true;
                   console.log('Changement d\'image - Zoom bloqué');

                   // Fade out
                   mainImage.style.opacity = '0';

                   setTimeout(function () {
                       // Changer l'image
                       mainImage.src = imageSrc;

                       // Handler pour quand l'image est chargée
                       mainImage.onload = function () {
                           // Fade in
                           mainImage.style.opacity = '1';

                           // Force NO ZOOM
                           this.style.transform = 'scale(1)';
                           this.style.transition = 'opacity 0.3s ease';

                           // DÉBLOQUER LE ZOOM après un délai
                           setTimeout(function () {
                               blockZoomTemporarily = false;
                               console.log('Zoom débloqué');
                           }, 500); // Attendre 500ms après le fade in
                       };
                   }, 150);

                   // Update active thumbnail
                   document.querySelectorAll('.thumbnail').forEach(function (thumb) {
                       thumb.classList.remove('active');
                   });
                   if (clickedElement) {
                       clickedElement.classList.add('active');
                   }
               }
           }
</script>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="container">
            <asp:Panel ID="pnlProductNotFound" runat="server" Visible="false">
                <div style="text-align: center; padding: 4rem;">
                    <h2>Produit non trouvé</h2>
                    <p style="color: var(--text-light); margin-bottom: 2rem;">Le produit que vous recherchez n'existe
                        pas ou n'est plus disponible.</p>
                    <a href="Shop.aspx" class="btn btn-primary">Retour à la boutique</a>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlProductDetails" runat="server">
                <div class="product-details-container">
                    <!-- Gallery -->
                    <div class="product-gallery">
                        <div class="image-zoom-indicator">
                            <i class="fas fa-search-plus"></i> Cliquez pour zoomer
                        </div>
                        <div class="main-image-container">
                            <asp:Image ID="imgMain" runat="server" CssClass="main-image" />
                        </div>
                        <div class="image-thumbnails">
                            <asp:Repeater ID="rptProductImages" runat="server">
                                <ItemTemplate>
                                    <div class="thumbnail-wrapper">
                                        <img src='<%# GetImageUrlForRepeater(Eval("ImageUrl")) %>' 
                                             class="thumbnail <%# GetThumbnailClass(Eval("IsPrimary")) %>"
                                             onclick="changeMainImage(this.src, this)"
                                             alt="Produit" 
                                             onerror="this.src='<%# ResolveUrl("~/Assets/Images/placeholder.svg") %>'" />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <!-- Info -->
                    <div class="product-info">
                        <div class="product-header">
                            <h1>
                                <asp:Label ID="lblName" runat="server"></asp:Label>
                            </h1>
                            <asp:Label ID="lblSKU" runat="server" CssClass="product-sku" Visible="false"></asp:Label>
                        </div>

                        <asp:Literal ID="litStockStatus" runat="server"></asp:Literal>

                        <div class="product-price">
                            <asp:Label ID="lblPrice" runat="server"></asp:Label> MAD
                            <asp:Label ID="lblComparePrice" runat="server" CssClass="product-price-compare"
                                Visible="false"></asp:Label>
                        </div>

                        <div class="product-meta">
                            <div class="meta-item">
                                <strong><i class="fas fa-box"></i> Stock</strong>
                                <span>
                                    <asp:Label ID="lblStock" runat="server"></asp:Label> unités
                                </span>
                            </div>
                            <div class="meta-item">
                                <strong><i class="fas fa-tag"></i> Catégorie</strong>
                                <span>
                                    <asp:Label ID="lblCategory" runat="server"></asp:Label>
                                </span>
                            </div>
                        </div>

                        <p class="product-description">
                            <asp:Label ID="lblDescription" runat="server"></asp:Label>
                        </p>

                        <!-- Variants -->
                        <asp:Panel ID="pnlVariants" runat="server" Visible="false">
                            <div class="options-group">
                                <label>Variante</label>
                                <asp:DropDownList ID="ddlVariants" runat="server" CssClass="form-select"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlVariants_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </asp:Panel>

                        <div class="options-group">
                            <label for="<%= txtQuantity.ClientID %>">Quantité</label>
                            <asp:TextBox ID="txtQuantity" runat="server" TextMode="Number" Text="1" min="1"
                                CssClass="qty-input"></asp:TextBox>
                        </div>

                        <div class="actions">
                            <asp:Button ID="btnAddToCart" runat="server" Text="Ajouter au panier"
                                CssClass="btn btn-primary" OnClick="btnAddToCart_Click"
                                style="flex: 1; padding: 15px; font-size: 16px;" />
                            <asp:LinkButton ID="btnWishlist" runat="server" CssClass="btn btn-outline"
                                style="padding: 15px; width: 60px;" OnClick="btnWishlist_Click">
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <!-- Reviews Section -->
                <div class="reviews-section">
                    <h3 style="margin-bottom: 2rem;">Avis clients</h3>

                    <asp:Panel ID="pnlAddReview" runat="server" Visible="false" style="margin-bottom: 2rem;">
                        <div class="card">
                            <h4 style="margin-bottom: 1rem;">Ajouter un avis</h4>
                            <asp:Panel ID="pnlReviewError" runat="server" Visible="false" CssClass="alert alert-danger">
                                <asp:Literal ID="litReviewError" runat="server"></asp:Literal>
                            </asp:Panel>
                            <div class="form-group">
                                <label>Note *</label>
                                <asp:DropDownList ID="ddlRating" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="5" Text="5 étoiles - Excellent"></asp:ListItem>
                                    <asp:ListItem Value="4" Text="4 étoiles - Très bien"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="3 étoiles - Bien"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="2 étoiles - Moyen"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="1 étoile - Décevant"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label>Votre avis *</label>
                                <asp:TextBox ID="txtReviewComment" runat="server" CssClass="form-control"
                                    TextMode="MultiLine" Rows="4" required></asp:TextBox>
                            </div>
                            <asp:Button ID="btnSubmitReview" runat="server" Text="Publier mon avis"
                                CssClass="btn btn-primary" OnClick="btnSubmitReview_Click" />
                        </div>
                    </asp:Panel>

                    <asp:Repeater ID="rptReviews" runat="server">
                        <ItemTemplate>
                            <div class="review-item">
                                <div class="review-header">
                                    <div>
                                        <strong>
                                            <%# Eval("FullName") %>
                                        </strong>
                                        <div class="review-rating">
                                            <%# GetStars(Eval("Rating")) %>
                                        </div>
                                    </div>
                                    <span style="color: var(--text-light); font-size: 0.9rem;">
                                        <%# GetReviewDate(Eval("ReviewDate")) %>
                                    </span>
                                </div>
                                <p style="margin: 0; color: var(--text-dark);">
                                    <%# Eval("Comment") %>
                                </p>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblNoReviews" runat="server" Text="Aucun avis pour le moment." Visible="false"
                        style="color: var(--text-light); text-align: center; display: block; padding: 2rem;">
                    </asp:Label>
                </div>
            </asp:Panel>
        </div>
    </asp:Content>