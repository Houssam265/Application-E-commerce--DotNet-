<%@ Page Title="Mon Panier" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Cart.aspx.cs" Inherits="Ecommerce.Pages.Public.Cart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .cart-container {
            max-width: 1000px;
            margin: 2rem auto;
        }

        .cart-header {
            margin-bottom: 2rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid var(--border-color);
        }

        .cart-table {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 2rem;
            background: var(--bg-white);
            border-radius: 12px;
            overflow: hidden;
            border: 1px solid var(--border-color);
            box-shadow: var(--shadow-md);
            transition: var(--transition);
        }

        .cart-table:hover {
            box-shadow: var(--shadow-lg);
        }

        .cart-table th,
        .cart-table td {
            padding: 1.5rem;
            text-align: left;
            border-bottom: 1px solid var(--border-color);
        }

        .cart-table th {
            background: var(--bg-light);
            font-weight: 600;
            color: var(--text-dark);
        }

        .cart-table tbody tr:last-child td {
            border-bottom: none;
        }

        .cart-table tbody tr {
            transition: var(--transition);
        }

        .cart-table tbody tr:hover {
            background: rgba(40, 167, 69, 0.03);
            transform: scale(1.01);
        }

        .cart-item-img {
            width: 80px;
            height: 80px;
            object-fit: cover;
            border-radius: 8px;
            border: 1px solid var(--border-color);
            transition: var(--transition);
            cursor: pointer;
        }

        .cart-item-img:hover {
            transform: scale(1.1);
            box-shadow: var(--shadow-sm);
        }

        .cart-item-info {
            display: flex;
            gap: 1rem;
            align-items: center;
        }

        .cart-item-name {
            font-weight: 600;
            color: var(--text-dark);
            margin-bottom: 0.25rem;
        }

        .cart-item-variant {
            font-size: 0.9rem;
            color: var(--text-light);
        }

        .qty-controls {
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .qty-btn {
            width: 35px;
            height: 35px;
            border: 1px solid var(--border-color);
            background: var(--bg-white);
            border-radius: 5px;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: all 0.3s;
        }

        .qty-btn:hover {
            background: var(--primary-color);
            color: #fff;
            border-color: var(--primary-color);
            transform: scale(1.1);
        }

        .qty-btn:active {
            transform: scale(0.95);
        }

        .qty-input {
            width: 60px;
            padding: 8px;
            border: 1px solid var(--border-color);
            border-radius: 5px;
            text-align: center;
        }

        .cart-summary {
            background: var(--bg-white);
            padding: 2rem;
            border-radius: 12px;
            border: 1px solid var(--border-color);
            box-shadow: var(--shadow-md);
            transition: var(--transition);
            position: sticky;
            top: 120px;
        }

        .cart-summary:hover {
            box-shadow: var(--shadow-lg);
        }

        .summary-row {
            display: flex;
            justify-content: space-between;
            margin-bottom: 1rem;
            padding-bottom: 1rem;
            border-bottom: 1px solid var(--border-color);
        }

        .summary-row:last-child {
            border-bottom: none;
        }

        .total-row {
            font-weight: 700;
            font-size: 1.3rem;
            color: var(--primary-color);
            margin-top: 1rem;
            padding-top: 1rem;
            border-top: 2px solid var(--primary-color);
        }

        .btn-remove {
            color: var(--danger-color);
            background: none;
            border: 2px solid transparent;
            cursor: pointer;
            font-size: 0.9rem;
            padding: 8px 15px;
            border-radius: 8px;
            transition: var(--transition);
            font-weight: 600;
        }

        .btn-remove:hover {
            background: rgba(220,53,69,0.1);
            border-color: var(--danger-color);
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(220, 53, 69, 0.2);
        }

        .empty-cart {
            text-align: center;
            padding: 4rem 2rem;
        }

        .empty-cart-icon {
            font-size: 5rem;
            color: var(--text-light);
            margin-bottom: 1rem;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="cart-container">
            <div class="cart-header">
                <h1><i class="fas fa-shopping-cart" style="color: var(--primary-color);"></i> Mon Panier</h1>
            </div>

            <asp:Panel ID="pnlEmptyCart" runat="server">
                <div class="empty-cart">
                    <div class="empty-cart-icon">
                        <i class="fas fa-shopping-cart"></i>
                    </div>
                    <h2 style="margin-bottom: 1rem;">Votre panier est vide</h2>
                    <p style="color: var(--text-light); margin-bottom: 2rem;">Ajoutez des produits à votre panier pour commencer vos achats.</p>
                    <a href="Shop.aspx" class="btn btn-primary">
                        <i class="fas fa-store"></i> Continuer vos achats
                    </a>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlCartItems" runat="server" Visible="false">
                <table class="cart-table">
                    <thead>
                        <tr>
                            <th>Produit</th>
                            <th>Prix unitaire</th>
                            <th>Quantité</th>
                            <th>Total</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptCartItems" runat="server" OnItemCommand="rptCartItems_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <div class="cart-item-info">
                                            <img src='<%# GetImageUrl(Eval("ImageUrl")) %>' class="cart-item-img"
                                                onerror="this.src='https://via.placeholder.com/80/f5f5f5/051922?text=Produit'" />
                                            <div>
                                                <div class="cart-item-name"><%# Eval("Name") %></div>
                                                <%# Eval("VariantInfo") != null && !string.IsNullOrEmpty(Eval("VariantInfo").ToString()) ? "<div class='cart-item-variant'>" + Eval("VariantInfo") + "</div>" : "" %>
                                            </div>
                                        </div>
                                    </td>
                                    <td>
                                        <span style="font-weight: 600; color: var(--text-dark);">
                                            <%# Eval("UnitPrice", "{0:F2}") %> MAD
                                        </span>
                                    </td>
                                    <td>
                                        <div class="qty-controls">
                                            <asp:LinkButton ID="btnDecrease" runat="server" CommandName="Decrease" 
                                                CommandArgument='<%# Eval("CartId") %>' CssClass="qty-btn">
                                                <i class="fas fa-minus"></i>
                                            </asp:LinkButton>
                                            <asp:TextBox ID="txtQty" runat="server" Text='<%# Eval("Quantity") %>' 
                                                CssClass="qty-input" ReadOnly="true" style="pointer-events: none;" />
                                            <asp:LinkButton ID="btnIncrease" runat="server" CommandName="Increase" 
                                                CommandArgument='<%# Eval("CartId") %>' CssClass="qty-btn">
                                                <i class="fas fa-plus"></i>
                                            </asp:LinkButton>
                                        </div>
                                    </td>
                                    <td>
                                        <span style="font-weight: 700; color: var(--primary-color); font-size: 1.1rem;">
                                            <%# Eval("TotalPrice", "{0:F2}") %> MAD
                                        </span>
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="btnRemove" runat="server" CommandName="Remove"
                                            CommandArgument='<%# Eval("CartId") %>' CssClass="btn-remove">
                                            <i class="fas fa-trash"></i> Supprimer
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>

                <div style="display: grid; grid-template-columns: 1fr 350px; gap: 2rem;">
                    <div>
                        <a href="Shop.aspx" class="btn btn-outline">
                            <i class="fas fa-arrow-left"></i> Continuer vos achats
                        </a>
                    </div>
                    <div class="cart-summary">
                        <h3 style="margin-bottom: 1.5rem;">Récapitulatif</h3>
                        <div class="summary-row">
                            <span>Sous-total</span>
                            <span><asp:Label ID="lblSubTotal" runat="server"></asp:Label> MAD</span>
                        </div>
                        <div class="summary-row">
                            <span>Livraison</span>
                            <span><asp:Label ID="lblShipping" runat="server"></asp:Label></span>
                        </div>
                        <div class="summary-row total-row">
                            <span>Total</span>
                            <span><asp:Label ID="lblTotal" runat="server"></asp:Label> MAD</span>
                        </div>
                        <asp:Button ID="btnCheckout" runat="server" Text="Passer la commande" CssClass="btn btn-primary"
                            Style="width: 100%; padding: 15px; font-size: 16px; margin-top: 1.5rem;" 
                            OnClick="btnCheckout_Click" />
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
