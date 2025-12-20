<%@ Page Title="Connexion" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="Ecommerce.Pages.Public.Login" %>

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

        .forgot-password {
            text-align: right;
            margin-bottom: 1.5rem;
        }

        .forgot-password a {
            color: var(--primary-color);
            font-size: 14px;
        }

        .divider {
            text-align: center;
            margin: 2rem 0;
            position: relative;
        }

        .divider::before {
            content: '';
            position: absolute;
            left: 0;
            top: 50%;
            width: 100%;
            height: 1px;
            background: var(--border-color);
        }

        .divider span {
            background: var(--bg-white);
            padding: 0 1rem;
            position: relative;
            color: var(--text-light);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="auth-container">
            <div class="card">
                <div class="auth-header">
                    <h2><i class="fas fa-sign-in-alt" style="color: var(--primary-color);"></i> Connexion</h2>
                    <p>Connectez-vous pour accéder à votre compte</p>
                </div>

                <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                    <i class="fas fa-exclamation-circle"></i> <asp:Literal ID="litError" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
                    <i class="fas fa-check-circle"></i> <asp:Literal ID="litSuccess" runat="server"></asp:Literal>
                </asp:Panel>

                <div class="form-group">
                    <label><i class="fas fa-envelope"></i> Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" 
                        placeholder="votre@email.com" required></asp:TextBox>
                </div>

                <div class="form-group">
                    <label><i class="fas fa-lock"></i> Mot de passe</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" 
                        placeholder="••••••••" required></asp:TextBox>
                </div>

                <div class="forgot-password">
                    <a href="ForgotPassword.aspx">Mot de passe oublié ?</a>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Se Connecter" CssClass="btn btn-primary"
                    Style="width: 100%; padding: 12px; font-size: 16px; margin-bottom: 1rem;" 
                    OnClick="btnLogin_Click" />

                <div class="divider">
                    <span>ou</span>
                </div>

                <div style="text-align: center;">
                    <p style="margin-bottom: 1rem;">Pas encore de compte ?</p>
                    <a href="Register.aspx" class="btn btn-secondary" style="width: 100%;">
                        <i class="fas fa-user-plus"></i> Créer un compte
                    </a>
                </div>

                <div style="margin-top: 2rem; padding-top: 1.5rem; border-top: 1px solid var(--border-color); text-align: center;">
                    <p style="font-size: 14px; color: var(--text-light); margin: 0;">
                        Vous êtes une coopérative ? 
                        <a href="RegisterCooperative.aspx" style="color: var(--primary-color); font-weight: 600;">
                            Inscrivez-vous ici
                        </a>
                    </p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
