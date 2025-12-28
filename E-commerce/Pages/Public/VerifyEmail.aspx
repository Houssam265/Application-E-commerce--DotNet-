<%@ Page Title="Vérifier l'email" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VerifyEmail.aspx.cs" Inherits="Ecommerce.Pages.Public.VerifyEmail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .verify-container { max-width: 600px; margin: 3rem auto; }
        .card { background: var(--bg-white); border: 1px solid var(--border-color); border-radius: 10px; padding: 2rem; }
        .code-input { letter-spacing: 6px; text-transform: uppercase; font-weight: 600; text-align: center; }
        .actions { display: flex; gap: 1rem; margin-top: 1rem; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container verify-container">
        <div class="card">
            <h2><i class="fas fa-envelope-open-text" style="color: var(--primary-color);"></i> Vérification d'email</h2>
            <p>Un code de vérification a été envoyé à <strong><asp:Label ID="lblEmail" runat="server"></asp:Label></strong>. Saisissez-le ci-dessous pour activer votre compte.</p>

            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                <i class="fas fa-exclamation-circle"></i> <asp:Literal ID="litError" runat="server"></asp:Literal>
            </asp:Panel>
            <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
                <i class="fas fa-check-circle"></i> <asp:Literal ID="litSuccess" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="form-group">
                <label>Code de vérification</label>
                <asp:TextBox ID="txtCode" runat="server" CssClass="form-control code-input" MaxLength="6" placeholder="••••••" required></asp:TextBox>
            </div>
            <div class="actions">
                <asp:Button ID="btnVerify" runat="server" CssClass="btn btn-primary" Text="Vérifier" OnClick="btnVerify_Click" />
                <asp:Button ID="btnResend" runat="server" CssClass="btn btn-secondary" Text="Renvoyer le code" OnClick="btnResend_Click" />
            </div>
        </div>
    </div>
</asp:Content>