<%@ Page Title="Boutique" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Shop.aspx.cs"
    Inherits="Ecommerce.Pages.Public.Shop" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .shop-header {
            margin-bottom: 2rem;
            padding-bottom: 1.5rem;
            border-bottom: 3px solid var(--primary-color);
            background: linear-gradient(135deg, rgba(40, 167, 69, 0.05) 0%, transparent 100%);
            padding: 2rem;
            border-radius: 12px;
        }

        .shop-container {
            display: grid;
            grid-template-columns: 250px 1fr;
            gap: 2rem;
            margin-top: 2rem;
            position: relative;
        }

        .products-section {
            margin-left: 0;
        }

        .filters {
            background: var(--bg-white);
            padding: 1.5rem;
            border-radius: 12px;
            border: 1px solid var(--border-color);
            position: sticky;
            top: 120px;
            width: 100%;
            max-height: calc(100vh - 140px);
            overflow-y: auto;
            overflow-x: hidden;
            z-index: 100;
            align-self: start;
            box-shadow: var(--shadow-sm);
            transition: var(--transition);
        }

        .filters:hover {
            box-shadow: var(--shadow-md);
        }

        .filters::-webkit-scrollbar {
            width: 6px;
        }

        .filters::-webkit-scrollbar-track {
            background: var(--bg-light);
            border-radius: 3px;
        }

        .filters::-webkit-scrollbar-thumb {
            background: var(--primary-color);
            border-radius: 3px;
        }

        .filters::-webkit-scrollbar-thumb:hover {
            background: var(--primary-dark);
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
            padding: 0.75rem 1rem;
            color: var(--text-light);
            text-decoration: none;
            transition: var(--transition);
            border-bottom: 1px solid var(--border-color);
            border-radius: 6px;
            margin-bottom: 0.25rem;
            position: relative;
        }

        .category-link:last-child {
            border-bottom: none;
            margin-bottom: 0;
        }

        .category-link::before {
            content: '';
            position: absolute;
            left: 0;
            top: 50%;
            transform: translateY(-50%);
            width: 3px;
            height: 0;
            background: var(--primary-color);
            border-radius: 0 3px 3px 0;
            transition: height 0.3s ease;
        }

        .category-link:hover,
        .category-link.active {
            color: var(--primary-color);
            padding-left: 1.5rem;
            font-weight: 600;
            background: rgba(40, 167, 69, 0.05);
        }

        .category-link:hover::before,
        .category-link.active::before {
            height: 60%;
        }

        .search-bar {
            margin-bottom: 2rem;
            display: flex;
            gap: 1rem;
        }

        .search-input {
            flex: 1;
            padding: 14px 20px;
            border-radius: 50px;
            border: 2px solid var(--border-color);
            font-size: 14px;
            transition: var(--transition);
            background: var(--bg-white);
        }

        .search-input:focus {
            outline: none;
            border-color: var(--primary-color);
            box-shadow: 0 0 0 4px rgba(40, 167, 69, 0.1);
            transform: translateY(-2px);
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
            padding: 1rem;
            background: var(--bg-light);
            border-radius: 10px;
        }

        .sort-select {
            padding: 10px 20px;
            border: 2px solid var(--border-color);
            border-radius: 8px;
            font-size: 14px;
            background: var(--bg-white);
            cursor: pointer;
            transition: var(--transition);
            font-weight: 500;
        }

        .sort-select:focus {
            outline: none;
            border-color: var(--primary-color);
            box-shadow: 0 0 0 3px rgba(40, 167, 69, 0.1);
        }

        .sort-select:hover {
            border-color: var(--primary-color);
        }

        .product-card-wrapper {
            position: relative;
        }

        .product-actions {
            display: flex;
            gap: 0.5rem;
            padding: 1rem;
            background: var(--bg-white);
            border-top: 1px solid var(--border-color);
            border-radius: 0 0 10px 10px;
        }

        .product-actions .btn {
            flex: 1;
            padding: 10px;
            font-size: 13px;
            border-radius: 5px;
            text-align: center;
        }

        .product-actions .btn-sm {
            padding: 8px 12px;
        }

        .product-actions .wishlist-btn {
            background: transparent;
            border: 1px solid var(--primary-color);
            color: var(--primary-color);
        }

        .product-actions .wishlist-btn:hover {
            background: var(--primary-color);
            color: #fff;
        }

        .product-card-wrapper:hover .product-actions {
            display: flex;
        }

        @media (max-width: 768px) {
            .shop-container {
                grid-template-columns: 1fr;
            }

            .filters {
                position: static;
                width: 100%;
                max-height: none;
                left: auto;
                top: auto;
            }

            .products-section {
                margin-left: 0;
            }
        }

        @media (max-width: 1200px) {
            .filters {
                left: 15px;
            }
        }
    </style>
    <script>
        function adjustFiltersHeight() {
            var filters = document.querySelector('.filters');
            if (!filters) return;

            var footer = document.querySelector('footer');
            if (!footer) return;

            var windowHeight = window.innerHeight;
            var filtersTop = filters.offsetTop;
            var footerTop = footer.getBoundingClientRect().top + window.scrollY;
            var scrollY = window.scrollY || window.pageYOffset;
            
            // Calculer la distance entre le bas du filtre et le haut du footer
            var filtersBottom = filtersTop + filters.offsetHeight;
            var distanceToFooter = footerTop - filtersBottom;
            
            // Si le footer approche (moins de 50px), réduire la hauteur max
            if (distanceToFooter < 50) {
                var maxHeight = Math.max(300, footerTop - filtersTop - 100);
                filters.style.maxHeight = maxHeight + 'px';
            } else {
                // Sinon, utiliser la hauteur maximale normale
                var availableHeight = windowHeight - filtersTop - 100;
                filters.style.maxHeight = Math.min(availableHeight, windowHeight - 200) + 'px';
            }
        }

        // Ajuster au chargement et au scroll
        window.addEventListener('load', adjustFiltersHeight);
        window.addEventListener('scroll', adjustFiltersHeight);
        window.addEventListener('resize', adjustFiltersHeight);
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="shop-header">
            <h1><i class="fas fa-store" style="color: var(--primary-color);"></i> Boutique</h1>
            <p style="color: var(--text-light);">Découvrez nos produits</p>
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
            <div class="products-section">
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

                <asp:Repeater ID="rptProducts" runat="server" OnItemCommand="rptProducts_ItemCommand">
                    <HeaderTemplate>
                        <div class="product-grid">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="product-card-wrapper">
                            <div class="product-card">
                                <a href='/Pages/Public/ProductDetails.aspx?id=<%# Eval("Id") %>' style="text-decoration: none; color: inherit;">
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
                                        </div>
                                    </div>
                                </a>
                                <div class="product-actions">
                                    <asp:LinkButton ID="btnAddToCart" runat="server" 
                                        CommandArgument='<%# Eval("Id") %>' 
                                        CommandName="AddToCart"
                                        CssClass="btn btn-icon btn-primary"
                                        Enabled='<%# IsInStock(Eval("StockQuantity")) %>'
                                        OnClientClick="event.stopPropagation(); return true;"
                                        title="Ajouter au panier">
                                        <i class="fas fa-shopping-cart"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnAddToWishlist" runat="server" 
                                        CommandArgument='<%# Eval("Id") %>' 
                                        CommandName="AddToWishlist"
                                        CssClass='<%# "btn btn-icon btn-wishlist " + IsInWishlist(Eval("IsInWishlist")) %>'
                                        OnClientClick="event.stopPropagation(); return true;"
                                        title="Ajouter aux favoris">
                                        <%# GetWishlistIcon(Eval("IsInWishlist")) %>
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
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
