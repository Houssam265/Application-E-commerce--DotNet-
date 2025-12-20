<%@ Page Title="Mon Profil" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs" Inherits="Ecommerce.Pages.Public.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .profile-container {
            max-width: 1000px;
            margin: 2rem auto;
        }

        .profile-tabs {
            display: flex;
            gap: 1rem;
            margin-bottom: 2rem;
            border-bottom: 2px solid var(--border-color);
        }

        .tab-button {
            padding: 1rem 2rem;
            background: transparent;
            border: none;
            border-bottom: 3px solid transparent;
            cursor: pointer;
            font-size: 1rem;
            font-weight: 600;
            color: var(--text-light);
            transition: all 0.3s;
            text-decoration: none;
            display: inline-block;
        }

        .tab-button:hover {
            color: var(--primary-color);
            text-decoration: none;
        }

        .tab-button.active {
            color: var(--primary-color);
            border-bottom-color: var(--primary-color);
            text-decoration: none;
        }

        .tab-content {
            display: none;
        }

        .tab-content.active {
            display: block;
        }

        .profile-header {
            display: flex;
            align-items: center;
            gap: 2rem;
            padding: 2rem;
            background: var(--bg-white);
            border-radius: 10px;
            border: 1px solid var(--border-color);
            margin-bottom: 2rem;
        }

        .profile-avatar {
            width: 100px;
            height: 100px;
            border-radius: 50%;
            background: var(--primary-color);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 3rem;
            color: #fff;
        }

        .profile-info h2 {
            margin: 0 0 0.5rem 0;
        }

        .profile-info p {
            margin: 0;
            color: var(--text-light);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="profile-container">
            <div class="profile-header">
                <div class="profile-avatar">
                    <i class="fas fa-user"></i>
                </div>
                <div class="profile-info">
                    <h2><asp:Label ID="lblFullName" runat="server"></asp:Label></h2>
                    <p><asp:Label ID="lblEmail" runat="server"></asp:Label></p>
                    <p><asp:Label ID="lblPhone" runat="server"></asp:Label></p>
                </div>
            </div>

            <div class="profile-tabs">
                <a href="Profile.aspx?tab=info" class='tab-button <%= GetActiveTabClass("info") %>'>
                    <i class="fas fa-user"></i> Informations personnelles
                </a>
                <a href="Profile.aspx?tab=orders" class='tab-button <%= GetActiveTabClass("orders") %>'>
                    <i class="fas fa-shopping-bag"></i> Mes commandes
                </a>
                <a href="Profile.aspx?tab=addresses" class='tab-button <%= GetActiveTabClass("addresses") %>'>
                    <i class="fas fa-map-marker-alt"></i> Adresses
                </a>
            </div>

            <!-- Tab: Informations personnelles -->
            <div id="tab-info" class='tab-content <%= GetActiveTabClass("info") == "active" ? "active" : "" %>'>
                <div class="card">
                    <h3 style="margin-bottom: 1.5rem;">Modifier mes informations</h3>
                    
                    <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
                        <asp:Literal ID="litSuccess" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                        <asp:Literal ID="litError" runat="server"></asp:Literal>
                    </asp:Panel>

                    <div class="form-group">
                        <label>Nom complet *</label>
                        <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" required></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>Email *</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" required></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>Téléphone</label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" TextMode="Phone"></asp:TextBox>
                    </div>

                    <h4 style="margin-top: 2rem; margin-bottom: 1rem;">Changer le mot de passe</h4>

                    <div class="form-group">
                        <label>Nouveau mot de passe</label>
                        <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                        <small style="color: var(--text-light);">Laisser vide pour ne pas changer</small>
                    </div>

                    <div class="form-group">
                        <label>Confirmer le nouveau mot de passe</label>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                    </div>

                    <asp:Button ID="btnSave" runat="server" Text="Enregistrer les modifications" 
                        CssClass="btn btn-primary" OnClick="btnSave_Click" />
                </div>
            </div>

            <!-- Tab: Mes commandes -->
            <div id="tab-orders" class='tab-content <%= GetActiveTabClass("orders") == "active" ? "active" : "" %>'>
                <div class="card">
                    <h3 style="margin-bottom: 1.5rem;">Historique des commandes</h3>
                    
                    <asp:Repeater ID="rptOrders" runat="server">
                        <HeaderTemplate>
                            <div style="display: flex; flex-direction: column; gap: 1rem;">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div style="padding: 1.5rem; background: var(--bg-light); border-radius: 8px; border: 1px solid var(--border-color);">
                                <div style="display: flex; justify-content: space-between; align-items: start; margin-bottom: 1rem;">
                                    <div>
                                        <h4 style="margin: 0 0 0.5rem 0;">
                                            Commande #<%# Eval("OrderNumber") %>
                                        </h4>
                                        <p style="margin: 0; color: var(--text-light); font-size: 0.9rem;">
                                            <i class="fas fa-calendar"></i> <%# GetOrderDate(Eval("OrderDate")) %>
                                        </p>
                                    </div>
                                    <div style="text-align: right;">
                                        <div style="font-size: 1.2rem; font-weight: 700; color: var(--primary-color); margin-bottom: 0.5rem;">
                                            <%# Eval("TotalAmount", "{0:F2}") %> MAD
                                        </div>
                                        <span class="badge badge-primary"><%# Eval("Status") %></span>
                                    </div>
                                </div>
                                <div style="display: flex; gap: 1rem;">
                                    <a href='OrderDetails.aspx?id=<%# Eval("Id") %>' class="btn btn-outline" style="padding: 8px 15px; text-decoration: none;">
                                        <i class="fas fa-eye"></i> Voir les détails
                                    </a>
                                    <%# GetTrackingButton(Eval("Status"), Eval("Id")) %>
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>

                    <asp:Label ID="lblNoOrders" runat="server" Text="Aucune commande pour le moment." 
                        Visible="false" style="text-align: center; padding: 3rem; color: var(--text-light); display: block;"></asp:Label>
                </div>
            </div>

            <!-- Tab: Adresses -->
            <div id="tab-addresses" class='tab-content <%= GetActiveTabClass("addresses") == "active" ? "active" : "" %>'>
                <div class="card">
                    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
                        <h3 style="margin: 0;">Mes adresses</h3>
                        <a href="Addresses.aspx" class="btn btn-primary">
                            <i class="fas fa-plus"></i> Ajouter une adresse
                        </a>
                    </div>
                    
                    <asp:Repeater ID="rptAddresses" runat="server">
                        <HeaderTemplate>
                            <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 1.5rem;">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div style="padding: 1.5rem; background: var(--bg-light); border-radius: 8px; border: 1px solid var(--border-color);">
                                <%# GetDefaultBadge(Eval("IsDefault")) %>
                                <h4 style="margin: 0 0 1rem 0;"><%# Eval("FullName") %></h4>
                                <p style="margin: 0 0 0.5rem 0; color: var(--text-dark);">
                                    <%# Eval("Street") %>
                                </p>
                                <p style="margin: 0 0 0.5rem 0; color: var(--text-dark);">
                                    <%# Eval("ZipCode") %> <%# Eval("City") %>
                                </p>
                                <p style="margin: 0 0 1rem 0; color: var(--text-dark);">
                                    <%# Eval("Country") %>
                                </p>
                                <div style="display: flex; gap: 0.5rem;">
                                    <a href='Addresses.aspx?id=<%# Eval("Id") %>' class="btn btn-outline" style="padding: 6px 12px; font-size: 0.9rem;">
                                        <i class="fas fa-edit"></i> Modifier
                                    </a>
                                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" 
                                        CommandArgument='<%# Eval("Id") %>' 
                                        CssClass="btn" 
                                        style="padding: 6px 12px; font-size: 0.9rem; background: rgba(220,53,69,0.1); color: var(--danger-color);"
                                        OnClientClick="return confirm('Êtes-vous sûr de vouloir supprimer cette adresse ?');">
                                        <i class="fas fa-trash"></i> Supprimer
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>

                    <asp:Label ID="lblNoAddresses" runat="server" Text="Aucune adresse enregistrée." 
                        Visible="false" style="text-align: center; padding: 3rem; color: var(--text-light); display: block;"></asp:Label>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

