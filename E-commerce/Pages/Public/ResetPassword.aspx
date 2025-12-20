<%@ Page Title="Réinitialiser le mot de passe" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ResetPassword.aspx.cs" Inherits="Ecommerce.Pages.Public.ResetPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .auth-container {
            max-width: 450px;
            margin: 4rem auto;
        }

        .auth-header {
            text-align: center;
            margin-bottom: 2rem;
        }

        .auth-header h2 {
            color: var(--text-dark);
            margin-bottom: 0.5rem;
        }

        .auth-header p {
            color: var(--text-light);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="auth-container">
            <div class="card">
                <div class="auth-header">
                    <h2><i class="fas fa-key" style="color: var(--primary-color);"></i> Réinitialiser le mot de passe</h2>
                    <p>Entrez votre nouveau mot de passe</p>
                </div>

                <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                    <i class="fas fa-exclamation-circle"></i> <asp:Literal ID="litError" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
                    <i class="fas fa-check-circle"></i> <asp:Literal ID="litSuccess" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:Panel ID="pnlForm" runat="server">
                    <div class="form-group">
                        <label><i class="fas fa-lock"></i> Nouveau mot de passe *</label>
                        <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" 
                            placeholder="Minimum 6 caractères" required></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label><i class="fas fa-lock"></i> Confirmer le mot de passe *</label>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" 
                            placeholder="Confirmez votre mot de passe" required></asp:TextBox>
                    </div>

                    <asp:Button ID="btnReset" runat="server" Text="Réinitialiser" CssClass="btn btn-primary"
                        Style="width: 100%; padding: 12px; font-size: 16px; margin-bottom: 1rem;" 
                        OnClick="btnReset_Click" />

                    <div style="text-align: center;">
                        <a href="Login.aspx" style="color: var(--primary-color);">
                            <i class="fas fa-arrow-left"></i> Retour à la connexion
                        </a>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>

