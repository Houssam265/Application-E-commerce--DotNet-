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
            border-radius: 12px;
            overflow: hidden;
            border: 1px solid var(--border-color);
            background: var(--bg-white);
            box-shadow: var(--shadow-md);
            transition: var(--transition);
            position: relative;
        }

        .product-gallery:hover {
            box-shadow: var(--shadow-lg);
        }

        .main-image {
            width: 100%;
            height: 550px;
            object-fit: contain;
            background: var(--bg-white);
            padding: 1rem;
            cursor: zoom-in;
            transition: transform 0.3s ease;
        }

        .main-image:hover {
            transform: scale(1.05);
        }

        .image-zoom-indicator {
            position: absolute;
            top: 15px;
            right: 15px;
            background: rgba(0, 0, 0, 0.7);
            color: white;
            padding: 8px 12px;
            border-radius: 20px;
            font-size: 12px;
            display: flex;
            align-items: center;
            gap: 5px;
            opacity: 0;
            transition: opacity 0.3s ease;
        }

        .product-gallery:hover .image-zoom-indicator {
            opacity: 1;
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
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
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
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
            transition: box-shadow 0.3s ease;
        }

        .review-item:hover {
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
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
        }
    </style>
    <script>
        // Initialize image zoom on page load
        document.addEventListener('DOMContentLoaded', function() {
            const mainImage = document.querySelector('.main-image');
            if (mainImage && typeof initImageZoom !== 'undefined') {
                initImageZoom(mainImage);
            }
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:Panel ID="pnlProductNotFound" runat="server" Visible="false">
            <div style="text-align: center; padding: 4rem;">
                <h2>Produit non trouvé</h2>
                <p style="color: var(--text-light); margin-bottom: 2rem;">Le produit que vous recherchez n'existe pas ou n'est plus disponible.</p>
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
                    <asp:Image ID="imgMain" runat="server" CssClass="main-image image-zoom-container"
                        onerror="this.src='https://via.placeholder.com/500x500/f5f5f5/051922?text=Produit'" />
                </div>

                <!-- Info -->
                <div class="product-info">
                    <div class="product-header">
                        <h1><asp:Label ID="lblName" runat="server"></asp:Label></h1>
                        <asp:Label ID="lblSKU" runat="server" CssClass="product-sku" Visible="false"></asp:Label>
                    </div>
                    
                    <asp:Literal ID="litStockStatus" runat="server"></asp:Literal>

                    <div class="product-price">
                        <asp:Label ID="lblPrice" runat="server"></asp:Label> MAD
                        <asp:Label ID="lblComparePrice" runat="server" CssClass="product-price-compare" Visible="false"></asp:Label>
                    </div>

                    <div class="product-meta">
                        <div class="meta-item">
                            <strong><i class="fas fa-box"></i> Stock</strong>
                            <span><asp:Label ID="lblStock" runat="server"></asp:Label> unités</span>
                        </div>
                        <div class="meta-item">
                            <strong><i class="fas fa-tag"></i> Catégorie</strong>
                            <span><asp:Label ID="lblCategory" runat="server"></asp:Label></span>
                        </div>
                    </div>

                    <p class="product-description">
                        <asp:Label ID="lblDescription" runat="server"></asp:Label>
                    </p>

                    <!-- Variants -->
                    <asp:Panel ID="pnlVariants" runat="server" Visible="false">
                        <div class="options-group">
                            <label>Variante</label>
                            <asp:DropDownList ID="ddlVariants" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlVariants_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </asp:Panel>

                    <div class="options-group">
                        <label for="<%= txtQuantity.ClientID %>">Quantité</label>
                        <asp:TextBox ID="txtQuantity" runat="server" TextMode="Number" Text="1" min="1" CssClass="qty-input"></asp:TextBox>
                    </div>

                    <div class="actions">
                        <asp:Button ID="btnAddToCart" runat="server" Text="Ajouter au panier" CssClass="btn btn-primary"
                            OnClick="btnAddToCart_Click" style="flex: 1; padding: 15px; font-size: 16px;" />
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
                            <asp:TextBox ID="txtReviewComment" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" required></asp:TextBox>
                        </div>
                        <asp:Button ID="btnSubmitReview" runat="server" Text="Publier mon avis" CssClass="btn btn-primary" OnClick="btnSubmitReview_Click" />
                    </div>
                </asp:Panel>

                <asp:Repeater ID="rptReviews" runat="server">
                    <ItemTemplate>
                        <div class="review-item">
                            <div class="review-header">
                                <div>
                                    <strong><%# Eval("FullName") %></strong>
                                    <div class="review-rating">
                                        <%# GetStars(Eval("Rating")) %>
                                    </div>
                                </div>
                                <span style="color: var(--text-light); font-size: 0.9rem;">
                                    <%# GetReviewDate(Eval("ReviewDate")) %>
                                </span>
                            </div>
                            <p style="margin: 0; color: var(--text-dark);"><%# Eval("Comment") %></p>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoReviews" runat="server" Text="Aucun avis pour le moment." Visible="false" 
                    style="color: var(--text-light); text-align: center; display: block; padding: 2rem;"></asp:Label>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
