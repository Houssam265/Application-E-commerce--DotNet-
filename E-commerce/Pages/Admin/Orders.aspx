<%@ Page Title="Gestion Commandes" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="Orders.aspx.cs" Inherits="Ecommerce.Pages.Admin.Orders" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .page-header {
                margin-bottom: 2rem;
                padding-bottom: 1.5rem;
                border-bottom: 1px solid #e2e8f0;
            }

            .orders-table-container {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 1px solid #e2e8f0;
                border-radius: 16px;
                padding: 1.5rem;
                overflow-x: auto;
                box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
                transition: all 0.3s ease;
            }

            .orders-table-container:hover {
                box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
            }

            .grid-view {
                width: 100%;
                border-collapse: collapse;
                margin: 0;
            }

            .grid-view th,
            .grid-view td {
                text-align: left;
                padding: 1.25rem 1rem;
                border-bottom: 1px solid #e2e8f0;
            }

            .grid-view th {
                background: rgba(59, 130, 246, 0.08);
                color: #475569;
                font-weight: 600;
                text-transform: uppercase;
                font-size: 0.85rem;
                letter-spacing: 0.5px;
            }

            .grid-view tbody tr {
                transition: all 0.2s ease;
            }

            .grid-view tbody tr:hover {
                background: rgba(59, 130, 246, 0.05);
                transform: scale(1.01);
            }

            .grid-view td {
                color: #1e293b;
            }

            .status-badge {
                padding: 0.5rem 1rem;
                border-radius: 8px;
                font-size: 0.85rem;
                font-weight: 600;
                display: inline-flex;
                align-items: center;
                gap: 0.5rem;
                transition: all 0.3s ease;
                cursor: default;
            }

            .status-badge:hover {
                transform: scale(1.05);
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            }

            .status-Pending {
                background: rgba(245, 158, 11, 0.15);
                color: #d97706;
                border: 1px solid rgba(245, 158, 11, 0.25);
            }

            .status-Processing {
                background: rgba(139, 92, 246, 0.15);
                color: #7c3aed;
                border: 1px solid rgba(139, 92, 246, 0.25);
            }

            .status-Shipped {
                background: rgba(59, 130, 246, 0.15);
                color: #2563eb;
                border: 1px solid rgba(59, 130, 246, 0.25);
            }

            .status-Delivered {
                background: rgba(16, 185, 129, 0.15);
                color: #059669;
                border: 1px solid rgba(16, 185, 129, 0.25);
            }

            .status-Cancelled {
                background: rgba(239, 68, 68, 0.15);
                color: #dc2626;
                border: 1px solid rgba(239, 68, 68, 0.25);
            }

            .order-details {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 1px solid #e2e8f0;
                padding: 2rem;
                border-radius: 16px;
                margin-top: 2rem;
            }

            .order-details h2 {
                color: #1e293b;
                font-family: 'Outfit', sans-serif;
                margin-bottom: 0;
            }

            .order-info-grid {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 2rem;
                margin-bottom: 2rem;
            }

            .info-section h3 {
                color: #64748b;
                font-size: 0.9rem;
                text-transform: uppercase;
                letter-spacing: 0.5px;
                margin-bottom: 0.75rem;
            }

            .info-section p {
                color: #1e293b;
                margin: 0.5rem 0;
            }

            .status-update-section {
                border-top: 1px solid #e2e8f0;
                padding-top: 1.5rem;
                margin-top: 2rem;
            }

            .status-update-section h3 {
                color: #1e293b;
                margin-bottom: 1rem;
            }

            .status-controls {
                display: flex;
                gap: 1rem;
                align-items: center;
            }

            .empty-state {
                padding: 4rem 2rem;
                text-align: center;
                color: #64748b;
            }

            .empty-state i {
                font-size: 3rem;
                margin-bottom: 1rem;
                opacity: 0.5;
            }
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <h1><i class="fas fa-clipboard-list"></i> Commandes</h1>
            <p style="color: #64748b; margin-top: 0.5rem; margin-bottom: 0;">Gérez et suivez toutes les commandes</p>
        </div>

        <asp:Panel ID="pnlList" runat="server">
            <div class="orders-table-container">
                <asp:GridView ID="gvOrders" runat="server" CssClass="grid-view" AutoGenerateColumns="False"
                    OnRowCommand="gvOrders_RowCommand" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="#" />
                        <asp:BoundField DataField="FullName" HeaderText="Client" />
                        <asp:BoundField DataField="OrderDate" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="TotalAmount" HeaderText="Total" DataFormatString="{0:F2} MAD" />
                        <asp:TemplateField HeaderText="Statut">
                            <ItemTemplate>
                                <span class='status-badge status-<%# Eval("Status") %>'>
                                    <i class="fas fa-circle" style="font-size: 0.5rem;"></i>
                                    <%# Eval("Status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnView" runat="server" CommandName="ViewOrder"
                                    CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-primary"
                                    style="padding: 0.5rem 1rem; font-size: 0.9rem;">
                                    <i class="fas fa-eye"></i>
                                    <span>Gérer</span>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-state">
                            <i class="fas fa-shopping-bag"></i>
                            <h3>Aucune commande trouvée</h3>
                            <p>Les commandes apparaîtront ici</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlDetails" runat="server" Visible="false" CssClass="order-details">
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem;">
                <h2>Commande #<asp:Label ID="lblOrderId" runat="server"></asp:Label></h2>
                <asp:LinkButton ID="btnClose" runat="server" OnClick="btnClose_Click" CssClass="btn secondary"
                    style="background: #f1f5f9; color: #64748b; border: 1px solid #e2e8f0;">
                    <i class="fas fa-times"></i>
                    <span>Fermer</span>
                </asp:LinkButton>
            </div>

            <div class="order-info-grid">
                <div class="info-section">
                    <h3><i class="fas fa-user"></i> Client</h3>
                    <p><strong><asp:Label ID="lblCustomer" runat="server"></asp:Label></strong></p>
                    <p><asp:Label ID="lblEmail" runat="server"></asp:Label></p>
                </div>
                <div class="info-section">
                    <h3><i class="fas fa-truck"></i> Livraison</h3>
                    <p><asp:Label ID="lblAddress" runat="server"></asp:Label></p>
                </div>
            </div>

            <div style="margin-bottom: 2rem;">
                <h3 style="color: #1e293b; margin-bottom: 1rem;">
                    <i class="fas fa-shopping-cart"></i> Articles commandés
                </h3>
                <div class="orders-table-container">
                    <asp:GridView ID="gvItems" runat="server" CssClass="grid-view" AutoGenerateColumns="False" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="Name" HeaderText="Produit" />
                            <asp:BoundField DataField="Quantity" HeaderText="Quantité" />
                            <asp:BoundField DataField="UnitPrice" HeaderText="Prix Unitaire" DataFormatString="{0:F2} MAD" />
                            <asp:TemplateField HeaderText="Total">
                                <ItemTemplate>
                                    <%# GetItemTotal(Eval("UnitPrice"), Eval("Quantity")) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="status-update-section">
                <h3><i class="fas fa-sync-alt"></i> Mise à jour du Statut</h3>
                <div class="status-controls">
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"
                        style="background: #ffffff; color: #1e293b; border: 1px solid #e2e8f0; padding: 0.75rem 1rem; border-radius: 10px; min-width: 200px;">
                        <asp:ListItem Value="Pending">En attente</asp:ListItem>
                        <asp:ListItem Value="Processing">En préparation</asp:ListItem>
                        <asp:ListItem Value="Shipped">Expédié</asp:ListItem>
                        <asp:ListItem Value="Delivered">Livré</asp:ListItem>
                        <asp:ListItem Value="Cancelled">Annulé</asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="txtCancelReason" runat="server" CssClass="form-control" placeholder="Raison de l'annulation" 
                        style="background: #ffffff; color: #1e293b; border: 1px solid #e2e8f0; padding: 0.75rem 1rem; border-radius: 10px; min-width: 300px; display: none;"></asp:TextBox>
                    <asp:LinkButton ID="btnUpdateStatus" runat="server" CssClass="btn btn-primary"
                        OnClick="btnUpdateStatus_Click">
                        <i class="fas fa-check"></i>
                        <span>Mettre à jour</span>
                    </asp:LinkButton>
                </div>
                <asp:Label ID="lblStatusError" runat="server" Visible="false" 
                    style="display:block; margin-top:0.75rem; color:#dc2626;"></asp:Label>
            </div>

            <div style="margin-top:2rem;">
                <h3><i class="fas fa-star"></i> Avis sur le service</h3>
                <asp:Panel ID="pnlReviewAdmin" runat="server" Visible="false" CssClass="orders-table-container">
                    <div style="display:flex; align-items:center; gap:0.75rem;">
                        <asp:Literal ID="litAdminReviewStars" runat="server"></asp:Literal>
                        <span id="lblAdminReviewDate" runat="server"></span>
                    </div>
                    <p style="margin-top:0.75rem;"><asp:Literal ID="litAdminReviewText" runat="server"></asp:Literal></p>
                </asp:Panel>
                <asp:Label ID="lblNoReview" runat="server" Visible="false" Text="Aucun avis pour cette commande." style="color:#64748b;"></asp:Label>
            </div>
        </asp:Panel>
    </asp:Content>
