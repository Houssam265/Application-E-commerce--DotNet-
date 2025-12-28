<%@ Page Title="Gestion des Réclamations" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="ComplaintsManagement.aspx.cs" Inherits="Ecommerce.Pages.Admin.ComplaintsManagement" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .complaint-card {
                background: white;
                border-radius: 8px;
                padding: 20px;
                margin-bottom: 15px;
                box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
            }

            .complaint-header {
                display: flex;
                justify-content: space-between;
                align-items: center;
                margin-bottom: 15px;
            }

            .complaint-title {
                font-size: 18px;
                font-weight: 600;
                color: #111827;
            }

            .priority-badge {
                padding: 4px 12px;
                border-radius: 12px;
                font-size: 12px;
                font-weight: 600;
            }

            .priority-high {
                background-color: #fee2e2;
                color: #991b1b;
            }

            .priority-medium {
                background-color: #fef3c7;
                color: #92400e;
            }

            .priority-low {
                background-color: #dbeafe;
                color: #1e40af;
            }

            .status-badge {
                padding: 6px 14px;
                border-radius: 16px;
                font-size: 13px;
                font-weight: 600;
            }

            .status-pending {
                background-color: #fef3c7;
                color: #92400e;
            }

            .status-inprogress {
                background-color: #dbeafe;
                color: #1e40af;
            }

            .status-resolved {
                background-color: #d1fae5;
                color: #065f46;
            }

            .status-closed {
                background-color: #e5e7eb;
                color: #374151;
            }

            .filter-section {
                background: white;
                padding: 20px;
                border-radius: 8px;
                margin-bottom: 20px;
            }

            .complaint-details {
                background: #f9fafb;
                padding: 15px;
                border-radius: 6px;
                margin: 10px 0;
            }

            .response-box {
                background: #f0fdf4;
                border: 1px solid #86efac;
                padding: 15px;
                border-radius: 6px;
                margin: 15px 0;
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="container-fluid py-4">
            <h2><i class="fas fa-exclamation-circle"></i> Gestion des Réclamations</h2>

            <div class="filter-section">
                <div class="row align-items-end">
                    <div class="col-md-3">
                        <label>Statut</label>
                        <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                            <asp:ListItem Value="" Text="Tous les statuts" />
                            <asp:ListItem Value="Pending" Text="En attente" />
                            <asp:ListItem Value="InProgress" Text="En cours" />
                            <asp:ListItem Value="Resolved" Text="Résolue" />
                            <asp:ListItem Value="Closed" Text="Fermée" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label>Priorité</label>
                        <asp:DropDownList ID="ddlPriorityFilter" runat="server" CssClass="form-control"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                            <asp:ListItem Value="" Text="Toutes les priorités" />
                            <asp:ListItem Value="High" Text="Haute" />
                            <asp:ListItem Value="Medium" Text="Moyenne" />
                            <asp:ListItem Value="Low" Text="Basse" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label>Recherche</label>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                            Placeholder="Rechercher par sujet ou utilisateur..." />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnSearch" runat="server" Text="Rechercher" CssClass="btn btn-primary w-100"
                            OnClick="btnSearch_Click" />
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnlList" runat="server">
                <asp:Repeater ID="rptComplaints" runat="server" OnItemCommand="rptComplaints_ItemCommand">
                    <ItemTemplate>
                        <div class="complaint-card">
                            <div class="complaint-header">
                                <div>
                                    <span class="complaint-title">
                                        <%# Eval("Subject") %>
                                    </span><br />
                                    <small class="text-muted">
                                        <i class="fas fa-user"></i>
                                        <%# Eval("FullName") %> (<%# Eval("Email") %>) • <i class="fas fa-calendar"></i>
                                                <%# System.Convert.ToDateTime(Eval("CreatedAt")).ToString("dd/MM/yyyy HH:mm") %>
                                                    <%# Eval("OrderNumber") != System.DBNull.Value ? " • Commande: " +
                                                        Eval("OrderNumber") : "" %>
                                    </small>
                                </div>
                                <div>
                                    <span class='priority-badge priority-<%# Eval("Priority").ToString().ToLower() %>'>
                                        <%# GetPriorityLabel(Eval("Priority").ToString()) %>
                                    </span>
                                    <span class='status-badge status-<%# Eval("Status").ToString().ToLower() %>'>
                                        <%# GetStatusLabel(Eval("Status").ToString()) %>
                                    </span>
                                </div>
                            </div>
                            <div class="complaint-details"><strong>Description:</strong><br />
                                <%# Eval("Description") %>
                            </div>
                            <asp:LinkButton ID="btnView" runat="server" CssClass="btn btn-sm btn-primary"
                                CommandName="ViewComplaint" CommandArgument='<%# Eval("Id") %>'>
                                <i class="fas fa-eye"></i> Voir détails et répondre
                            </asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlNoComplaints" runat="server" Visible="false" CssClass="text-center py-5">
                    <i class="fas fa-inbox fa-3x text-muted mb-3"></i>
                    <p class="text-muted">Aucune réclamation trouvée.</p>
                </asp:Panel>
            </asp:Panel>

            <asp:Panel ID="pnlDetails" runat="server" Visible="false">
                <div class="card">
                    <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                        <h5 class="mb-0"><i class="fas fa-file-alt"></i> Détails de la Réclamation</h5>
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-sm btn-light"
                            OnClick="btnBack_Click">
                            <i class="fas fa-arrow-left"></i> Retour
                        </asp:LinkButton>
                    </div>
                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-md-6">
                                <strong>Utilisateur:</strong>
                                <asp:Label ID="lblUserName" runat="server" /><br />
                                <strong>Email:</strong>
                                <asp:Label ID="lblUserEmail" runat="server" /><br />
                                <strong>Date:</strong>
                                <asp:Label ID="lblCreatedDate" runat="server" />
                            </div>
                            <div class="col-md-6 text-end">
                                <strong>Commande:</strong>
                                <asp:Label ID="lblOrderNumber" runat="server" Text="Aucune" /><br />
                                <strong>Priorité:</strong>
                                <asp:Label ID="lblPriority" runat="server" /><br />
                                <strong>Catégorie:</strong>
                                <asp:Label ID="lblCategory" runat="server" Text="Non spécifié" />
                            </div>
                        </div>

                        <hr />

                        <div class="mb-3">
                            <strong>Sujet:</strong>
                            <div class="mt-2">
                                <asp:Label ID="lblSubject" runat="server" CssClass="fs-5" />
                            </div>
                        </div>

                        <div class="mb-3">
                            <strong>Description:</strong>
                            <div class="complaint-details mt-2">
                                <asp:Label ID="lblDescription" runat="server" />
                            </div>
                        </div>

                        <asp:Panel ID="pnlExistingResponse" runat="server" Visible="false">
                            <div class="response-box">
                                <strong><i class="fas fa-check-circle text-success"></i> Réponse envoyée le
                                    <asp:Label ID="lblResponseDate" runat="server" />
                                </strong>
                                <div class="mt-2">
                                    <asp:Label ID="lblExistingResponse" runat="server" />
                                </div>
                            </div>
                        </asp:Panel>

                        <hr />

                        <h6><i class="fas fa-reply"></i> Répondre à la réclamation</h6>
                        <div class="mb-3">
                            <label>Statut de la réclamation</label>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Pending" Text="En attente" />
                                <asp:ListItem Value="InProgress" Text="En cours de traitement" />
                                <asp:ListItem Value="Resolved" Text="Résolue (sera automatiquement fermée)" />
                                <asp:ListItem Value="Closed" Text="Fermée" />
                            </asp:DropDownList>
                            <small class="text-muted">Note: Si vous sélectionnez "Résolue", la réclamation sera automatiquement fermée et ne pourra plus être modifiée.</small>
                        </div>
                        <div class="mb-3">
                            <label>Votre réponse</label>
                            <asp:TextBox ID="txtResponse" runat="server" TextMode="MultiLine" Rows="5"
                                CssClass="form-control" Placeholder="Rédigez votre réponse ici..."></asp:TextBox>
                        </div>
                        <div class="mb-3 form-check">
                            <asp:CheckBox ID="chkSendEmail" runat="server" CssClass="form-check-input" Checked="true" />
                            <label class="form-check-label">Envoyer un email de notification à l'utilisateur</label>
                        </div>
                        <asp:Button ID="btnSubmitResponse" runat="server" Text="Enregistrer et envoyer"
                            CssClass="btn btn-success" OnClick="btnSubmitResponse_Click" />

                        <asp:Panel ID="pnlResponseSuccess" runat="server" Visible="false"
                            CssClass="alert alert-success mt-3">
                            <i class="fas fa-check-circle"></i>
                            <asp:Literal ID="litSuccess" runat="server" />
                        </asp:Panel>
                        <asp:Panel ID="pnlResponseError" runat="server" Visible="false"
                            CssClass="alert alert-danger mt-3">
                            <i class="fas fa-exclamation-circle"></i>
                            <asp:Literal ID="litError" runat="server" />
                        </asp:Panel>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </asp:Content>