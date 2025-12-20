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
                box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
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

            .empty-state {
                padding: 4rem 2rem;
                text-align: center;
                color: #64748b;
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

        <div class="users-table-container">
            <asp:GridView ID="gvUsers" runat="server" CssClass="grid-view" AutoGenerateColumns="False" 
                GridLines="None" OnRowCommand="gvUsers_RowCommand" DataKeyNames="Id">
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