<%@ Page Title="Détails du Produit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ProductDetails.aspx.cs" Inherits="Ecommerce.Pages.Public.ProductDetails" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .details-container {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 4rem;
                margin-top: 2rem;
                align-items: start;
            }

            .product-gallery {
                border-radius: 16px;
                overflow: hidden;
                border: 1px solid var(--glass-border);
                background: #000;
            }

            .main-image {
                width: 100%;
                height: 500px;
                object-fit: contain;
            }

            .product-meta h1 {
                font-size: 2.5rem;
                margin-bottom: 1rem;
            }

            .price {
                font-size: 2rem;
                color: var(--accent);
                font-weight: 700;
                margin-bottom: 2rem;
            }

            .description {
                color: var(--text-muted);
                margin-bottom: 2rem;
                line-height: 1.8;
                font-size: 1.1rem;
            }

            .options-group {
                margin-bottom: 2rem;
            }

            .options-group label {
                display: block;
                margin-bottom: 0.5rem;
                font-weight: 600;
                color: var(--text-muted);
            }

            .form-select {
                width: 100%;
                padding: 0.75rem;
                background: rgba(255, 255, 255, 0.05);
                border: 1px solid var(--glass-border);
                border-radius: 8px;
                color: white;
                font-size: 1rem;
                cursor: pointer;
            }

            .qty-input {
                width: 80px;
                padding: 0.75rem;
                background: rgba(255, 255, 255, 0.05);
                border: 1px solid var(--glass-border);
                border-radius: 8px;
                color: white;
                font-size: 1rem;
                text-align: center;
            }

            .actions {
                display: flex;
                gap: 1rem;
                margin-top: 2rem;
            }

            /* Mobile */
            @media (max-width: 768px) {
                .details-container {
                    grid-template-columns: 1fr;
                    gap: 2rem;
                }
            }
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <div class="details-container">
            <!-- Gallery -->
            <div class="product-gallery">
                <asp:Image ID="imgMain" runat="server" CssClass="main-image"
                    onerror="this.src='https://via.placeholder.com/500x500/1e293b/ffffff?text=No+Image'" />
            </div>

            <!-- Info -->
            <div class="product-meta">
                <h1>
                    <asp:Label ID="lblName" runat="server"></asp:Label>
                </h1>
                <div class="price">
                    <asp:Label ID="lblPrice" runat="server"></asp:Label>
                </div>

                <p class="description">
                    <asp:Label ID="lblDescription" runat="server"></asp:Label>
                </p>

                <!-- Variants (Simplified: just sizes for now if applicable, dynamic later) -->
                <asp:Panel ID="pnlVariants" runat="server" Visible="false">
                    <div class="options-group">
                        <label>Variante (Taille / Couleur)</label>
                        <asp:DropDownList ID="ddlVariants" runat="server" CssClass="form-select">
                        </asp:DropDownList>
                    </div>
                </asp:Panel>

                <div class="options-group">
                    <label>Quantité</label>
                    <asp:TextBox ID="txtQuantity" runat="server" TextMode="Number" Text="1" min="1" max="10"
                        CssClass="qty-input"></asp:TextBox>
                </div>

                <div class="actions">
                    <asp:Button ID="btnAddToCart" runat="server" Text="Ajouter au panier" CssClass="btn btn-primary"
                        OnClick="btnAddToCart_Click" style="flex: 1;" />
                    <asp:Button ID="btnWishlist" runat="server" Text="❤" CssClass="btn"
                        style="background: rgba(255,255,255,0.05); border: 1px solid var(--glass-border);" />
                </div>

                <asp:Label ID="lblMessage" runat="server" Visible="false"
                    Style="display: block; margin-top: 1rem; color: #10b981;"></asp:Label>
            </div>
        </div>

    </asp:Content>