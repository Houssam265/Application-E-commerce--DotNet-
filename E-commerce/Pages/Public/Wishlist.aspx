<%@ Page Title="Mes Favoris" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Wishlist.aspx.cs" Inherits="Ecommerce.Pages.Public.Wishlist" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .wishlist-header {
                margin-bottom: 2rem;
                padding-bottom: 1rem;
                border-bottom: 2px solid var(--border-color);
            }

            .empty-wishlist {
                text-align: center;
                padding: 4rem 2rem;
                background: var(--bg-light);
                border-radius: 10px;
                margin-top: 2rem;
            }

            .empty-wishlist i {
                font-size: 4rem;
                color: var(--text-light);
                margin-bottom: 1rem;
            }

            .wishlist-actions {
                display: flex;
                gap: 0.5rem;
                margin-top: 1rem;
            }

            .wishlist-actions .btn {
                flex: 1;
                padding: 10px;
                font-size: 13px;
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="container">
            <div class="wishlist-header">
                <h1><i class="fas fa-heart" style="color: var(--primary-color);"></i> Mes Favoris</h1>
                <p style="color: var(--text-light);">Vos produits préférés sauvegardés</p>
            </div>

            <asp:Panel ID="pnlNotLoggedIn" runat="server" Visible="false">
                <div class="empty-wishlist">
                    <i class="fas fa-lock"></i>
                    <h3>Connexion requise</h3>
                    <p>Vous devez être connecté pour voir vos favoris.</p>
                    <a href="Login.aspx?returnUrl=<%= Server.UrlEncode(Request.RawUrl) %>" class="btn btn-primary">
                        <i class="fas fa-sign-in-alt"></i> Se connecter
                    </a>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlWishlist" runat="server">
                <asp:Repeater ID="rptWishlist" runat="server" OnItemCommand="rptWishlist_ItemCommand">
                    <HeaderTemplate>
                        <div class="product-grid">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="product-card-wrapper">
                            <div class="product-card">
                                <a href='/Pages/Public/ProductDetails.aspx?id=<%# Eval("ProductId") %>'
                                    style="text-decoration: none; color: inherit;">
                                    <%# GetStockBadge(Eval("StockQuantity")) %>
                                        <img src='<%# GetImageUrl(Eval("ImageUrl")) %>' alt='<%# Eval("Name") %>'
                                            onerror="this.src='<%# ResolveUrl("~/Assets/Images/placeholder.svg") %>'"
                                        loading="lazy" />
                                        <div class="product-info">
                                            <h3>
                                                <%# Eval("Name") %>
                                            </h3>
                                            <p>
                                                <%# Eval("ShortDescription") ?? (Eval("Description") !=null ?
                                                    (Eval("Description").ToString().Length> 100 ?
                                                    Eval("Description").ToString().Substring(0, 100) + "..." :
                                                    Eval("Description")) : "") %>
                                            </p>
                                            <div class="product-price">
                                                <%# Eval("Price", "{0:F2}" ) %> MAD
                                            </div>
                                            <div
                                                style="display: flex; justify-content: space-between; align-items: center; margin-top: 1rem;">
                                                <span style="color: var(--text-light); font-size: 0.85rem;">
                                                    <i class="fas fa-box"></i> Stock: <%# Eval("StockQuantity") %>
                                                </span>
                                            </div>
                                        </div>
                                </a>
                                <div class="product-actions">
                                    <asp:LinkButton ID="btnAddToCart" runat="server"
                                        CommandArgument='<%# Eval("ProductId") %>' CommandName="AddToCart"
                                        CssClass="btn btn-icon btn-primary"
                                        Enabled='<%# IsInStock(Eval("StockQuantity")) %>'
                                        OnClientClick="event.stopPropagation(); return true;" title="Ajouter au panier">
                                        <i class="fas fa-shopping-cart"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnRemoveFromWishlist" runat="server"
                                        CommandArgument='<%# Eval("Id") %>' CommandName="RemoveFromWishlist"
                                        CssClass="btn btn-icon btn-wishlist active"
                                        OnClientClick="event.stopPropagation(); return true;"
                                        title="Retirer des favoris">
                                        <i class="fas fa-heart"></i>
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
        </div>
        </FooterTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlEmptyWishlist" runat="server" Visible="false">
            <div class="empty-wishlist">
                <i class="far fa-heart"></i>
                <h3>Votre liste de favoris est vide</h3>
                <p>Ajoutez des produits à vos favoris pour les retrouver facilement.</p>
                <a href="Shop.aspx" class="btn btn-primary">
                    <i class="fas fa-store"></i> Découvrir la boutique
                </a>
            </div>
        </asp:Panel>
        </asp:Panel>
        </div>
    </asp:Content>