<%@ Page Title="Gestion des Réclamations" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="ComplaintsManagement.aspx.cs" Inherits="Ecommerce.Pages.Admin.ComplaintsManagement" %>

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

            .status-badge {
                padding: 0.375rem 0.75rem;
                border-radius: 6px;
                font-size: 0.875rem;
                font-weight: 600;
                display: inline-block;
            }

            .status-pending {
                background: rgba(245, 158, 11, 0.15);
                color: #d97706;
            }

            .status-inprogress {
                background: rgba(59, 130, 246, 0.15);
                color: #2563eb;
            }

            .status-resolved {
                background: rgba(16, 185, 129, 0.15);
                color: #059669;
            }

            .status-closed {
                background: rgba(148, 163, 184, 0.15);
                color: #64748b;
            }

            .priority-badge {
                font-size: 0.75rem;
                text-transform: uppercase;
                letter-spacing: 0.05em;
                padding: 0.25rem 0.5rem;
                border-radius: 4px;
                font-weight: 600;
            }

            .priority-high {
                color: #dc2626;
                background: rgba(239, 68, 68, 0.15);
            }

            .priority-medium {
                color: #b45309;
                background: rgba(245, 158, 11, 0.15);
            }

            .priority-low {
                color: #15803d;
                background: rgba(16, 185, 129, 0.15);
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

            .action-buttons {
                display: flex;
                gap: 0.75rem;
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

            .action-btn.edit {
                background: rgba(59, 130, 246, 0.15);
                color: #2563eb;
                border: 1px solid rgba(59, 130, 246, 0.25);
            }

            .action-btn.edit:hover {
                background: rgba(59, 130, 246, 0.25);
                transform: translateY(-2px);
                box-shadow: 0 4px 8px rgba(59, 130, 246, 0.2);
            }

            .form-container {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 1px solid #e2e8f0;
                border-radius: 16px;
                padding: 2rem;
                max-width: 700px;
                margin-top: 2rem;
                box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
                transition: all 0.3s ease;
                animation: fadeInUp 0.5s ease-out;
            }

            @keyframes fadeInUp {
                from {
                    opacity: 0;
                    transform: translateY(20px);
                }
                to {
                    opacity: 1;
                    transform: translateY(0);
                }
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

            /* Enhanced Status Dropdown Styling */
            .status-dropdown-container {
                position: relative;
                margin-bottom: 1.5rem;
            }

            .status-dropdown-container label {
                display: block;
                font-weight: 600;
                color: #1e293b;
                margin-bottom: 0.75rem;
                font-size: 0.95rem;
                display: flex;
                align-items: center;
                gap: 0.5rem;
            }

            .status-dropdown-container label i {
                color: #3b82f6;
                font-size: 1rem;
            }

            #ddlStatus {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 2px solid #e2e8f0;
                border-radius: 12px;
                padding: 0.875rem 1.25rem;
                padding-right: 3rem;
                font-size: 1rem;
                font-weight: 500;
                color: #1e293b;
                width: 100%;
                cursor: pointer;
                transition: all 0.3s ease;
                appearance: none;
                background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 16 16'%3E%3Cpath fill='%233b82f6' d='M8 11L3 6h10z'/%3E%3C/svg%3E");
                background-repeat: no-repeat;
                background-position: right 1rem center;
                background-size: 1rem;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
            }

            #ddlStatus:hover {
                border-color: #3b82f6;
                background: linear-gradient(135deg, #ffffff 0%, #f1f5f9 100%);
                box-shadow: 0 4px 12px rgba(59, 130, 246, 0.15);
                transform: translateY(-1px);
            }

            #ddlStatus:focus {
                outline: none;
                border-color: #3b82f6;
                box-shadow: 0 0 0 4px rgba(59, 130, 246, 0.1), 0 4px 12px rgba(59, 130, 246, 0.2);
                background: #ffffff;
            }

            #ddlStatus:disabled {
                background: #f1f5f9;
                border-color: #cbd5e1;
                color: #94a3b8;
                cursor: not-allowed;
                opacity: 0.7;
            }

            /* Status-specific styling for options */
            #ddlStatus option {
                padding: 0.75rem 1rem;
                font-weight: 500;
            }

            #ddlStatus option[value="Pending"] {
                background-color: #fef3c7;
                color: #d97706;
            }

            #ddlStatus option[value="InProgress"] {
                background-color: #dbeafe;
                color: #2563eb;
            }

            #ddlStatus option[value="Resolved"] {
                background-color: #d1fae5;
                color: #059669;
            }

            #ddlStatus option[value="Closed"] {
                background-color: #f1f5f9;
                color: #64748b;
            }

            .status-dropdown-help {
                margin-top: 0.5rem;
                font-size: 0.875rem;
                color: #64748b;
                padding: 0.75rem 1rem;
                background: rgba(59, 130, 246, 0.05);
                border-left: 3px solid #3b82f6;
                border-radius: 6px;
                line-height: 1.6;
            }

            .status-dropdown-help i {
                color: #3b82f6;
                margin-right: 0.5rem;
            }

            /* Status indicator badge next to dropdown */
            .status-indicator {
                display: inline-flex;
                align-items: center;
                gap: 0.5rem;
                margin-top: 0.5rem;
                padding: 0.5rem 1rem;
                border-radius: 8px;
                font-size: 0.875rem;
                font-weight: 600;
            }

            .status-indicator.pending {
                background: rgba(245, 158, 11, 0.15);
                color: #d97706;
            }

            .status-indicator.inprogress {
                background: rgba(59, 130, 246, 0.15);
                color: #2563eb;
            }

            .status-indicator.resolved {
                background: rgba(16, 185, 129, 0.15);
                color: #059669;
            }

            .status-indicator.closed {
                background: rgba(148, 163, 184, 0.15);
                color: #64748b;
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <div>
                <h1><i class="fas fa-exclamation-circle"></i> Gestion des Réclamations</h1>
                <p>Suivi et traitement des tickets support</p>
            </div>
        </div>

        <div class="search-filter-container">
            <div class="filter-row">
                <div class="filter-group">
                    <label><i class="fas fa-info-circle"></i> Statut</label>
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="search-input"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="Tous les statuts" />
                        <asp:ListItem Value="Pending" Text="En attente" />
                        <asp:ListItem Value="InProgress" Text="En cours" />
                        <asp:ListItem Value="Resolved" Text="Résolue" />
                        <asp:ListItem Value="Closed" Text="Fermée" />
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label><i class="fas fa-exclamation-triangle"></i> Priorité</label>
                    <asp:DropDownList ID="ddlPriorityFilter" runat="server" CssClass="search-input"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlPriorityFilter_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="Toutes les priorités" />
                        <asp:ListItem Value="High" Text="Haute" />
                        <asp:ListItem Value="Medium" Text="Moyenne" />
                        <asp:ListItem Value="Low" Text="Basse" />
                    </asp:DropDownList>
                </div>
                <div class="filter-group" style="flex: 2;">
                    <label><i class="fas fa-search"></i> Recherche</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input"
                        Placeholder="Sujet, nom client, email, N° commande..." />
                </div>
                </div>
            <div class="filter-row">
                <asp:LinkButton ID="btnSearch" runat="server" CssClass="search-btn" OnClick="btnSearch_Click">
                    <i class="fas fa-search"></i>
                    <span>Rechercher</span>
                </asp:LinkButton>
            </div>
        </div>

        <div class="categories-table-container">

            <asp:Panel ID="pnlList" runat="server">
                <asp:Repeater ID="rptComplaints" runat="server" OnItemCommand="rptComplaints_ItemCommand">
                    <HeaderTemplate>
                        <table class="grid-view">
                            <thead>
                                <tr>
                                    <th>Sujet</th>
                                    <th>Client</th>
                                    <th>Priorité</th>
                                    <th>Statut</th>
                                    <th>Date</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Subject") %></td>
                            <td><%# Eval("FullName") %> (<%# Eval("Email") %>)</td>
                            <td>
                                <span class='priority-badge priority-<%# Eval("Priority").ToString().ToLower() %>'>
                                    <%# GetPriorityLabel(Eval("Priority").ToString()) %>
                                </span>
                            </td>
                            <td>
                                <span class='status-badge status-<%# Eval("Status").ToString().ToLower() %>'>
                                    <%# GetStatusLabel(Eval("Status").ToString()) %>
                                </span>
                            </td>
                            <td><%# System.Convert.ToDateTime(Eval("CreatedAt")).ToString("dd/MM/yyyy HH:mm") %></td>
                            <td>
                                <asp:LinkButton ID="btnView" runat="server" CommandName="ViewComplaint"
                                    CommandArgument='<%# Eval("Id") %>' CssClass="action-btn edit">
                                    <i class="fas fa-comments"></i>
                                    <span>Voir</span>
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>

            <asp:Panel ID="pnlNoComplaints" runat="server" Visible="false" CssClass="empty-state">
                <i class="fas fa-exclamation-circle"></i>
                <h3>Aucune réclamation trouvée</h3>
                <p>Les réclamations apparaîtront ici</p>
            </asp:Panel>

            <asp:Panel ID="pnlDetails" runat="server" Visible="false">
                <div class="details-card">
                    <div class="card-header" style="display: flex; justify-content: space-between; align-items: center;">
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
                        <div class="status-dropdown-container">
                            <label>
                                <i class="fas fa-tasks"></i>
                                Statut de la réclamation
                            </label>
                            <asp:DropDownList ID="ddlStatus" runat="server">
                                <asp:ListItem Value="Pending" Text="En attente" />
                                <asp:ListItem Value="InProgress" Text="En cours de traitement" />
                                <asp:ListItem Value="Resolved" Text="Résolue (sera automatiquement fermée)" />
                                <asp:ListItem Value="Closed" Text="Fermée" />
                            </asp:DropDownList>
                            <div class="status-dropdown-help">
                                <i class="fas fa-info-circle"></i>
                                <strong>Note importante:</strong> Si vous sélectionnez "Résolue", la réclamation sera automatiquement fermée et ne pourra plus être modifiée.
                            </div>
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

        <script type="text/javascript">
            // Enhanced dropdown styling with dynamic status colors
            function initStatusDropdown() {
                var statusDropdown = document.getElementById('<%= ddlStatus.ClientID %>');
                if (!statusDropdown) return;

                // Function to get status color
                function getStatusColor(status) {
                    switch(status) {
                        case 'Pending': return '#d97706';
                        case 'InProgress': return '#2563eb';
                        case 'Resolved': return '#059669';
                        case 'Closed': return '#64748b';
                        default: return '#e2e8f0';
                    }
                }

                // Function to update dropdown border color based on selected value
                function updateDropdownStyle() {
                    if (statusDropdown.disabled) {
                        statusDropdown.style.borderLeftColor = '#cbd5e1';
                        return;
                    }
                    var value = statusDropdown.value;
                    var color = getStatusColor(value);
                    statusDropdown.style.borderLeftColor = color;
                    statusDropdown.style.borderLeftWidth = '4px';
                }

                // Update on load
                updateDropdownStyle();

                // Update on change
                statusDropdown.addEventListener('change', updateDropdownStyle);

                // Update when enabled/disabled state changes (for postback scenarios)
                var observer = new MutationObserver(function(mutations) {
                    mutations.forEach(function(mutation) {
                        if (mutation.type === 'attributes' && mutation.attributeName === 'disabled') {
                            updateDropdownStyle();
                        }
                    });
                });
                observer.observe(statusDropdown, { attributes: true, attributeFilter: ['disabled'] });
            }

            // Initialize on page load
            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', initStatusDropdown);
            } else {
                initStatusDropdown();
            }

            // Re-initialize after postback (for ASP.NET Web Forms)
            if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initStatusDropdown);
            }
        </script>
    </asp:Content>