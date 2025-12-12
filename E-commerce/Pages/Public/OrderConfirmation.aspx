<%@ Page Title="Commande Confirmée" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="OrderConfirmation.aspx.cs" Inherits="Ecommerce.Pages.Public.OrderConfirmation" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div style="text-align: center; margin-top: 4rem;">
            <div style="font-size: 4rem; margin-bottom: 1rem;">🎉</div>
            <h1 style="color: #10b981; margin-bottom: 1rem;">Merci pour votre commande !</h1>
            <p style="font-size: 1.2rem; color: var(--text-muted); margin-bottom: 2rem;">
                Votre commande #<asp:Label ID="lblOrderId" runat="server"></asp:Label> a été enregistrée avec succès.
            </p>
            <a href="Shop.aspx" class="btn btn-primary">Retour à la boutique</a>
        </div>
    </asp:Content>