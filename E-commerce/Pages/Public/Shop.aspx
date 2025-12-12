<%@ Page Title="Boutique" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Shop.aspx.cs"
    Inherits="Ecommerce.Pages.Public.Shop" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .shop-container {
                display: grid;
                grid-template-columns: 250px 1fr;
                gap: 2rem;
                margin-top: 2rem;
            }

            .filters {
                background: var(--secondary-bg);
                padding: 1.5rem;
                border-radius: 12px;
                border: 1px solid var(--glass-border);
                height: fit-content;
            }

            .filter-section {
                margin-bottom: 2rem;
            }

            .filter-section h3 {
                margin-bottom: 1rem;
                font-size: 1.1rem;
                border-bottom: 1px solid var(--glass-border);
                padding-bottom: 0.5rem;
            }

            .category-link {
                display: block;
                padding: 0.5rem 0;
                color: var(--text-muted);
            }

            .category-link:hover,
            .category-link.active {
                color: var(--accent);
                padding-left: 0.5rem;
            }

            .search-bar {
                margin-bottom: 2rem;
                display: flex;
                gap: 1rem;
            }

            .search-input {
                flex: 1;
                padding: 0.75rem;
                border-radius: 8px;
                border: 1px solid var(--glass-border);
                background: rgba(255, 255, 255, 0.05);
                color: white;
            }
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <div class="shop-container">
            <!-- Sidebar Filters -->
            <aside class="filters">
                <div class="filter-section">
                    <h3>Catégories</h3>
                    <asp:Repeater ID="rptCategories" runat="server">
                        <ItemTemplate>
                            <a href='Shop.aspx?cat=<%# Eval("Id") %>'
                                class='category-link <%# IsActiveCategory(Eval("Id")) %>'>
                                <%# Eval("Name") %>
                            </a>
                        </ItemTemplate>
                    </asp:Repeater>
                    <a href="Shop.aspx" class="category-link">Tout voir</a>
                </div>

                <div class="filter-section">
                    <h3>Prix</h3>
                    <!-- Placeholder for Price Filter -->
                    <p style="color:var(--text-muted)">Filtre par prix bientôt disponible</p>
                </div>
            </aside>

            <!-- Product Grid -->
            <div>
                <div class="search-bar">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input"
                        Placeholder="Rechercher un produit..."></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Chercher" CssClass="btn btn-primary"
                        OnClick="btnSearch_Click" />
                </div>

                <asp:Repeater ID="rptProducts" runat="server">
                    <HeaderTemplate>
                        <div class="products-grid">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="product-card">
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
                                    <a href='ProductDetails.aspx?id=<%# Eval("Id") %>' class="btn btn-primary"
                                        style="padding: 0.5rem 1rem; font-size: 0.9rem;">Voir</a>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
            </div>
            </FooterTemplate>
            </asp:Repeater>

            <asp:Label ID="lblNoResults" runat="server" Text="Aucun produit trouvé." Visible="false"
                Style="color: var(--text-muted); font-size: 1.2rem;"></asp:Label>
        </div>
        </div>

    </asp:Content>