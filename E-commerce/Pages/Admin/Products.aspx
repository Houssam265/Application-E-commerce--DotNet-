<%@ Page Title="Gestion Produits" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="Products.aspx.cs" Inherits="Ecommerce.Pages.Admin.Products" %>

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
            }

            .products-table-container {
                background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
                border: 1px solid #e2e8f0;
                border-radius: 16px;
                padding: 1.5rem;
                overflow-x: auto;
                box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
                transition: all 0.3s ease;
            }

            .products-table-container:hover {
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

            .product-thumb {
                width: 60px;
                height: 60px;
                object-fit: cover;
                border-radius: 8px;
                border: 2px solid #e2e8f0;
                transition: all 0.3s ease;
            }

            .product-thumb:hover {
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
                color: #64748b;
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

            /* Styles pour la galerie d'images */
            .images-grid {
                display: grid;
                grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
                gap: 1rem;
                margin-top: 1rem;
            }

            .image-item {
                position: relative;
                border: 2px solid #e2e8f0;
                border-radius: 12px;
                overflow: hidden;
                aspect-ratio: 1;
                background: #f8fafc;
                transition: all 0.3s ease;
            }

            .image-item:hover {
                border-color: #3b82f6;
                transform: translateY(-4px);
                box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
            }

            .image-item.primary {
                border-color: #f59e0b;
                border-width: 3px;
            }

            .image-item img {
                width: 100%;
                height: 100%;
                object-fit: cover;
                display: block;
            }

            .image-overlay {
                position: absolute;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: rgba(0, 0, 0, 0.6);
                display: flex;
                align-items: center;
                justify-content: center;
                gap: 0.5rem;
                opacity: 0;
                transition: opacity 0.3s ease;
            }

            .image-item:hover .image-overlay {
                opacity: 1;
            }

            .image-action-btn {
                width: 40px;
                height: 40px;
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                border: none;
                cursor: pointer;
                transition: all 0.3s ease;
                text-decoration: none;
                color: white;
                font-size: 1rem;
            }

            .image-action-btn.primary-btn {
                background: rgba(245, 158, 11, 0.9);
            }

            .image-action-btn.primary-btn:hover {
                background: #f59e0b;
                transform: scale(1.1);
            }

            .image-action-btn.delete-btn {
                background: rgba(239, 68, 68, 0.9);
            }

            .image-action-btn.delete-btn:hover {
                background: #ef4444;
                transform: scale(1.1);
            }

            .primary-badge {
                position: absolute;
                top: 8px;
                right: 8px;
                background: linear-gradient(135deg, #f59e0b, #d97706);
                color: white;
                padding: 4px 10px;
                border-radius: 20px;
                font-size: 0.75rem;
                font-weight: 600;
                display: flex;
                align-items: center;
                gap: 4px;
                box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
            }

            .primary-badge i {
                font-size: 0.7rem;
            }
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <div>
                <h1><i class="fas fa-box"></i> Produits</h1>
                <p style="color: #64748b; margin-top: 0.5rem; margin-bottom: 0;">Gérez votre catalogue de produits</p>
            </div>
            <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn-primary"
                OnClick="btnAddNew_Click">
                <i class="fas fa-plus-circle"></i>
                <span>Nouveau Produit</span>
            </asp:LinkButton>
        </div>

        <asp:Panel ID="pnlList" runat="server">
            <div class="products-table-container">
                <asp:GridView ID="gvProducts" runat="server" CssClass="grid-view" AutoGenerateColumns="False"
                    OnRowCommand="gvProducts_RowCommand" GridLines="None" DataKeyNames="Id">
                    <Columns>
                        <asp:TemplateField HeaderText="Image">
                            <ItemTemplate>
                                <img src='/Assets/Images/Products/<%# Eval("ImageUrl") %>' class="product-thumb" 
                                    onerror="this.src='https://via.placeholder.com/60x60/334155/94a3b8?text=No+Image'" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Nom" />
                        <asp:BoundField DataField="Price" HeaderText="Prix" DataFormatString="{0:F2} MAD" />
                        <asp:BoundField DataField="StockQuantity" HeaderText="Stock" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditProd"
                                        CommandArgument='<%# Eval("Id") %>' CssClass="action-btn edit">
                                        <i class="fas fa-edit"></i>
                                        <span>Modifier</span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteProd"
                                        CommandArgument='<%# Eval("Id") %>' CssClass="action-btn delete"
                                        OnClientClick="return confirm('Êtes-vous sûr de vouloir supprimer ce produit ?');">
                                        <i class="fas fa-trash"></i>
                                        <span>Supprimer</span>
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-state">
                            <i class="fas fa-box-open"></i>
                            <h3>Aucun produit trouvé</h3>
                            <p>Commencez par ajouter votre premier produit</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlEdit" runat="server" Visible="false">
            <div class="form-container">
                <h2>
                    <asp:Label ID="lblTitle" runat="server" Text="<i class=&quot;fas fa-plus&quot;></i> Ajouter un Produit"></asp:Label>
                </h2>

                <asp:HiddenField ID="hfProductId" runat="server" />

                <div class="form-group">
                    <label>Catégorie</label>
                    <asp:DropDownList ID="ddlCategories" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <label>Nom du produit</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Ex: Tomates Bio"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4"
                        CssClass="form-control" placeholder="Décrivez votre produit..."></asp:TextBox>
                </div>

                <div class="form-group" style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
                    <div>
                        <label>Prix (MAD)</label>
                        <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" type="number" step="0.01" placeholder="0.00"></asp:TextBox>
                    </div>
                    <div>
                        <label>Stock</label>
                        <asp:TextBox ID="txtStock" runat="server" CssClass="form-control" type="number" placeholder="0"></asp:TextBox>
                    </div>
                </div>

                <div class="form-group">
                    <label><i class="fas fa-images"></i> Ajouter des images</label>
                    <input type="file" id="fuImages" name="fuImages" multiple="multiple" accept="image/*" class="form-control" />
                    <small style="color: #94a3b8; margin-top: 0.5rem; display: block;">Vous pouvez sélectionner plusieurs images à la fois (Ctrl+Click ou Cmd+Click)</small>
                </div>

                <script>
                    // S'assurer que le formulaire a l'enctype correct pour l'upload de fichiers
                    window.addEventListener('DOMContentLoaded', function() {
                        var form = document.querySelector('form');
                        if (form && !form.getAttribute('enctype')) {
                            form.setAttribute('enctype', 'multipart/form-data');
                        }
                    });
                </script>

                <!-- Galerie des images existantes -->
                <div class="form-group" id="imagesGallery" runat="server">
                    <label><i class="fas fa-images"></i> Images du produit</label>
                    <div class="images-grid" id="imagesGrid" runat="server">
                        <asp:Repeater ID="rptProductImages" runat="server" OnItemCommand="rptProductImages_ItemCommand">
                            <ItemTemplate>
                                <div class="image-item <%# Eval("IsPrimary").ToString() == "True" ? "primary" : "" %>">
                                    <img src='<%# BuildImageUrl(Eval("ImageUrl")) %>' alt="Produit" />
                                    <div class="image-overlay">
                                        <asp:LinkButton ID="btnSetPrimary" runat="server" 
                                            CommandName="SetPrimary" 
                                            CommandArgument='<%# Eval("Id") %>'
                                            CssClass="image-action-btn primary-btn"
                                            Visible='<%# Eval("IsPrimary").ToString() != "True" %>'
                                            title="Définir comme photo principale">
                                            <i class="fas fa-star"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDeleteImage" runat="server" 
                                            CommandName="DeleteImage" 
                                            CommandArgument='<%# Eval("Id") %>'
                                            CssClass="image-action-btn delete-btn"
                                            OnClientClick="return confirm('Êtes-vous sûr de vouloir supprimer cette image ?');"
                                            title="Supprimer">
                                            <i class="fas fa-trash"></i>
                                        </asp:LinkButton>
                                    </div>
                                    <%# Eval("IsPrimary").ToString() == "True" ? "<span class='primary-badge'><i class='fas fa-star'></i> Principale</span>" : "" %>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <asp:Label ID="lblNoImages" runat="server" Text="Aucune image pour ce produit." Visible="false" 
                        style="color: #94a3b8; font-style: italic; display: block; margin-top: 1rem;"></asp:Label>
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
            </div>
        </asp:Panel>
    </asp:Content>