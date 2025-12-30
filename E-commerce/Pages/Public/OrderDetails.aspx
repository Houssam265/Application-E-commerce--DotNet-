<%@ Page Title="Détails de la commande" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="OrderDetails.aspx.cs" Inherits="Ecommerce.Pages.Public.OrderDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .order-details-container {
            max-width: 900px;
            margin: 2rem auto;
        }

        .order-header {
            background: var(--bg-white);
            padding: 2rem;
            border-radius: 10px;
            border: 1px solid var(--border-color);
            margin-bottom: 2rem;
        }

        .order-info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 2rem;
            margin-bottom: 2rem;
        }

        .info-box {
            padding: 1.5rem;
            background: var(--bg-light);
            border-radius: 8px;
            border: 1px solid var(--border-color);
        }

        .info-box h4 {
            margin: 0 0 1rem 0;
            color: var(--text-dark);
            font-size: 0.9rem;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .info-box p {
            margin: 0.5rem 0;
            color: var(--text-dark);
        }

        .status-badge {
            display: inline-block;
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-weight: 600;
            font-size: 0.9rem;
        }

        .status-Pending { background: #fff3cd; color: #856404; }
        .status-Processing { background: #d1ecf1; color: #0c5460; }
        .status-Shipped { background: #cce5ff; color: #004085; }
        .status-Delivered { background: #d4edda; color: #155724; }
        .status-Cancelled { background: #f8d7da; color: #721c24; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="order-details-container">
            <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
                <div style="text-align: center; padding: 4rem;">
                    <h2>Commande non trouvée</h2>
                    <p style="color: var(--text-light); margin-bottom: 2rem;">La commande que vous recherchez n'existe pas ou vous n'avez pas l'autorisation de la consulter.</p>
                    <a href="Profile.aspx" class="btn btn-primary">Retour à mon compte</a>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlOrderDetails" runat="server">
                <div class="order-header">
                    <div style="display: flex; justify-content: space-between; align-items: start; margin-bottom: 1.5rem;">
                        <div>
                            <h1 style="margin: 0 0 0.5rem 0;">Commande <asp:Label ID="lblOrderNumber" runat="server"></asp:Label></h1>
                            <p style="margin: 0; color: var(--text-light);">
                                Passée le <asp:Label ID="lblOrderDate" runat="server"></asp:Label>
                            </p>
                        </div>
                        <div>
                            <span class="status-badge" id="statusBadge" runat="server"></span>
                        </div>
                    </div>
                </div>

                <div class="order-info-grid">
                    <div class="info-box">
                        <h4><i class="fas fa-user"></i> Informations client</h4>
                        <p><strong><asp:Label ID="lblCustomerName" runat="server"></asp:Label></strong></p>
                        <p><asp:Label ID="lblCustomerEmail" runat="server"></asp:Label></p>
                    </div>

                    <div class="info-box">
                        <h4><i class="fas fa-map-marker-alt"></i> Adresse de livraison</h4>
                        <p><asp:Label ID="lblShippingAddress" runat="server"></asp:Label></p>
                    </div>

                    <div class="info-box">
                        <h4><i class="fas fa-truck"></i> Livraison</h4>
                        <p><strong>Méthode:</strong> <asp:Label ID="lblShippingMethod" runat="server"></asp:Label></p>
                        <asp:Panel ID="pnlTracking" runat="server" Visible="false">
                            <p><strong>Numéro de suivi:</strong> <asp:Label ID="lblTrackingNumber" runat="server"></asp:Label></p>
                            <a href='OrderTracking.aspx?id=<%= Request.QueryString["id"] %>' class="btn btn-outline" style="margin-top: 0.5rem;">
                                <i class="fas fa-search"></i> Suivre la commande
                            </a>
                        </asp:Panel>
                    </div>
                    
                    <asp:Panel ID="pnlCancel" runat="server" Visible="false" CssClass="info-box">
                        <h4><i class="fas fa-ban"></i> Raison d'annulation</h4>
                        <p><asp:Label ID="lblCancelReason" runat="server"></asp:Label></p>
                    </asp:Panel>
                </div>

                <div class="card">
                    <h3 style="margin-bottom: 1.5rem;">Articles commandés</h3>
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Produit</th>
                                <th>Quantité</th>
                                <th>Prix unitaire</th>
                                <th>Total</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptOrderItems" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("ProductName") %></td>
                                        <td><%# Eval("Quantity") %></td>
                                        <td><%# Eval("UnitPrice", "{0:F2}") %> MAD</td>
                                        <td><strong><%# Eval("TotalPrice", "{0:F2}") %> MAD</strong></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>

                    <div style="margin-top: 2rem; padding-top: 1.5rem; border-top: 2px solid var(--border-color);">
                        <div style="display: flex; justify-content: space-between; margin-bottom: 0.5rem;">
                            <span>Sous-total:</span>
                            <span><asp:Label ID="lblSubTotal" runat="server"></asp:Label> MAD</span>
                        </div>
                        <div style="display: flex; justify-content: space-between; margin-bottom: 0.5rem;">
                            <span>Livraison:</span>
                            <span><asp:Label ID="lblShippingCost" runat="server"></asp:Label></span>
                        </div>
                        <asp:Panel ID="pnlDiscount" runat="server" Visible="false">
                            <div style="display: flex; justify-content: space-between; margin-bottom: 0.5rem; color: #16a34a; font-weight: 600;">
                                <span><i class="fas fa-tag"></i> Réduction:</span>
                                <span><asp:Label ID="lblDiscount" runat="server"></asp:Label></span>
                            </div>
                        </asp:Panel>
                        <div style="display: flex; justify-content: space-between; font-size: 1.3rem; font-weight: 700; color: var(--primary-color); margin-top: 1rem; padding-top: 1rem; border-top: 2px solid var(--primary-color);">
                            <span>Total:</span>
                            <span><asp:Label ID="lblTotal" runat="server"></asp:Label> MAD</span>
                        </div>
                    </div>
                </div>

                <div style="margin-top: 2rem; text-align: center;">
                    <a href="Profile.aspx?tab=orders" class="btn btn-outline">
                        <i class="fas fa-arrow-left"></i> Retour à mes commandes
                    </a>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>

