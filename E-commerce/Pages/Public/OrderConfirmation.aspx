<%@ Page Title="Commande Confirmée" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="OrderConfirmation.aspx.cs" Inherits="Ecommerce.Pages.Public.OrderConfirmation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .confirmation-container {
            max-width: 800px;
            margin: 4rem auto;
            text-align: center;
        }

        .success-icon {
            font-size: 5rem;
            color: var(--success-color);
            margin-bottom: 2rem;
        }

        .order-details {
            background: var(--bg-white);
            padding: 2rem;
            border-radius: 10px;
            border: 1px solid var(--border-color);
            margin: 2rem 0;
            text-align: left;
        }

        .detail-row {
            display: flex;
            justify-content: space-between;
            padding: 1rem 0;
            border-bottom: 1px solid var(--border-color);
        }

        .detail-row:last-child {
            border-bottom: none;
        }

        .detail-label {
            font-weight: 600;
            color: var(--text-dark);
        }

        .detail-value {
            color: var(--text-light);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="confirmation-container">
            <div class="success-icon">
                <i class="fas fa-check-circle"></i>
            </div>
            <h1 style="color: var(--success-color); margin-bottom: 1rem;">Commande Confirmée !</h1>
            <p style="font-size: 1.2rem; color: var(--text-light); margin-bottom: 2rem;">
                Merci pour votre commande. Nous avons bien reçu votre demande et vous contacterons bientôt.
            </p>

            <asp:Panel ID="pnlOrderDetails" runat="server">
                <div class="order-details">
                    <h3 style="margin-bottom: 1.5rem; text-align: center;">
                        <i class="fas fa-receipt" style="color: var(--primary-color);"></i> Détails de la commande
                    </h3>
                    
                    <div class="detail-row">
                        <span class="detail-label">Numéro de commande</span>
                        <span class="detail-value"><asp:Label ID="lblOrderNumber" runat="server"></asp:Label></span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Date</span>
                        <span class="detail-value"><asp:Label ID="lblOrderDate" runat="server"></asp:Label></span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Total</span>
                        <span class="detail-value" style="color: var(--primary-color); font-weight: 700; font-size: 1.2rem;">
                            <asp:Label ID="lblTotal" runat="server"></asp:Label> MAD
                        </span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Statut</span>
                        <span class="detail-value">
                            <span class="badge badge-primary">En attente</span>
                        </span>
                    </div>
                </div>
            </asp:Panel>

            <div style="margin-top: 2rem;">
                <a href="Shop.aspx" class="btn btn-primary" style="margin-right: 1rem;">
                    <i class="fas fa-store"></i> Continuer vos achats
                </a>
                <a href="Profile.aspx?tab=orders" class="btn btn-outline">
                    <i class="fas fa-user"></i> Voir mes commandes
                </a>
            </div>
        </div>
    </div>
</asp:Content>
