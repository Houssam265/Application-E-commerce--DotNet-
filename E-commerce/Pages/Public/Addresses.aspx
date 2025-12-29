<%@ Page Title="Mes Adresses" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Addresses.aspx.cs" Inherits="Ecommerce.Pages.Public.Addresses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .addresses-page {
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            min-height: calc(100vh - 200px);
            padding: 3rem 1rem;
        }

        .addresses-container {
            max-width: 700px;
            margin: 0 auto;
        }

        .addresses-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 2.5rem;
            padding: 1.5rem;
            background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
            border-radius: 16px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
        }

        .addresses-header h1 {
            margin: 0;
            font-size: 2rem;
            color: #1e293b;
            display: flex;
            align-items: center;
            gap: 0.75rem;
        }

        .addresses-header h1 i {
            color: var(--primary-color);
            font-size: 2rem;
        }

        .address-form-card {
            background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
            border-radius: 20px;
            padding: 2.5rem;
            box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
            border: 1px solid rgba(255, 255, 255, 0.5);
            transition: all 0.3s ease;
        }

        .address-form-card:hover {
            box-shadow: 0 12px 48px rgba(0, 0, 0, 0.15);
            transform: translateY(-2px);
        }

        .address-form-card h3 {
            margin: 0 0 2rem 0;
            font-size: 1.5rem;
            color: #1e293b;
            font-weight: 700;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .address-form-card h3::before {
            content: '';
            width: 4px;
            height: 24px;
            background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-light) 100%);
            border-radius: 2px;
        }

        .form-group {
            margin-bottom: 1.5rem;
        }

        .form-group label {
            display: block;
            margin-bottom: 0.5rem;
            font-weight: 600;
            color: #475569;
            font-size: 0.95rem;
        }

        .form-control {
            width: 100%;
            padding: 0.875rem 1.25rem;
            border: 2px solid #e2e8f0;
            border-radius: 12px;
            font-size: 1rem;
            transition: all 0.3s ease;
            background: #ffffff;
            color: #1e293b;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--primary-color);
            box-shadow: 0 0 0 3px rgba(34, 197, 94, 0.1);
            transform: translateY(-1px);
        }

        .form-grid {
            display: grid;
            grid-template-columns: 2fr 1fr;
            gap: 1.25rem;
        }

        .checkbox-group {
            display: flex;
            align-items: center;
            gap: 0.75rem;
            padding: 1rem;
            background: #f1f5f9;
            border-radius: 12px;
            margin-top: 0.5rem;
            transition: all 0.3s ease;
        }

        .checkbox-group:hover {
            background: #e2e8f0;
        }

        .checkbox-group input[type="checkbox"] {
            width: 20px;
            height: 20px;
            cursor: pointer;
            accent-color: var(--primary-color);
        }

        .checkbox-group label {
            margin: 0;
            cursor: pointer;
            font-weight: 500;
            color: #475569;
        }

        .form-actions {
            display: flex;
            gap: 1rem;
            margin-top: 2rem;
            padding-top: 2rem;
            border-top: 2px solid #e2e8f0;
        }

        .btn-primary {
            flex: 1;
            padding: 1rem 2rem;
            background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-light) 100%);
            color: white;
            border: none;
            border-radius: 12px;
            font-weight: 600;
            font-size: 1rem;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 4px 12px rgba(34, 197, 94, 0.3);
        }

        .btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(34, 197, 94, 0.4);
        }

        .btn-outline {
            padding: 1rem 2rem;
            background: transparent;
            color: #64748b;
            border: 2px solid #e2e8f0;
            border-radius: 12px;
            font-weight: 600;
            font-size: 1rem;
            text-decoration: none;
            transition: all 0.3s ease;
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
        }

        .btn-outline:hover {
            background: #f1f5f9;
            border-color: #cbd5e1;
            color: #1e293b;
            transform: translateY(-1px);
        }

        .alert {
            padding: 1.25rem 1.5rem;
            border-radius: 12px;
            margin-bottom: 1.5rem;
            font-weight: 500;
            display: flex;
            align-items: center;
            gap: 0.75rem;
            animation: slideIn 0.3s ease;
        }

        @keyframes slideIn {
            from {
                opacity: 0;
                transform: translateY(-10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .alert-success {
            background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
            color: #065f46;
            border: 2px solid #6ee7b7;
        }

        .alert-danger {
            background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
            color: #991b1b;
            border: 2px solid #fca5a5;
        }

        .alert i {
            font-size: 1.25rem;
        }

        @media (max-width: 768px) {
            .form-grid {
                grid-template-columns: 1fr;
            }

            .addresses-header {
                flex-direction: column;
                gap: 1rem;
                align-items: flex-start;
            }

            .address-form-card {
                padding: 1.5rem;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="addresses-page">
        <div class="addresses-container">
            <div class="addresses-header">
                <h1>
                    <i class="fas fa-map-marker-alt"></i>
                    Gestion des Adresses
                </h1>
                <a href="Profile.aspx?tab=addresses" class="btn btn-outline">
                    <i class="fas fa-arrow-left"></i> Retour
                </a>
            </div>

            <div class="address-form-card">
                <h3>
                    <asp:Label ID="lblTitle" runat="server" Text="Ajouter une adresse"></asp:Label>
                </h3>

                <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
                    <i class="fas fa-check-circle"></i>
                    <asp:Literal ID="litSuccess" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                    <i class="fas fa-exclamation-circle"></i>
                    <asp:Literal ID="litError" runat="server"></asp:Literal>
                </asp:Panel>

                <asp:HiddenField ID="hfAddressId" runat="server" />

                <div class="form-group">
                    <label><i class="fas fa-user"></i> Nom complet *</label>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Ex: Ahmed Benali" required></asp:TextBox>
                </div>

                <div class="form-group">
                    <label><i class="fas fa-road"></i> Rue *</label>
                    <asp:TextBox ID="txtStreet" runat="server" CssClass="form-control" placeholder="Ex: 123 Avenue Mohammed V" required></asp:TextBox>
                </div>

                <div class="form-grid">
                    <div class="form-group">
                        <label><i class="fas fa-city"></i> Ville *</label>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" placeholder="Ex: Casablanca" required></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label><i class="fas fa-mail-bulk"></i> Code Postal *</label>
                        <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-control" placeholder="Ex: 20000" required></asp:TextBox>
                    </div>
                </div>

                <div class="form-group">
                    <label><i class="fas fa-globe"></i> Pays *</label>
                    <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control">
                        <asp:ListItem Value="Maroc" Selected="True">🇲🇦 Maroc</asp:ListItem>
                        <asp:ListItem Value="France">🇫🇷 France</asp:ListItem>
                        <asp:ListItem Value="Belgique">🇧🇪 Belgique</asp:ListItem>
                        <asp:ListItem Value="Canada">🇨🇦 Canada</asp:ListItem>
                        <asp:ListItem Value="Autre">🌍 Autre</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <label><i class="fas fa-phone"></i> Téléphone</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" TextMode="Phone" placeholder="Ex: +212 6XX XXX XXX"></asp:TextBox>
                </div>

                <div class="form-group">
                    <div class="checkbox-group">
                        <asp:CheckBox ID="chkIsDefault" runat="server" />
                        <label for="<%= chkIsDefault.ClientID %>">
                            <i class="fas fa-star" style="color: #fbbf24;"></i> Définir comme adresse par défaut
                        </label>
                    </div>
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnSave" runat="server" Text="Enregistrer" CssClass="btn btn-primary" 
                        OnClick="btnSave_Click" />
                    <a href="Profile.aspx?tab=addresses" class="btn btn-outline">
                        <i class="fas fa-times"></i> Annuler
                    </a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

