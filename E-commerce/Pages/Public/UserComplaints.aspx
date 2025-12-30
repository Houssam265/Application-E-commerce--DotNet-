<%@ Page Title="Mes Réclamations" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="UserComplaints.aspx.cs" Inherits="Ecommerce.Pages.Public.UserComplaints" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        <style>
            .complaints-container {
                max-width: 1000px;
                margin: 40px auto;
            }

            .complaint-card {
                background: white;
                border: 1px solid #e5e7eb;
                border-radius: 8px;
                padding: 20px;
                margin-bottom: 20px;
            }

            .status-badge {
                padding: 6px 14px;
                border-radius: 16px;
                font-size: 13px;
                font-weight: 600;
                display: inline-block;
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

            .response-section {
                background: #f0fdf4;
                border-left: 4px solid #22c55e;
                padding: 15px;
                margin-top: 15px;
                border-radius: 4px;
            }

            .priority-badge {
                padding: 4px 10px;
                border-radius: 12px;
                font-size: 11px;
                font-weight: 600;
                display: inline-block;
                margin-top: 5px;
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
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="complaints-container">
            <div class="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h2><i class="fas fa-exclamation-triangle"></i> Mes Réclamations</h2>
                    <p class="text-muted mb-0">Gérez vos réclamations et suivez leur traitement</p>
                </div>
                <asp:Button ID="btnNewComplaint" runat="server" Text="✚ Nouvelle réclamation" CssClass="btn btn-primary"
                    OnClick="btnNewComplaint_Click" />
            </div>

            <!-- New Complaint Form -->
            <asp:Panel ID="pnlNewComplaint" runat="server" Visible="false" CssClass="card mb-4">
                <div class="card-header bg-primary text-white">
                    <h5 class="mb-0">Nouvelle Réclamation</h5>
                </div>
                <div class="card-body">
                    <div class="mb-3">
                        <label>Commande concernée (optionnel)</label>
                        <asp:DropDownList ID="ddlOrder" runat="server" CssClass="form-control">
                            <asp:ListItem Value="" Text="-- Aucune commande spécifique --" />
                        </asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label>Catégorie</label>
                        <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control">
                            <asp:ListItem Value="Qualité Produit" Text="Qualité du produit" />
                            <asp:ListItem Value="Livraison" Text="Problème de livraison" />
                            <asp:ListItem Value="Paiement" Text="Problème de paiement" />
                            <asp:ListItem Value="Service Client" Text="Service client" />
                            <asp:ListItem Value="Autre" Text="Autre" Selected="True" />
                        </asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label>Sujet <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" MaxLength="200"
                            Placeholder="Décrivez brièvement votre réclamation (ex: Produit défectueux, Livraison retardée...)" />
                        <small class="text-muted">Un sujet clair nous aide à traiter votre demande plus rapidement</small>
                    </div>
                    <div class="mb-3">
                        <label>Description détaillée <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="6"
                            CssClass="form-control" Placeholder="Expliquez votre réclamation en détails...&#10;&#10;Exemple:&#10;- Date du problème&#10;- Numéro de commande (si applicable)&#10;- Description précise du problème&#10;- Ce que vous attendez comme solution" />
                        <small class="text-muted">Plus de détails = résolution plus rapide</small>
                    </div>
                    <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                        <asp:Literal ID="litError" runat="server" />
                    </asp:Panel>
                    <asp:Button ID="btnSubmit" runat="server" Text="Soumettre la réclamation" CssClass="btn btn-success"
                        OnClick="btnSubmit_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Annuler" CssClass="btn btn-secondary ms-2"
                        OnClick="btnCancel_Click" />
                </div>
            </asp:Panel>

            <!-- Complaints List -->
            <asp:Repeater ID="rptComplaints" runat="server">
                <ItemTemplate>
                    <div class="complaint-card">
                        <div class="d-flex justify-content-between align-items-start mb-2">
                            <div>
                                <h5>
                                    <i class="fas fa-tag" style="color: #6b7280;"></i>
                                    <%# Eval("Subject") %>
                                </h5>
                                <small class="text-muted">
                                    <i class="fas fa-calendar"></i>
                                    <%# System.Convert.ToDateTime(Eval("CreatedAt")).ToString("dd/MM/yyyy à HH:mm") %>
                                        <%# Eval("OrderNumber") != System.DBNull.Value ? " • Commande: " + Eval("OrderNumber")
                                            : "" %>
                                        <%# Eval("Category") != System.DBNull.Value ? " • Catégorie: " + Eval("Category")
                                            : "" %>
                                </small>
                            </div>
                            <div class="text-end">
                                <span class='status-badge status-<%# Eval("Status").ToString().ToLower() %>'>
                                    <%# GetStatusLabel(Eval("Status").ToString()) %>
                                </span>
                                <br />
                                <small class="text-muted">
                                    <span class='priority-badge priority-<%# Eval("Priority").ToString().ToLower() %>'>
                                        <%# GetPriorityLabel(Eval("Priority").ToString()) %>
                                    </span>
                                </small>
                            </div>
                        </div>
                        <div class="mb-2">
                            <strong>Description:</strong><br />
                            <%# Eval("Description") %>
                        </div>
                        <%# Eval("AdminResponse") != System.DBNull.Value &&
                            !string.IsNullOrWhiteSpace(Eval("AdminResponse").ToString())
                            ? "<div class='response-section'><strong><i class='fas fa-reply'></i> Réponse de l'équipe:</strong><br/>"
                            + Server.HtmlEncode(Eval("AdminResponse").ToString()).Replace("\n", "<br/>" ) + "</div>"
                            : "" %>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoComplaints" runat="server" Visible="false" CssClass="text-center py-5">
                <i class="fas fa-inbox fa-3x text-muted mb-3"></i>
                <p class="text-muted">Vous n'avez aucune réclamation.</p>
            </asp:Panel>
        </div>
    </asp:Content>