<%@ Page Title="Mon Panier" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Cart.aspx.cs" Inherits="Ecommerce.Pages.Public.Cart" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .cart-container {
                max-width: 900px;
                margin: 2rem auto;
            }

            .cart-table {
                width: 100%;
                border-collapse: collapse;
                margin-bottom: 2rem;
                background: var(--secondary-bg);
                border-radius: 12px;
                overflow: hidden;
                border: 1px solid var(--glass-border);
            }

            .cart-table th,
            .cart-table td {
                padding: 1rem;
                text-align: left;
                border-bottom: 1px solid var(--glass-border);
            }

            .cart-table th {
                background: rgba(15, 23, 42, 0.8);
                font-weight: 600;
                color: var(--text-muted);
            }

            .cart-item-img {
                width: 60px;
                height: 60px;
                object-fit: cover;
                border-radius: 8px;
            }

            .cart-summary {
                background: var(--secondary-bg);
                padding: 1.5rem;
                border-radius: 12px;
                border: 1px solid var(--glass-border);
                text-align: right;
            }

            .btn-remove {
                color: #ef4444;
                background: none;
                border: none;
                cursor: pointer;
                font-size: 0.9rem;
            }

            .btn-remove:hover {
                text-decoration: underline;
            }
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="cart-container">
            <h1 style="margin-bottom: 2rem;">Mon Panier</h1>

            <asp:Panel ID="pnlEmptyCart" runat="server">
                <div style="text-align: center; padding: 4rem;">
                    <p style="font-size: 1.2rem; margin-bottom: 1rem; color: var(--text-muted);">Votre panier est vide.
                    </p>
                    <a href="Shop.aspx" class="btn btn-primary">Continuer vos achats</a>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlCartItems" runat="server" Visible="false">
                <asp:Repeater ID="rptCartItems" runat="server" OnItemCommand="rptCartItems_ItemCommand">
                    <HeaderTemplate>
                        <table class="cart-table">
                            <thead>
                                <tr>
                                    <th>Produit</th>
                                    <th>Prix</th>
                                    <th>Quantité</th>
                                    <th>Total</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td style="display: flex; gap: 1rem; align-items: center;">
                                <img src='/Assets/Images/Products/<%# Eval("ImageUrl") %>' class="cart-item-img"
                                    onerror="this.src='https://via.placeholder.com/60'" />
                                <span>
                                    <%# Eval("Name") %>
                                </span>
                            </td>
                            <td>
                                <%# Eval("Price", "{0:C}" ) %>
                            </td>
                            <td>
                                <%# Eval("Quantity") %>
                            </td>
                            <td style="color: var(--accent); font-weight: 600;">
                                <%# Eval("Total", "{0:C}" ) %>
                            </td>
                            <td>
                                <asp:LinkButton ID="btnRemove" runat="server" CommandName="Remove"
                                    CommandArgument='<%# Eval("ProductId") + "|" + Eval("VariantId") %>'
                                    CssClass="btn-remove">Supprimer</asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>

                <div class="cart-summary">
                    <div style="font-size: 1.5rem; margin-bottom: 1rem;">
                        Total: <asp:Label ID="lblTotal" runat="server" Text="0.00 €"
                            style="color: var(--accent); font-weight: 700;"></asp:Label>
                    </div>
                    <div style="display: flex; gap: 1rem; justify-content: flex-end;">
                        <a href="Shop.aspx" class="btn" style="background: rgba(255,255,255,0.1);">Continuer</a>
                        <asp:Button ID="btnCheckout" runat="server" Text="Commander" CssClass="btn btn-primary"
                            OnClick="btnCheckout_Click" />
                    </div>
                </div>
            </asp:Panel>
        </div>
    </asp:Content>