<%@ Page Title="Utilisateurs" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="Users.aspx.cs" Inherits="Ecommerce.Pages.Admin.Users" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .page-header {
                margin-bottom: 2rem;
                padding-bottom: 1.5rem;
                border-bottom: 1px solid #e2e8f0;
            }

            .users-table-container {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 1px solid #e2e8f0;
                border-radius: 16px;
                padding: 1.5rem;
                overflow-x: auto;
                box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
                transition: all 0.3s ease;
            }

            .users-table-container:hover {
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

            .role-badge {
                padding: 0.375rem 0.75rem;
                border-radius: 6px;
                font-size: 0.875rem;
                font-weight: 600;
                display: inline-block;
            }

            .role-badge.Admin {
                background: rgba(139, 92, 246, 0.15);
                color: #7c3aed;
            }

            .role-badge.Customer {
                background: rgba(59, 130, 246, 0.15);
                color: #2563eb;
            }

            .status-badge {
                padding: 0.375rem 0.75rem;
                border-radius: 6px;
                font-size: 0.875rem;
                font-weight: 600;
                display: inline-block;
            }

            .status-badge.active {
                background: rgba(16, 185, 129, 0.15);
                color: #059669;
            }

            .status-badge.inactive {
                background: rgba(148, 163, 184, 0.15);
                color: #64748b;
            }

            .role-badge, .status-badge {
                transition: all 0.3s ease;
                cursor: default;
            }

            .role-badge:hover, .status-badge:hover {
                transform: scale(1.05);
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            }

            .empty-state i {
                animation: pulse 2s infinite;
            }

            @keyframes pulse {
                0%, 100% {
                    opacity: 0.5;
                    transform: scale(1);
                }
                50% {
                    opacity: 0.8;
                    transform: scale(1.1);
                }
            }

            .empty-state {
                padding: 4rem 2rem;
                text-align: center;
                color: #64748b;
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

            .pager {
                margin-top: 1rem;
                padding: 1rem;
                background: #f8fafc;
                border-radius: 8px;
                display: flex;
                justify-content: center;
                align-items: center;
                gap: 0.5rem;
            }

            .pager a, .pager span {
                padding: 0.5rem 1rem;
                border-radius: 6px;
                text-decoration: none;
                color: #475569;
                font-weight: 500;
                transition: all 0.2s ease;
            }

            .pager a:hover {
                background: #e2e8f0;
                color: #1e293b;
            }

            .pager .pager-current {
                background: #3b82f6;
                color: white;
            }
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <div>
                <h1><i class="fas fa-users"></i> Utilisateurs</h1>
                <p style="color: #64748b; margin-top: 0.5rem; margin-bottom: 0;">Gérez les utilisateurs du site</p>
            </div>
        </div>

        <div class="search-filter-container">
            <div class="filter-row">
                <div class="filter-group" style="flex: 2;">
                    <label><i class="fas fa-search"></i> Recherche</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Nom, email..." />
                </div>
                <div class="filter-group">
                    <label><i class="fas fa-user-tag"></i> Rôle</label>
                    <asp:DropDownList ID="ddlRoleFilter" runat="server" CssClass="search-input">
                        <asp:ListItem Value="" Text="Tous" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="Admin" Text="Admin"></asp:ListItem>
                        <asp:ListItem Value="Customer" Text="Client"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label><i class="fas fa-toggle-on"></i> Statut</label>
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="search-input">
                        <asp:ListItem Value="" Text="Tous" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Actif"></asp:ListItem>
                        <asp:ListItem Value="0" Text="Inactif"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="filter-row">
                <asp:LinkButton ID="btnSearch" runat="server" CssClass="search-btn" OnClick="btnSearch_Click">
                    <i class="fas fa-search"></i>
                    <span>Rechercher</span>
                </asp:LinkButton>
                <asp:LinkButton ID="btnClear" runat="server" CssClass="search-btn" OnClick="btnClear_Click" 
                    style="background: #64748b;">
                    <i class="fas fa-times"></i>
                    <span>Effacer les filtres</span>
                </asp:LinkButton>
            </div>
        </div>
        <div class="users-table-container">
            <asp:GridView ID="gvUsers" runat="server" CssClass="grid-view" AutoGenerateColumns="False" 
                GridLines="None" OnRowCommand="gvUsers_RowCommand" OnPageIndexChanging="gvUsers_PageIndexChanging"
                DataKeyNames="Id" AllowPaging="true" PageSize="10" PagerStyle-CssClass="pager">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="#" />
                    <asp:BoundField DataField="FullName" HeaderText="Nom" />
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:TemplateField HeaderText="Rôle">
                        <ItemTemplate>
                            <span class="role-badge <%# Eval("Role") %>">
                                <%# Eval("Role") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreatedAt" HeaderText="Inscrit le" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:TemplateField HeaderText="Statut">
                        <ItemTemplate>
                            <span class="status-badge <%# GetStatusClass(Eval("IsActive")) %>">
                                <%# GetStatusText(Eval("IsActive")) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleActive"
                                CommandArgument='<%# Eval("Id") %>' 
                                CssClass='<%# GetFullButtonClass(Eval("IsActive")) %>'
                                style="padding: 0.5rem 1rem; font-size: 0.9rem; text-decoration: none; display: inline-flex; align-items: center; gap: 0.5rem;"
                                OnClientClick='<%# GetConfirmMessage(Eval("IsActive")) %>'>
                                <i class="fas fa-<%# GetButtonIcon(Eval("IsActive")) %>"></i>
                                <span><%# GetToggleText(Eval("IsActive")) %></span>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="empty-state">
                        <i class="fas fa-users"></i>
                        <h3>Aucun utilisateur trouvé</h3>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>

    </asp:Content>