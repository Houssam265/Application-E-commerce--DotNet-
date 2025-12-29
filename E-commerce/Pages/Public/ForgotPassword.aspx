<%@ Page Title="Mot de passe oublié" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ForgotPassword.aspx.cs" Inherits="Ecommerce.Pages.Public.ForgotPassword" %>

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
                    <h2><i class="fas fa-key" style="color: var(--primary-color);"></i> Mot de passe oublié</h2>
                    <p>Entrez votre email pour recevoir un lien de réinitialisation</p>
                </div>

                <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                    <i class="fas fa-exclamation-circle"></i> <asp:Literal ID="litError" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
                    <i class="fas fa-check-circle"></i> <asp:Literal ID="litSuccess" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:Panel ID="pnlForm" runat="server">
                    <div class="form-group">
                        <label><i class="fas fa-envelope"></i> Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" 
                            placeholder="votre@email.com" required></asp:TextBox>
                    </div>

                    <asp:Button ID="btnSend" runat="server" Text="Envoyer le code de vérification" 
                        CssClass="btn btn-primary" Style="width: 100%; padding: 12px; font-size: 16px; margin-bottom: 1rem;" 
                        OnClick="btnSend_Click" />

                    <div style="text-align: center;">
                        <a href="Login.aspx" style="color: var(--primary-color);">
                            <i class="fas fa-arrow-left"></i> Retour à la connexion
                        </a>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlVerifyCode" runat="server" Visible="false">
                    <div class="form-group">
                        <label><i class="fas fa-key"></i> Code de vérification</label>
                        <asp:TextBox ID="txtVerificationCode" runat="server" CssClass="form-control" 
                            placeholder="Entrez le code à 6 chiffres" MaxLength="6" required></asp:TextBox>
                        <small class="text-muted">Un code de vérification a été envoyé à votre email</small>
                    </div>

                    <asp:Button ID="btnVerifyCode" runat="server" Text="Vérifier le code" 
                        CssClass="btn btn-primary" Style="width: 100%; padding: 12px; font-size: 16px; margin-bottom: 1rem;" 
                        OnClick="btnVerifyCode_Click" />

                    <div style="text-align: center; margin-top: 1rem;">
                        <asp:LinkButton ID="btnResendCode" runat="server" OnClick="btnResendCode_Click" 
                            CssClass="btn btn-link" Style="color: var(--primary-color);">
                            <i class="fas fa-redo"></i> Renvoyer le code
                        </asp:LinkButton>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>

