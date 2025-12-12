<%@ Page Title="Utilisateurs" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="Users.aspx.cs" Inherits="Ecommerce.Pages.Admin.Users" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .grid-view {
                width: 100%;
                border-collapse: collapse;
                margin-top: 1rem;
            }

            .grid-view th,
            .grid-view td {
                text-align: left;
                padding: 1rem;
                border-bottom: 1px solid var(--glass-border);
            }

            .grid-view th {
                background: rgba(255, 255, 255, 0.05);
                color: var(--text-muted);
            }
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <h1>Utilisateurs</h1>

        <asp:GridView ID="gvUsers" runat="server" CssClass="grid-view" AutoGenerateColumns="False" GridLines="None">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="#" />
                <asp:BoundField DataField="FullName" HeaderText="Nom" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="Role" HeaderText="Rôle" />
                <asp:BoundField DataField="CreatedAt" HeaderText="Inscrit le" DataFormatString="{0:d}" />
                <asp:TemplateField HeaderText="Actif">
                    <ItemTemplate>
                        <%# Convert.ToBoolean(Eval("IsActive")) ? "<span style='color:#10b981'>Oui</span>"
                            : "<span style='color:#ef4444'>Non</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div style="padding: 2rem; text-align: center; color: var(--text-muted);">
                    Aucun utilisateur trouvé.
                </div>
            </EmptyDataTemplate>
        </asp:GridView>

    </asp:Content>