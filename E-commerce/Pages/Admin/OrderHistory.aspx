<%@ Page Title="Historique des Commandes" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="OrderHistory.aspx.cs" Inherits="Ecommerce.Pages.Admin.OrderHistory" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .page-header {
                display: flex;
                justify-content: space-between;
                align-items: center;
                margin-bottom: 2rem;
                padding-bottom: 1.5rem;
                border-bottom: 1px solid #e2e8f0;
            }

            .page-header h1 {
                margin: 0;
                color: #1e293b;
            }

            .page-header p {
                color: #64748b;
                margin-top: 0.5rem;
                margin-bottom: 0;
            }

            .categories-table-container {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 1px solid #e2e8f0;
                border-radius: 16px;
                padding: 1.5rem;
                overflow-x: auto;
                box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
                transition: all 0.3s ease;
            }

            .categories-table-container:hover {
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

            .form-control {
                background: #ffffff;
                color: #1e293b;
                border: 1px solid #e2e8f0;
                padding: 0.75rem 1rem;
                width: 100%;
                border-radius: 10px;
                font-size: 0.95rem;
                transition: all 0.3s ease;
            }

            .form-control:focus {
                outline: none;
                border-color: #3b82f6;
                background: #ffffff;
                box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
            }

            .btn-primary {
                background: linear-gradient(135deg, var(--primary-color) 0%, var(--primary-light) 100%);
                border: none;
                border-radius: 10px;
                padding: 0.75rem 1.5rem;
                font-weight: 600;
                transition: all 0.3s ease;
                box-shadow: 0 4px 12px rgba(34, 197, 94, 0.3);
            }

            .btn-primary:hover {
                transform: translateY(-2px);
                box-shadow: 0 6px 20px rgba(34, 197, 94, 0.4);
            }

            .order-card {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 1px solid #e2e8f0;
                border-radius: 16px;
                padding: 1.5rem;
                margin-bottom: 1.5rem;
                box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
                transition: all 0.3s ease;
            }

            .order-card:hover {
                box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
                transform: translateY(-2px);
            }

            .status-badge {
                padding: 0.375rem 0.75rem;
                border-radius: 6px;
                font-size: 0.875rem;
                font-weight: 600;
                display: inline-block;
            }

            .status-delivered {
                background: rgba(16, 185, 129, 0.15);
                color: #059669;
            }

            .status-cancelled {
                background: rgba(239, 68, 68, 0.15);
                color: #dc2626;
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

            .action-btn {
                padding: 0.5rem 1rem;
                border-radius: 8px;
                font-weight: 600;
                font-size: 0.875rem;
                border: none;
                cursor: pointer;
                transition: all 0.3s ease;
                text-decoration: none;
                display: inline-flex;
                align-items: center;
                gap: 0.5rem;
            }

            .action-btn.primary {
                background: rgba(59, 130, 246, 0.15);
                color: #2563eb;
                border: 1px solid rgba(59, 130, 246, 0.25);
            }

            .action-btn.primary:hover {
                background: rgba(59, 130, 246, 0.25);
                transform: translateY(-2px);
                box-shadow: 0 4px 8px rgba(59, 130, 246, 0.2);
            }

            .search-filter-container {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 1px solid #e2e8f0;
                border-radius: 12px;
                padding: 1.5rem;
                margin-bottom: 1.5rem;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
            }

            .filter-row {
                display: flex;
                gap: 1rem;
                align-items: center;
                flex-wrap: wrap;
                margin-bottom: 1rem;
            }

            .filter-row:last-child {
                margin-bottom: 0;
            }

            .filter-group {
                display: flex;
                flex-direction: column;
                gap: 0.5rem;
                min-width: 150px;
            }

            .filter-group label {
                font-size: 0.85rem;
                color: #64748b;
                font-weight: 500;
            }

            .search-filter-container .search-input {
                flex: 1;
                padding: 0.75rem 1rem;
                border: 1px solid #e2e8f0;
                border-radius: 8px;
                font-size: 0.95rem;
                transition: all 0.3s ease;
            }

            .search-filter-container .search-input:focus {
                outline: none;
                border-color: #3b82f6;
                box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
            }

            .search-filter-container .search-btn {
                padding: 0.75rem 1.5rem;
                background: #3b82f6;
                color: white;
                border: none;
                border-radius: 8px;
                cursor: pointer;
                font-weight: 600;
                transition: all 0.3s ease;
                display: flex;
                align-items: center;
                gap: 0.5rem;
            }

            .search-filter-container .search-btn:hover {
                background: #2563eb;
                transform: translateY(-2px);
                box-shadow: 0 4px 8px rgba(59, 130, 246, 0.3);
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <div>
                <h1><i class="fas fa-archive"></i> Historique des Commandes</h1>
                <p>Archives et suivi des commandes clôturées</p>
            </div>
        </div>

        <div class="search-filter-container">
            <div class="filter-row">
                <div class="filter-group">
                    <label><i class="fas fa-info-circle"></i> Statut</label>
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="search-input"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="Tous" />
                        <asp:ListItem Value="Delivered" Text="Livrées" />
                        <asp:ListItem Value="Cancelled" Text="Annulées" />
                    </asp:DropDownList>
                </div>
                <div class="filter-group" style="flex: 2;">
                    <label><i class="fas fa-search"></i> Recherche</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input"
                        Placeholder="N° commande, nom client, email..." />
                </div>
                </div>
            <div class="filter-row">
                <asp:LinkButton ID="btnSearch" runat="server" CssClass="search-btn" OnClick="btnSearch_Click">
                    <i class="fas fa-search"></i>
                    <span>Rechercher</span>
                </asp:LinkButton>
                <div style="flex: 1; display: flex; justify-content: flex-end; align-items: center;">
                    <asp:Label ID="lblTotalArchived" runat="server" style="display: inline-block; padding: 0.5rem 1rem; background: rgba(59, 130, 246, 0.1); color: #2563eb; border-radius: 8px; font-weight: 600; font-size: 0.9rem;" />
                </div>
            </div>
        </div>

        <div class="categories-table-container">

            <asp:Repeater ID="rptOrders" runat="server" OnItemCommand="rptOrders_ItemCommand"
                OnItemDataBound="rptOrders_ItemDataBound">
                <HeaderTemplate>
                    <table class="grid-view">
                        <thead>
                            <tr>
                                <th>N° Commande</th>
                                <th>Client</th>
                                <th>Date</th>
                                <th>Total</th>
                                <th>Statut</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("OrderNumber") %></td>
                        <td><%# Eval("FullName") %></td>
                        <td><%# System.Convert.ToDateTime(Eval("OrderDate")).ToString("dd/MM/yyyy") %></td>
                        <td><%# System.Convert.ToDecimal(Eval("TotalAmount")).ToString("F2") %> MAD</td>
                        <td>
                            <span class='status-badge status-<%# Eval("Status").ToString().ToLower() %>'>
                                <%# GetStatusLabel(Eval("Status").ToString()) %>
                            </span>
                        </td>
                        <td>
                            <asp:LinkButton ID="btnDownloadInvoice" runat="server" CssClass="action-btn primary"
                                CommandName="DownloadInvoice" CommandArgument='<%# Eval("OrderId") %>'>
                                <i class="fas fa-file-pdf"></i>
                                <span>Facture</span>
                            </asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoOrders" runat="server" Visible="false" CssClass="empty-state">
                <i class="fas fa-archive"></i>
                <h3>Aucune commande archivée trouvée</h3>
                <p>Les commandes archivées apparaîtront ici</p>
            </asp:Panel>
        </div>
    </asp:Content>