<%@ Page Title="Gestion Commandes" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="Orders.aspx.cs" Inherits="Ecommerce.Pages.Admin.Orders" %>

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

            .status-badge {
                padding: 0.25rem 0.5rem;
                border-radius: 4px;
                font-size: 0.85rem;
                font-weight: 600;
            }

            .status-Pending {
                background: #f59e0b;
                color: #fff;
            }

            .status-Shipped {
                background: #3b82f6;
                color: #fff;
            }

            .status-Delivered {
                background: #10b981;
                color: #fff;
            }

            .status-Cancelled {
                background: #ef4444;
                color: #fff;
            }

            .order-details {
                background: var(--secondary-bg);
                border: 1px solid var(--glass-border);
                padding: 1.5rem;
                border-radius: 12px;
                margin-top: 2rem;
            }
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <h1>Commandes</h1>

        <asp:Panel ID="pnlList" runat="server">
            <asp:GridView ID="gvOrders" runat="server" CssClass="grid-view" AutoGenerateColumns="False"
                OnRowCommand="gvOrders_RowCommand" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="#" />
                    <asp:BoundField DataField="FullName" HeaderText="Client" />
                    <asp:BoundField DataField="OrderDate" HeaderText="Date" DataFormatString="{0:d}" />
                    <asp:BoundField DataField="TotalAmount" HeaderText="Total" DataFormatString="{0:C}" />
                    <asp:TemplateField HeaderText="Statut">
                        <ItemTemplate>
                            <span class='status-badge status-<%# Eval("Status") %>'>
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnView" runat="server" CommandName="ViewOrder"
                                CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-primary"
                                style="padding: 0.25rem 0.75rem; font-size: 0.9rem;">Gérer</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div style="padding: 2rem; text-align: center; color: var(--text-muted);">
                        Aucune commande trouvée.
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </asp:Panel>

        <asp:Panel ID="pnlDetails" runat="server" Visible="false" CssClass="order-details">
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
                <h2>Commande #<asp:Label ID="lblOrderId" runat="server"></asp:Label>
                </h2>
                <asp:Button ID="btnClose" runat="server" Text="Fermer" OnClick="btnClose_Click" CssClass="btn"
                    style="background: rgba(255,255,255,0.1);" />
            </div>

            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 2rem; margin-bottom: 2rem;">
                <div>
                    <h3 style="margin-bottom: 0.5rem; color: var(--text-muted);">Client</h3>
                    <p>
                        <asp:Label ID="lblCustomer" runat="server"></asp:Label>
                    </p>
                    <p>
                        <asp:Label ID="lblEmail" runat="server"></asp:Label>
                    </p>
                </div>
                <div>
                    <h3 style="margin-bottom: 0.5rem; color: var(--text-muted);">Livraison</h3>
                    <p>
                        <asp:Label ID="lblAddress" runat="server"></asp:Label>
                    </p>
                </div>
            </div>

            <h3 style="margin-bottom: 1rem;">Articles</h3>
            <asp:GridView ID="gvItems" runat="server" CssClass="grid-view" AutoGenerateColumns="False" GridLines="None"
                style="margin-bottom: 2rem;">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Produit" />
                    <asp:BoundField DataField="Quantity" HeaderText="Qté" />
                    <asp:BoundField DataField="UnitPrice" HeaderText="Prix Unitaire" DataFormatString="{0:C}" />
                    <asp:TemplateField HeaderText="Total">
                        <ItemTemplate>
                            <%# GetItemTotal(Eval("UnitPrice"), Eval("Quantity")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div style="border-top: 1px solid var(--glass-border); padding-top: 1.5rem;">
                <h3 style="margin-bottom: 1rem;">Mise à jour du Statut</h3>
                <div style="display: flex; gap: 1rem; align-items: center;">
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" style="width: auto;">
                        <asp:ListItem Value="Pending">En attente</asp:ListItem>
                        <asp:ListItem Value="Processing">En préparation</asp:ListItem>
                        <asp:ListItem Value="Shipped">Expédié</asp:ListItem>
                        <asp:ListItem Value="Delivered">Livré</asp:ListItem>
                        <asp:ListItem Value="Cancelled">Annulé</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Button ID="btnUpdateStatus" runat="server" Text="Mettre à jour" CssClass="btn btn-primary"
                        OnClick="btnUpdateStatus_Click" />
                </div>
            </div>
        </asp:Panel>
    </asp:Content>