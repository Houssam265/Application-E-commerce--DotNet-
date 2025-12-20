<%@ Page Title="Inscription" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="Ecommerce.Pages.Public.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .auth-container {
            max-width: 500px;
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

        .form-group {
            margin-bottom: 1.5rem;
        }

        .form-group label {
            display: block;
            margin-bottom: 0.5rem;
            color: var(--text-dark);
            font-weight: 600;
        }

        .form-control {
            width: 100%;
            padding: 12px 15px;
            border: 1px solid var(--border-color);
            border-radius: 5px;
            font-size: 14px;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--primary-color);
            box-shadow: 0 0 0 3px rgba(40, 167, 69, 0.1);
        }

        .password-strength {
            font-size: 12px;
            margin-top: 5px;
            color: var(--text-light);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="auth-container">
            <div class="card">
                <div class="auth-header">
                    <h2><i class="fas fa-user-plus" style="color: var(--primary-color);"></i> Créer un compte</h2>
                    <p>Rejoignez-nous pour découvrir les produits des coopératives marocaines</p>
                </div>

                <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                    <i class="fas fa-exclamation-circle"></i> <asp:Literal ID="litError" runat="server"></asp:Literal>
                </asp:Panel>

                <div class="form-group">
                    <label><i class="fas fa-user"></i> Nom complet *</label>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" 
                        placeholder="Votre nom complet" required></asp:TextBox>
                </div>

                <div class="form-group">
                    <label><i class="fas fa-envelope"></i> Email *</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" 
                        placeholder="votre@email.com" required></asp:TextBox>
                </div>

                <div class="form-group">
                    <label><i class="fas fa-phone"></i> Téléphone</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" TextMode="Phone" 
                        placeholder="06XX XXX XXX"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label><i class="fas fa-lock"></i> Mot de passe *</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" 
                        placeholder="Minimum 6 caractères" required></asp:TextBox>
                    <div class="password-strength">Le mot de passe doit contenir au moins 6 caractères</div>
                </div>

                <div class="form-group">
                    <label><i class="fas fa-lock"></i> Confirmer le mot de passe *</label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" 
                        placeholder="Confirmez votre mot de passe" required></asp:TextBox>
                </div>

                <asp:Button ID="btnRegister" runat="server" Text="S'inscrire" CssClass="btn btn-primary"
                    Style="width: 100%; padding: 12px; font-size: 16px; margin-bottom: 1rem;" 
                    OnClick="btnRegister_Click" />

                <div style="text-align: center;">
                    <p style="margin-bottom: 1rem;">Déjà un compte ?</p>
                    <a href="Login.aspx" class="btn btn-secondary" style="width: 100%;">
                        <i class="fas fa-sign-in-alt"></i> Se connecter
                    </a>
                </div>

                <div style="margin-top: 2rem; padding-top: 1.5rem; border-top: 1px solid var(--border-color); text-align: center;">
                    <p style="font-size: 14px; color: var(--text-light); margin: 0;">
                        Vous représentez une coopérative ? 
                        <a href="RegisterCooperative.aspx" style="color: var(--primary-color); font-weight: 600;">
                            Inscrivez votre coopérative
                        </a>
                    </p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
