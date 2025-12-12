<%@ Page Title="Accueil" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Ecommerce.Default" %>

    <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

        <section class="hero" style="border-radius: 20px; margin-bottom: 4rem;">
            <div class="container">
                <h1
                    style="background: linear-gradient(to right, #fff, #a5b4fc); -webkit-background-clip: text; -webkit-text-fill-color: transparent;">
                    Découvrez l'Excellence
                </h1>
                <p>Une collection exclusive de produits premium pour un style de vie unique.</p>
                <a href="/Pages/Public/Shop.aspx" class="btn btn-primary"
                    style="font-size: 1.2rem; padding: 1rem 2rem;">
                    Commencer vos achats
                </a>
            </div>
        </section>

        <div class="container">
            <h2 style="margin-bottom: 2rem; border-left: 5px solid var(--accent); padding-left: 1rem;">Catégories
                Populaires</h2>
            <div class="products-grid" style="grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));">
                <!-- Dynamic Categories or Hardcoded for demo -->
                <div class="card" style="text-align: center; cursor: pointer;"
                    onclick="location.href='/Pages/Public/Shop.aspx?cat=1'">
                    <h3 style="color: var(--accent);">Electronics</h3>
                    <p>Latest gadgets</p>
                </div>
                <div class="card" style="text-align: center; cursor: pointer;"
                    onclick="location.href='/Pages/Public/Shop.aspx?cat=2'">
                    <h3 style="color: var(--gold);">Fashion</h3>
                    <p>Trending styles</p>
                </div>
                <div class="card" style="text-align: center; cursor: pointer;"
                    onclick="location.href='/Pages/Public/Shop.aspx?cat=3'">
                    <h3 style="color: #10b981;">Home</h3>
                    <p>Decor & Essentials</p>
                </div>
            </div>
        </div>

        <div class="container" style="margin-top: 4rem;">
            <h2 style="margin-bottom: 2rem; border-left: 5px solid var(--gold); padding-left: 1rem;">Nouveautés</h2>

            <asp:Repeater ID="rptFeaturedProducts" runat="server">
                <HeaderTemplate>
                    <div class="products-grid">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="product-card">
                        <span class="badge badge-new">New</span>
                        <img src='<%# Eval("ImageUrl") %>' class="product-img" alt='<%# Eval("Name") %>'
                            onerror="this.src='https://via.placeholder.com/300x250/1e293b/ffffff?text=No+Image'" />
                        <div class="product-info">
                            <h3 class="product-title">
                                <%# Eval("Name") %>
                            </h3>
                            <div
                                style="display: flex; justify-content: space-between; align-items: center; margin-top: 1rem;">
                                <span class="product-price">
                                    <%# Eval("Price", "{0:C}" ) %>
                                </span>
                                <a href='/Pages/Public/ProductDetails.aspx?id=<%# Eval("Id") %>' class="btn btn-primary"
                                    style="padding: 0.5rem 1rem; font-size: 0.9rem;">Voir</a>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
        </div>
        </FooterTemplate>
        </asp:Repeater>
        </div>

    </asp:Content>