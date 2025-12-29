<%@ Page Title="Historique des Commandes" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="OrderHistory.aspx.cs" Inherits="Ecommerce.Pages.Admin.OrderHistory" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .order-card {
                background: white;
                border-radius: 12px;
                padding: 20px;
                margin-bottom: 15px;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            }

            .order-header {
                display: flex;
                justify-content: space-between;
                align-items: center;
                margin-bottom: 15px;
                border-bottom: 2px solid #f0f0f0;
                padding-bottom: 15px;
            }

            .status-badge {
                padding: 6px 14px;
                border-radius: 16px;
                font-size: 13px;
                font-weight: 600;
            }

            .status-delivered {
                background-color: #d1fae5;
                color: #065f46;
            }

            .status-cancelled {
                background-color: #fee2e2;
                color: #991b1b;
            }

            .filter-section {
                background: white;
                padding: 20px;
                border-radius: 12px;
                margin-bottom: 20px;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="container-fluid">
            <h1><i class="fas fa-archive"></i> Historique des Commandes</h1>
            <p>Commandes archivées (livrées ou annulées)</p>

            <div class="filter-section">
                <div class="row align-items-end">
                    <div class="col-md-3">
                        <label>Statut</label>
                        <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                            <asp:ListItem Value="" Text="Tous" />
                            <asp:ListItem Value="Delivered" Text="Livrées" />
                            <asp:ListItem Value="Cancelled" Text="Annulées" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label>Recherche (N° commande, nom client, email)</label>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                            Placeholder="Rechercher par numéro, nom ou email..." />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnSearch" runat="server" Text="Rechercher" CssClass="btn btn-primary w-100"
                            OnClick="btnSearch_Click" />
                    </div>
                    <div class="col-md-3 text-end">
                        <asp:Label ID="lblTotalArchived" runat="server" CssClass="badge bg-secondary fs-6" />
                    </div>
                </div>
            </div>

            <asp:Repeater ID="rptOrders" runat="server" OnItemCommand="rptOrders_ItemCommand" OnItemDataBound="rptOrders_ItemDataBound">
                <ItemTemplate>
                    <div class="order-card">
                        <div class="order-header">
                            <div>
                                <h5 style="margin: 0;">Commande #<%# Eval("OrderNumber") %>
                                </h5>
                                <small class="text-muted">
                                    <i class="fas fa-user"></i>
                                    <%# Eval("FullName") %> (<%# Eval("Email") %>)<br />
                                            <i class="fas fa-calendar"></i> Commandée le: <%#
                                                System.Convert.ToDateTime(Eval("OrderDate")).ToString("dd/MM/yyyy HH:mm") %>
                                                <br />
                                                <i class="fas fa-check-circle"></i> Archivée le: <%#
                                                    System.Convert.ToDateTime(Eval("CompletedDate")).ToString("dd/MM/yyyy HH:mm") %>
                                </small>
                            </div>
                            <div class="text-end">
                                <div class="fs-4 fw-bold text-success mb-2">
                                    <%# System.Convert.ToDecimal(Eval("TotalAmount")).ToString("F2") %> MAD
                                </div>
                                <span class='status-badge status-<%# Eval("Status").ToString().ToLower() %>'>
                                    <%# GetStatusLabel(Eval("Status").ToString()) %>
                                </span>
                            </div>
                        </div>
                        <%# !string.IsNullOrEmpty(Eval("Notes").ToString())
                            ? "<div class='alert alert-warning'><strong>Notes:</strong> " +
                            Server.HtmlEncode(Eval("Notes").ToString()) + "</div>" : "" %>
                        <asp:Panel ID="pnlReview" runat="server" Visible="false" CssClass="alert alert-info mt-2">
                            <strong><i class="fas fa-star"></i> Avis client:</strong>
                            <div id="reviewStars" runat="server"></div>
                            <div id="reviewComment" runat="server" class="mt-2"></div>
                            <small class="text-muted" id="reviewDate" runat="server"></small>
                        </asp:Panel>
                        <div class="mt-3">
                            <asp:LinkButton ID="btnDownloadInvoice" runat="server" 
                                CssClass="btn btn-primary btn-sm"
                                CommandName="DownloadInvoice" 
                                CommandArgument='<%# Eval("OrderId") %>'>
                                <i class="fas fa-file-pdf"></i> Télécharger la facture (PDF)
                            </asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoOrders" runat="server" Visible="false" CssClass="text-center py-5">
                <i class="fas fa-inbox fa-3x text-muted mb-3"></i>
                <p class="text-muted">Aucune commande archivée trouvée.</p>
            </asp:Panel>
        </div>
    </asp:Content>