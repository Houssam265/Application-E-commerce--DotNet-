<%@ Page Title="Boutique" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Shop.aspx.cs"
    Inherits="Ecommerce.Pages.Public.Shop" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .shop-header {
            margin-bottom: 2rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid var(--border-color);
        }

        .shop-container {
            display: grid;
            grid-template-columns: 250px 1fr;
            gap: 2rem;
            margin-top: 2rem;
        }

        .filters {
            background: var(--bg-white);
            padding: 1.5rem;
            border-radius: 10px;
            border: 1px solid var(--border-color);
            height: fit-content;
            position: sticky;
            top: 100px;
        }

        .filter-section {
            margin-bottom: 2rem;
        }

        .filter-section:last-child {
            margin-bottom: 0;
        }

        .filter-section h4 {
            margin-bottom: 1rem;
            font-size: 1.1rem;
            color: var(--text-dark);
            border-bottom: 2px solid var(--primary-color);
            padding-bottom: 0.5rem;
        }

        .category-link {
            display: block;
            padding: 0.75rem 0;
            color: var(--text-light);
            text-decoration: none;
            transition: all 0.3s;
            border-bottom: 1px solid var(--border-color);
        }

        .category-link:last-child {
            border-bottom: none;
        }

        .category-link:hover,
        .category-link.active {
            color: var(--primary-color);
            padding-left: 0.5rem;
            font-weight: 600;
        }

        .search-bar {
            margin-bottom: 2rem;
            display: flex;
            gap: 1rem;
        }

        .search-input {
            flex: 1;
            padding: 12px 15px;
            border-radius: 5px;
            border: 1px solid var(--border-color);
            font-size: 14px;
        }

        .search-input:focus {
            outline: none;
            border-color: var(--primary-color);
            box-shadow: 0 0 0 3px rgba(40, 167, 69, 0.1);
        }

        .price-filter {
            display: flex;
            gap: 0.5rem;
            align-items: center;
        }

        .price-input {
            width: 100px;
            padding: 8px;
            border: 1px solid var(--border-color);
            border-radius: 5px;
        }

        .filter-btn {
            width: 100%;
            margin-top: 1rem;
        }

        .products-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1.5rem;
        }

        .sort-select {
            padding: 8px 15px;
            border: 1px solid var(--border-color);
            border-radius: 5px;
            font-size: 14px;
        }

        @media (max-width: 768px) {
            .shop-container {
                grid-template-columns: 1fr;
            }

            .filters {
                position: static;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="shop-header">
            <h1><i class="fas fa-store" style="color: var(--primary-color);"></i> Boutique</h1>
            <p style="color: var(--text-light);">Découvrez nos produits des coopératives marocaines</p>
        </div>

        <div class="shop-container">
            <!-- Sidebar Filters -->
            <aside class="filters">
                <div class="filter-section">
                    <h4><i class="fas fa-list"></i> Catégories</h4>
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
                    <h4><i class="fas fa-filter"></i> Filtres</h4>
                    <div style="margin-bottom: 1rem;">
                        <label style="display: block; margin-bottom: 0.5rem; font-size: 14px; font-weight: 600;">Prix min</label>
                        <asp:TextBox ID="txtMinPrice" runat="server" CssClass="price-input" TextMode="Number" placeholder="0"></asp:TextBox>
                    </div>
                    <div style="margin-bottom: 1rem;">
                        <label style="display: block; margin-bottom: 0.5rem; font-size: 14px; font-weight: 600;">Prix max</label>
                        <asp:TextBox ID="txtMaxPrice" runat="server" CssClass="price-input" TextMode="Number" placeholder="1000"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnApplyFilters" runat="server" Text="Appliquer" CssClass="btn btn-primary filter-btn" OnClick="btnApplyFilters_Click" />
                </div>

                <div class="filter-section">
                    <h4><i class="fas fa-check-circle"></i> Disponibilité</h4>
                    <label style="display: flex; align-items: center; gap: 0.5rem; cursor: pointer;">
                        <asp:CheckBox ID="chkInStock" runat="server" Checked="true" />
                        <span>En stock uniquement</span>
                    </label>
                </div>
            </aside>

            <!-- Product Grid -->
            <div>
                <div class="search-bar">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input"
                        Placeholder="Rechercher un produit..."></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Rechercher" CssClass="btn btn-primary"
                        OnClick="btnSearch_Click" />
                </div>

                <div class="products-header">
                    <div>
                        <asp:Label ID="lblProductCount" runat="server" Text="" Style="color: var(--text-light);"></asp:Label>
                    </div>
                    <div>
                        <asp:DropDownList ID="ddlSort" runat="server" CssClass="sort-select" AutoPostBack="true" OnSelectedIndexChanged="ddlSort_SelectedIndexChanged">
                            <asp:ListItem Value="newest" Text="Plus récents" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="price_asc" Text="Prix croissant"></asp:ListItem>
                            <asp:ListItem Value="price_desc" Text="Prix décroissant"></asp:ListItem>
                            <asp:ListItem Value="name" Text="Nom A-Z"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <asp:Repeater ID="rptProducts" runat="server">
                    <HeaderTemplate>
                        <div class="product-grid">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <a href='/Pages/Public/ProductDetails.aspx?id=<%# Eval("Id") %>' class="product-card">
                            <%# GetStockBadge(Eval("StockQuantity")) %>
                            <img src='<%# GetImageUrl(Eval("ImageUrl")) %>' alt='<%# Eval("Name") %>'
                                onerror="this.src='https://via.placeholder.com/300x250/f5f5f5/051922?text=Produit'" loading="lazy" />
                            <div class="product-info">
                                <h3><%# Eval("Name") %></h3>
                                <p><%# Eval("ShortDescription") ?? (Eval("Description") != null ? (Eval("Description").ToString().Length > 100 ? Eval("Description").ToString().Substring(0, 100) + "..." : Eval("Description")) : "") %></p>
                                <div class="product-price">
                                    <%# Eval("Price", "{0:F2}") %> MAD
                                </div>
                                <div style="display: flex; justify-content: space-between; align-items: center; margin-top: 1rem;">
                                    <span style="color: var(--text-light); font-size: 0.85rem;">
                                        <i class="fas fa-box"></i> Stock: <%# Eval("StockQuantity") %>
                                    </span>
                                    <span class="btn btn-primary" style="padding: 8px 15px; font-size: 14px;">
                                        <i class="fas fa-eye"></i> Voir
                                    </span>
                                </div>
                            </div>
                        </a>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoResults" runat="server" Text="Aucun produit trouvé." Visible="false"
                    Style="color: var(--text-light); font-size: 1.2rem; text-align: center; display: block; padding: 3rem;"></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>
