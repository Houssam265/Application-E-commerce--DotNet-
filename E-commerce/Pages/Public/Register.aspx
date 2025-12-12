<%@ Page Title="Inscription" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="Ecommerce.Pages.Public.Register" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .auth-container {
                max-width: 500px;
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
                <h2 style="margin-bottom: 1rem;">Créer un compte</h2>
                <p style="color: var(--text-muted); margin-bottom: 2rem;">Rejoignez-nous pour une expérience exclusive
                </p>

                <asp:Panel ID="pnlError" runat="server" Visible="false"
                    Style="background: rgba(239, 68, 68, 0.2); color: #fca5a5; padding: 1rem; border-radius: 8px; margin-bottom: 1rem;">
                    <asp:Literal ID="litError" runat="server"></asp:Literal>
                </asp:Panel>

                <div class="form-group">
                    <label>Nom complet</label>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Téléphone</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" TextMode="Phone"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Mot de passe</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password">
                    </asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Confirmer le mot de passe</label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password">
                    </asp:TextBox>
                </div>

                <asp:Button ID="btnRegister" runat="server" Text="S'inscrire" CssClass="btn btn-primary"
                    Style="width: 100%; margin-bottom: 1rem;" OnClick="btnRegister_Click" />

                <p style="font-size: 0.9rem;">
                    Déjà un compte ? <a href="Login.aspx" style="color: var(--accent);">Se connecter</a>
                </p>
            </div>
        </div>
    </asp:Content>