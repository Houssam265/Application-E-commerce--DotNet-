<%@ Page Title="Mes Adresses" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Addresses.aspx.cs" Inherits="Ecommerce.Pages.Public.Addresses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div style="max-width: 600px; margin: 2rem auto;">
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem;">
                <h1><i class="fas fa-map-marker-alt" style="color: var(--primary-color);"></i> Gestion des Adresses</h1>
                <a href="Profile.aspx" class="btn btn-outline">
                    <i class="fas fa-arrow-left"></i> Retour
                </a>
            </div>

            <div class="card">
                <h3 style="margin-bottom: 1.5rem;">
                    <asp:Label ID="lblTitle" runat="server" Text="Ajouter une adresse"></asp:Label>
                </h3>

                <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
                    <asp:Literal ID="litSuccess" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                    <asp:Literal ID="litError" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:HiddenField ID="hfAddressId" runat="server" />

                <div class="form-group">
                    <label>Nom complet *</label>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" required></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Rue *</label>
                    <asp:TextBox ID="txtStreet" runat="server" CssClass="form-control" required></asp:TextBox>
                </div>

                <div style="display: grid; grid-template-columns: 2fr 1fr; gap: 1rem;">
                    <div class="form-group">
                        <label>Ville *</label>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" required></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Code Postal *</label>
                        <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-control" required></asp:TextBox>
                    </div>
                </div>

                <div class="form-group">
                    <label>Pays *</label>
                    <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control">
                        <asp:ListItem Value="Maroc" Selected="True">Maroc</asp:ListItem>
                        <asp:ListItem Value="France">France</asp:ListItem>
                        <asp:ListItem Value="Belgique">Belgique</asp:ListItem>
                        <asp:ListItem Value="Canada">Canada</asp:ListItem>
                        <asp:ListItem Value="Autre">Autre</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <label>Téléphone</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" TextMode="Phone"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>
                        <asp:CheckBox ID="chkIsDefault" runat="server" />
                        Définir comme adresse par défaut
                    </label>
                </div>

                <div style="display: flex; gap: 1rem; margin-top: 1.5rem;">
                    <asp:Button ID="btnSave" runat="server" Text="Enregistrer" CssClass="btn btn-primary" 
                        OnClick="btnSave_Click" />
                    <a href="Profile.aspx" class="btn btn-outline">Annuler</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

