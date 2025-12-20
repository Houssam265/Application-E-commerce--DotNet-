<%@ Page Title="Accueil" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Ecommerce.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Hero Section -->
    <section class="hero">
        <div class="container">
            <h1>Découvrez nos Produits</h1>
            <p>Agriculture biologique, artisanat traditionnel et produits du terroir</p>
            <a href="/Pages/Public/Shop.aspx" class="btn btn-primary" style="font-size: 1.2rem; padding: 15px 30px; margin-top: 1rem;">
                <i class="fas fa-shopping-bag"></i> Découvrir la Boutique
            </a>
        </div>
    </section>

    <!-- Categories Section -->
    <div class="container mb-80">
        <div class="section-title">
            <h3>Nos Catégories</h3>
            <p>Explorez nos différentes catégories de produits authentiques</p>
        </div>
        <div class="categories">
            <asp:Repeater ID="rptCategories" runat="server">
                <ItemTemplate>
                    <a href='/Pages/Public/Shop.aspx?cat=<%# Eval("Id") %>' class="category-card">
                        <%# GetCategoryImage(Eval("ImageUrl"), Eval("Name")) %>
                        <div class="category-content">
                            <h3><%# Eval("Name") %></h3>
                            <p><%# Eval("Description") %></p>
                        </div>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <!-- Featured Products Section -->
    <div class="container mb-80">
        <div class="section-title">
            <h3>Produits Mis en Avant</h3>
            <p>Découvrez nos produits phares</p>
        </div>
        <asp:Repeater ID="rptFeaturedProducts" runat="server">
            <HeaderTemplate>
                <div class="product-grid">
            </HeaderTemplate>
                    <ItemTemplate>
                        <a href='/Pages/Public/ProductDetails.aspx?id=<%# Eval("Id") %>' class="product-card">
                            <%# GetFeaturedBadge(Eval("IsFeatured")) %>
                            <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>'
                                onerror="this.src='https://via.placeholder.com/300x250/f5f5f5/051922?text=Produit'" />
                    <div class="product-info">
                        <h3><%# Eval("Name") %></h3>
                        <p><%# Eval("ShortDescription") ?? Eval("Description") %></p>
                        <div class="product-price">
                            <%# Eval("Price", "{0:F2}") %> MAD
                        </div>
                        <div style="display: flex; justify-content: flex-end; align-items: center; margin-top: 1rem;">
                            <span class="btn btn-primary" style="padding: 8px 15px; font-size: 14px;">
                                <i class="fas fa-shopping-cart"></i> Ajouter
                            </span>
                        </div>
                    </div>
                </a>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>

    <!-- Why Choose Us Section -->
    <section class="why-choose-us">
        <div class="container">
            <div class="section-title">
                <h3>Pourquoi Nous Choisir ?</h3>
                <p>Des avantages qui font la différence</p>
            </div>
            <div class="grid grid-3">
                <div style="text-align: center; padding: 1.5rem;">
                    <div style="font-size: 3rem; color: var(--primary-color); margin-bottom: 1rem;">
                        <i class="fas fa-leaf"></i>
                    </div>
                    <h4>Produits Bio & Naturels</h4>
                    <p>Des produits 100% biologiques et naturels</p>
                </div>
                <div style="text-align: center; padding: 1.5rem;">
                    <div style="font-size: 3rem; color: var(--primary-color); margin-bottom: 1rem;">
                        <i class="fas fa-truck"></i>
                    </div>
                    <h4>Livraison Rapide</h4>
                    <p>Livraison partout au Maroc et à l'international</p>
                </div>
                <div style="text-align: center; padding: 1.5rem;">
                    <div style="font-size: 3rem; color: var(--primary-color); margin-bottom: 1rem;">
                        <i class="fas fa-shield-alt"></i>
                    </div>
                    <h4>Paiement Sécurisé</h4>
                    <p>Transactions sécurisées et garanties</p>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
