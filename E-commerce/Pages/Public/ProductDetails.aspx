<%@ Page Title="Détails du Produit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ProductDetails.aspx.cs" Inherits="Ecommerce.Pages.Public.ProductDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .product-details-container {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 3rem;
            margin-top: 2rem;
            align-items: start;
        }

        .product-gallery {
            border-radius: 10px;
            overflow: hidden;
            border: 1px solid var(--border-color);
            background: var(--bg-light);
        }

        .main-image {
            width: 100%;
            height: 500px;
            object-fit: contain;
            background: var(--bg-white);
        }

        .product-info h1 {
            font-size: 2.5rem;
            margin-bottom: 1rem;
            color: var(--text-dark);
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
            color: var(--text-light);
            margin-bottom: 2rem;
            line-height: 1.8;
            font-size: 1rem;
        }

        .product-meta {
            display: flex;
            gap: 2rem;
            margin-bottom: 2rem;
            padding: 1rem;
            background: var(--bg-light);
            border-radius: 5px;
        }

        .meta-item {
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
        }

        .meta-item strong {
            color: var(--text-dark);
            font-size: 0.9rem;
        }

        .meta-item span {
            color: var(--text-light);
            font-size: 0.9rem;
        }

        .options-group {
            margin-bottom: 1.5rem;
        }

        .options-group label {
            display: block;
            margin-bottom: 0.5rem;
            font-weight: 600;
            color: var(--text-dark);
        }

        .form-select {
            width: 100%;
            padding: 12px 15px;
            border: 1px solid var(--border-color);
            border-radius: 5px;
            font-size: 14px;
            background: var(--bg-white);
            cursor: pointer;
        }

        .qty-input {
            width: 100px;
            padding: 12px;
            border: 1px solid var(--border-color);
            border-radius: 5px;
            font-size: 14px;
            text-align: center;
        }

        .actions {
            display: flex;
            gap: 1rem;
            margin-top: 2rem;
        }

        .stock-status {
            padding: 0.5rem 1rem;
            border-radius: 5px;
            display: inline-block;
            margin-bottom: 1rem;
            font-weight: 600;
            font-size: 0.9rem;
        }

        .stock-in {
            background: #d4edda;
            color: #155724;
        }

        .stock-out {
            background: #f8d7da;
            color: #721c24;
        }

        .reviews-section {
            margin-top: 4rem;
            padding-top: 2rem;
            border-top: 2px solid var(--border-color);
        }

        .review-item {
            padding: 1.5rem;
            background: var(--bg-light);
            border-radius: 5px;
            margin-bottom: 1rem;
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
                    <asp:Image ID="imgMain" runat="server" CssClass="main-image"
                        onerror="this.src='https://via.placeholder.com/500x500/f5f5f5/051922?text=Produit'" />
                </div>

                <!-- Info -->
                <div class="product-info">
                    <h1><asp:Label ID="lblName" runat="server"></asp:Label></h1>
                    
                    <asp:Panel ID="pnlStockStatus" runat="server">
                        <span class="stock-status stock-in">
                            <i class="fas fa-check-circle"></i> En stock
                        </span>
                    </asp:Panel>

                    <div class="product-price">
                        <asp:Label ID="lblPrice" runat="server"></asp:Label> MAD
                        <asp:Label ID="lblComparePrice" runat="server" CssClass="product-price-compare" Visible="false"></asp:Label>
                    </div>

                    <div class="product-meta">
                        <div class="meta-item">
                            <strong><i class="fas fa-store"></i> Coopérative</strong>
                            <span><asp:Label ID="lblCooperative" runat="server"></asp:Label></span>
                        </div>
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
                        <label>Quantité</label>
                        <asp:TextBox ID="txtQuantity" runat="server" TextMode="Number" Text="1" min="1" CssClass="qty-input"></asp:TextBox>
                    </div>

                    <div class="actions">
                        <asp:Button ID="btnAddToCart" runat="server" Text="Ajouter au panier" CssClass="btn btn-primary"
                            OnClick="btnAddToCart_Click" style="flex: 1; padding: 15px; font-size: 16px;" />
                        <asp:LinkButton ID="btnWishlist" runat="server" CssClass="btn btn-outline"
                            style="padding: 15px; width: 60px;" OnClick="btnWishlist_Click">
                            <i class="fas fa-heart"></i>
                        </asp:LinkButton>
                    </div>

                    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-success" style="margin-top: 1rem;">
                        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
                    </asp:Panel>
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
