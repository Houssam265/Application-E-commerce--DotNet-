<%@ Page Title="Gestion Catégories" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="Categories.aspx.cs" Inherits="Ecommerce.Pages.Admin.Categories" UnobtrusiveValidationMode="None" %>

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

            .category-image {
                width: 60px;
                height: 60px;
                object-fit: cover;
                border-radius: 8px;
                border: 2px solid #e2e8f0;
                transition: all 0.3s ease;
            }

            .category-image:hover {
                transform: scale(1.1);
                border-color: #3b82f6;
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
                max-width: 700px;
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
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <div>
                <h1><i class="fas fa-tags"></i> Catégories</h1>
                <p style="color: #64748b; margin-top: 0.5rem; margin-bottom: 0;">Gérez les catégories de produits</p>
            </div>
            <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn-primary"
                OnClick="btnAddNew_Click">
                <i class="fas fa-plus-circle"></i>
                <span>Nouvelle Catégorie</span>
            </asp:LinkButton>
        </div>

        <asp:Panel ID="pnlList" runat="server">
            <div class="categories-table-container">
                <asp:GridView ID="gvCategories" runat="server" CssClass="grid-view" AutoGenerateColumns="False"
                    OnRowCommand="gvCategories_RowCommand" GridLines="None" DataKeyNames="Id">
                    <Columns>
                        <asp:TemplateField HeaderText="Image">
                            <ItemTemplate>
                                <img src='<%# GetCategoryImageUrl(Eval("ImageUrl")) %>' 
                                    class="category-image" 
                                    onerror="this.src='https://via.placeholder.com/60x60/e2e8f0/94a3b8?text=No+Image'" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Nom" />
                        <asp:TemplateField HeaderText="Description">
                            <ItemTemplate>
                                <span style="color: #64748b; font-size: 0.9rem;">
                                    <%# GetCategoryDescription(Eval("Description")) %>
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
                        <asp:BoundField DataField="DisplayOrder" HeaderText="Ordre" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditCat"
                                        CommandArgument='<%# Eval("Id") %>' CssClass="action-btn edit">
                                        <i class="fas fa-edit"></i>
                                        <span>Modifier</span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteCat"
                                        CommandArgument='<%# Eval("Id") %>' CssClass="action-btn delete"
                                        OnClientClick="return confirm('Êtes-vous sûr de vouloir supprimer cette catégorie ?');">
                                        <i class="fas fa-trash"></i>
                                        <span>Supprimer</span>
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-state">
                            <i class="fas fa-tags"></i>
                            <h3>Aucune catégorie trouvée</h3>
                            <p>Commencez par ajouter votre première catégorie</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlEdit" runat="server" Visible="false">
            <div class="form-container">
                <h2>
                    <asp:Label ID="lblTitle" runat="server" Text="<i class=&quot;fas fa-plus&quot;></i> Ajouter une Catégorie"></asp:Label>
                </h2>

                <asp:HiddenField ID="hfCategoryId" runat="server" />

                <div class="form-group">
                    <label>Nom de la catégorie *</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Ex: Agriculture"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                        ErrorMessage="Le nom est requis" ForeColor="#ef4444" Display="Dynamic" />
                </div>

                <div class="form-group">
                    <label>Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4"
                        CssClass="form-control" placeholder="Décrivez la catégorie..."></asp:TextBox>
                </div>

                <div class="form-group" style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
                    <div>
                        <label>Ordre d'affichage</label>
                        <asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="form-control" type="number" value="0"></asp:TextBox>
                    </div>
                    <div>
                        <label>Statut</label>
                        <asp:DropDownList ID="ddlIsActive" runat="server" CssClass="form-control">
                            <asp:ListItem Value="1" Text="Actif" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="0" Text="Inactif"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="form-group">
                    <label>Image de la catégorie</label>
                    <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" />
                    <small style="color: #94a3b8; margin-top: 0.5rem; display: block;">Laisser vide pour garder l'image actuelle</small>
                </div>

                <div class="form-group">
                    <label>URL de l'image (alternative)</label>
                    <asp:TextBox ID="txtImageUrl" runat="server" CssClass="form-control" placeholder="https://..."></asp:TextBox>
                    <small style="color: #94a3b8; margin-top: 0.5rem; display: block;">Ou entrez une URL d'image</small>
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
                    Style="display: block; margin-top: 1.5rem; padding: 1rem; border-radius: 8px; background: rgba(239, 68, 68, 0.1); color: #dc2626; border: 1px solid rgba(239, 68, 68, 0.3);"></asp:Label>
                <asp:Label ID="lblSuccess" runat="server" Visible="false"
                    Style="display: block; margin-top: 1.5rem; padding: 1rem; border-radius: 8px; background: rgba(16, 185, 129, 0.1); color: #059669; border: 1px solid rgba(16, 185, 129, 0.3);"></asp:Label>
            </div>
        </asp:Panel>
    </asp:Content>

