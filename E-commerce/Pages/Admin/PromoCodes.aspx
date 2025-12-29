<%@ Page Title="Gestion Codes Promo" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="PromoCodes.aspx.cs" Inherits="Ecommerce.Pages.Admin.PromoCodes" UnobtrusiveValidationMode="None" %>

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

        .promocodes-table-container {
            background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
            border: 1px solid #e2e8f0;
            border-radius: 16px;
            padding: 1.5rem;
            overflow-x: auto;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            transition: all 0.3s ease;
        }

        .promocodes-table-container:hover {
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

        .grid-view tbody tr.inactive-row {
            opacity: 0.6;
            background-color: #f8fafc;
        }

        .grid-view tbody tr:hover {
            background: rgba(59, 130, 246, 0.05);
            transform: scale(1.01);
        }

        .grid-view tbody tr.inactive-row:hover {
            background: rgba(148, 163, 184, 0.1);
        }

        .grid-view td {
            color: #1e293b;
        }

        .code-badge {
            display: inline-block;
            padding: 0.5rem 1rem;
            background: linear-gradient(135deg, #3b82f6, #8b5cf6);
            color: white;
            border-radius: 8px;
            font-weight: 700;
            font-size: 0.95rem;
            letter-spacing: 1px;
            font-family: 'Courier New', monospace;
            box-shadow: 0 2px 4px rgba(59, 130, 246, 0.3);
        }

        .action-buttons {
            display: flex;
            gap: 0.75rem;
            flex-wrap: wrap;
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

        .action-btn.toggle {
            background: rgba(245, 158, 11, 0.15);
            color: #d97706;
            border: 1px solid rgba(245, 158, 11, 0.25);
        }

        .action-btn.toggle:hover {
            background: rgba(245, 158, 11, 0.25);
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(245, 158, 11, 0.2);
        }

        .action-btn.delete {
            background: rgba(239, 68, 68, 0.15);
            color: #dc2626;
            border: 1px solid rgba(239, 68, 68, 0.25);
        }

        .action-btn.delete:hover {
            background: rgba(239, 68, 68, 0.25);
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(239, 68, 68, 0.2);
        }

        .form-container {
            background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
            border: 1px solid #e2e8f0;
            border-radius: 16px;
            padding: 2rem;
            max-width: 800px;
            margin-top: 2rem;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            transition: all 0.3s ease;
            animation: fadeInUp 0.5s ease-out;
        }

        .form-container:hover {
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
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

        .form-container h2 {
            margin-bottom: 1.5rem;
            color: #1e293b;
            font-family: 'Outfit', sans-serif;
        }

        .form-group {
            margin-bottom: 1.5rem;
        }

        .form-group label {
            display: block;
            margin-bottom: 0.5rem;
            color: #475569;
            font-weight: 500;
            font-size: 0.9rem;
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

        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 1rem;
        }

        .form-actions {
            display: flex;
            gap: 1rem;
            margin-top: 2rem;
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

        .discount-info {
            font-weight: 600;
            color: #059669;
        }

        .usage-info {
            color: #64748b;
            font-size: 0.9rem;
        }

        .search-filter-container {
            background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 1.5rem;
            margin-bottom: 1.5rem;
            display: flex;
            gap: 1rem;
            align-items: center;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
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
            <h1><i class="fas fa-ticket-alt"></i> Codes Promo</h1>
            <p style="color: #64748b; margin-top: 0.5rem; margin-bottom: 0;">Gérez vos codes promo et réductions</p>
        </div>
        <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn-primary" OnClick="btnAddNew_Click">
            <i class="fas fa-plus-circle"></i>
            <span>Nouveau Code Promo</span>
        </asp:LinkButton>
    </div>

    <asp:Panel ID="pnlList" runat="server">
        <div class="search-filter-container">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Rechercher par code..." />
            <asp:LinkButton ID="btnSearch" runat="server" CssClass="search-btn" OnClick="btnSearch_Click">
                <i class="fas fa-search"></i>
                <span>Rechercher</span>
            </asp:LinkButton>
            <asp:LinkButton ID="btnClear" runat="server" CssClass="search-btn" OnClick="btnClear_Click" 
                style="background: #64748b;" Visible="false">
                <i class="fas fa-times"></i>
                <span>Effacer</span>
            </asp:LinkButton>
        </div>
        <div class="promocodes-table-container">
            <asp:GridView ID="gvPromoCodes" runat="server" CssClass="grid-view" AutoGenerateColumns="False"
                OnRowCommand="gvPromoCodes_RowCommand" OnRowDataBound="gvPromoCodes_RowDataBound" 
                OnPageIndexChanging="gvPromoCodes_PageIndexChanging"
                GridLines="None" DataKeyNames="Id" AllowPaging="true" PageSize="10" PagerStyle-CssClass="pager">
                <Columns>
                    <asp:TemplateField HeaderText="Code">
                        <ItemTemplate>
                            <span class="code-badge"><%# Eval("Code") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Réduction">
                        <ItemTemplate>
                            <span class="discount-info">
                                <%# GetDiscountText(Eval("DiscountType"), Eval("DiscountValue")) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Min. Montant">
                        <ItemTemplate>
                            <%# GetMinimumAmountText(Eval("MinimumAmount")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Validité">
                        <ItemTemplate>
                            <%# GetValidityText(Eval("StartDate"), Eval("EndDate")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Utilisation">
                        <ItemTemplate>
                            <span class="usage-info">
                                <%# GetUsageText(Eval("UsedCount"), Eval("UsageLimit")) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Statut">
                        <ItemTemplate>
                            <span class="status-badge <%# GetStatusClass(Eval("IsActive")) %>">
                                <%# GetStatusText(Eval("IsActive")) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <div class="action-buttons">
                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditCoupon"
                                    CommandArgument='<%# Eval("Id") %>' CssClass="action-btn edit">
                                    <i class="fas fa-edit"></i>
                                    <span>Modifier</span>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleCoupon"
                                    CommandArgument='<%# Eval("Id") %>' CssClass="action-btn toggle">
                                    <i class='<%# System.Convert.ToBoolean(Eval("IsActive")) ? "fas fa-eye-slash" : "fas fa-eye" %>'></i>
                                    <span><%# System.Convert.ToBoolean(Eval("IsActive")) ? "Désactiver" : "Activer" %></span>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteCoupon"
                                    CommandArgument='<%# Eval("Id") %>' CssClass="action-btn delete"
                                    OnClientClick="return confirm('Êtes-vous sûr de vouloir supprimer ce code promo ?');">
                                    <i class="fas fa-trash"></i>
                                    <span>Supprimer</span>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="empty-state">
                        <i class="fas fa-ticket-alt"></i>
                        <h3>Aucun code promo trouvé</h3>
                        <p>Commencez par ajouter votre premier code promo</p>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlEdit" runat="server" Visible="false">
        <div class="form-container">
            <h2>
                <asp:Label ID="lblTitle" runat="server"
                    Text="<i class=&quot;fas fa-plus&quot;></i> Ajouter un Code Promo"></asp:Label>
            </h2>

            <asp:HiddenField ID="hfCouponId" runat="server" />

            <div class="form-group">
                <label>Code Promo *</label>
                <asp:TextBox ID="txtCode" runat="server" CssClass="form-control" placeholder="Ex: WELCOME20"
                    style="text-transform: uppercase; font-family: 'Courier New', monospace; font-weight: 700; letter-spacing: 2px;">
                </asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                    ErrorMessage="Le code est requis" ForeColor="#ef4444" Display="Dynamic" />
                <small style="color: #94a3b8; margin-top: 0.5rem; display: block;">Utilisez des lettres et chiffres uniquement (ex: WELCOME20, SAVE15)</small>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label>Type de réduction *</label>
                    <asp:DropDownList ID="ddlDiscountType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDiscountType_SelectedIndexChanged">
                        <asp:ListItem Value="Percentage" Text="Pourcentage (%)" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="Fixed" Text="Montant fixe (MAD)"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label>Valeur de la réduction *</label>
                    <asp:TextBox ID="txtDiscountValue" runat="server" CssClass="form-control" type="number" step="0.01" min="0" placeholder="20">
                    </asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDiscountValue" runat="server" ControlToValidate="txtDiscountValue"
                        ErrorMessage="La valeur est requise" ForeColor="#ef4444" Display="Dynamic" />
                    <asp:Label ID="lblDiscountHint" runat="server" style="color: #94a3b8; font-size: 0.85rem; margin-top: 0.5rem; display: block;">
                        Entrez un pourcentage (ex: 20 pour 20%) ou un montant fixe
                    </asp:Label>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label>Montant minimum (MAD)</label>
                    <asp:TextBox ID="txtMinimumAmount" runat="server" CssClass="form-control" type="number" step="0.01" min="0" placeholder="0">
                    </asp:TextBox>
                    <small style="color: #94a3b8; margin-top: 0.5rem; display: block;">Montant minimum du panier pour appliquer ce code (0 = aucun minimum)</small>
                </div>
                <div class="form-group">
                    <label>Réduction maximum (MAD) <span id="maxDiscountLabel" runat="server">(Pourcentage uniquement)</span></label>
                    <asp:TextBox ID="txtMaximumDiscount" runat="server" CssClass="form-control" type="number" step="0.01" min="0" placeholder="0">
                    </asp:TextBox>
                    <small style="color: #94a3b8; margin-top: 0.5rem; display: block;">Limite de réduction (0 = pas de limite)</small>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label>Date de début *</label>
                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" type="datetime-local">
                    </asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="txtStartDate"
                        ErrorMessage="La date de début est requise" ForeColor="#ef4444" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Date de fin *</label>
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" type="datetime-local">
                    </asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="txtEndDate"
                        ErrorMessage="La date de fin est requise" ForeColor="#ef4444" Display="Dynamic" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label>Limite d'utilisation</label>
                    <asp:TextBox ID="txtUsageLimit" runat="server" CssClass="form-control" type="number" min="0" placeholder="0">
                    </asp:TextBox>
                    <small style="color: #94a3b8; margin-top: 0.5rem; display: block;">Nombre maximum d'utilisations (0 = illimité)</small>
                </div>
                <div class="form-group">
                    <label>Statut</label>
                    <asp:DropDownList ID="ddlIsActive" runat="server" CssClass="form-control">
                        <asp:ListItem Value="1" Text="Actif" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="0" Text="Inactif"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="form-actions">
                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click">
                    <i class="fas fa-save"></i>
                    <span>Enregistrer</span>
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn secondary" OnClick="btnCancel_Click"
                    style="background: #f1f5f9; color: #64748b; border: 1px solid #e2e8f0;">
                    <i class="fas fa-times"></i>
                    <span>Annuler</span>
                </asp:LinkButton>
            </div>

            <asp:Label ID="lblError" runat="server" Visible="false"
                Style="display: block; margin-top: 1.5rem; padding: 1rem; border-radius: 8px; background: rgba(239, 68, 68, 0.1); color: #dc2626; border: 1px solid rgba(239, 68, 68, 0.3);">
            </asp:Label>
            <asp:Label ID="lblSuccess" runat="server" Visible="false"
                Style="display: block; margin-top: 1.5rem; padding: 1rem; border-radius: 8px; background: rgba(16, 185, 129, 0.1); color: #059669; border: 1px solid rgba(16, 185, 129, 0.3);">
            </asp:Label>
        </div>
    </asp:Panel>
</asp:Content>

