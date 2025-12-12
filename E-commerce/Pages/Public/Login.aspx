<%@ Page Title="Connexion" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="Ecommerce.Pages.Public.Login" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .auth-container {
                max-width: 400px;
                margin: 4rem auto;
                text-align: center;
            }

            .form-group {
                margin-bottom: 1.5rem;
                text-align: left;
            }

            .form-group label {
                display: block;
                margin-bottom: 0.5rem;
                color: var(--text-muted);
            }

            .form-control {
                width: 100%;
                padding: 0.75rem;
                background: rgba(255, 255, 255, 0.05);
                border: 1px solid var(--glass-border);
                border-radius: 8px;
                color: white;
                font-size: 1rem;
            }

            .form-control:focus {
                outline: none;
                border-color: var(--accent);
            }
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="auth-container">
            <div class="card">
                <h2 style="margin-bottom: 1rem;">Bienvenue</h2>
                <p style="color: var(--text-muted); margin-bottom: 2rem;">Connectez-vous pour accéder à votre compte</p>

                <asp:Panel ID="pnlError" runat="server" Visible="false"
                    Style="background: rgba(239, 68, 68, 0.2); color: #fca5a5; padding: 1rem; border-radius: 8px; margin-bottom: 1rem;">
                    <asp:Literal ID="litError" runat="server"></asp:Literal>
                </asp:Panel>

                <div class="form-group">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Mot de passe</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password">
                    </asp:TextBox>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Se Connecter" CssClass="btn btn-primary"
                    Style="width: 100%; margin-bottom: 1rem;" OnClick="btnLogin_Click" />

                <p style="font-size: 0.9rem;">
                    Pas encore de compte ? <a href="Register.aspx" style="color: var(--accent);">Créer un compte</a>
                </p>
            </div>
        </div>
    </asp:Content>