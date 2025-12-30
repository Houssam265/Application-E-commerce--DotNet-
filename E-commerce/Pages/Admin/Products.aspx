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

            .action-btn.activate {
                background: rgba(16, 185, 129, 0.15);
                color: #059669;
                border: 1px solid rgba(16, 185, 129, 0.25);
            }

            .action-btn.activate:hover {
                background: rgba(16, 185, 129, 0.25);
                transform: translateY(-2px);
                box-shadow: 0 4px 8px rgba(16, 185, 129, 0.2);
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
                <h1><i class="fas fa-box"></i> Produits</h1>
                <p style="color: #64748b; margin-top: 0.5rem; margin-bottom: 0;">Gérez votre catalogue de produits</p>
            </div>
            <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn-primary" OnClick="btnAddNew_Click">
                <i class="fas fa-plus-circle"></i>
                <span>Nouveau Produit</span>
            </asp:LinkButton>
        </div>

        <asp:Panel ID="pnlList" runat="server">
            <div class="search-filter-container">
                <div class="filter-row">
                    <div class="filter-group" style="flex: 2;">
                        <label><i class="fas fa-search"></i> Recherche</label>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Nom, description..." />
                    </div>
                    <div class="filter-group">
                        <label><i class="fas fa-tags"></i> Catégorie</label>
                        <asp:DropDownList ID="ddlCategoryFilter" runat="server" CssClass="search-input">
                            <asp:ListItem Value="" Text="Toutes les catégories" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="filter-group">
                        <label><i class="fas fa-toggle-on"></i> Statut</label>
                        <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="search-input">
                            <asp:ListItem Value="" Text="Tous" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="1" Text="Actif"></asp:ListItem>
                            <asp:ListItem Value="0" Text="Inactif"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="filter-group">
                        <label><i class="fas fa-dollar-sign"></i> Prix Min</label>
                        <asp:TextBox ID="txtPriceMin" runat="server" CssClass="search-input" type="number" step="0.01" placeholder="0.00" />
                    </div>
                    <div class="filter-group">
                        <label><i class="fas fa-dollar-sign"></i> Prix Max</label>
                        <asp:TextBox ID="txtPriceMax" runat="server" CssClass="search-input" type="number" step="0.01" placeholder="9999.99" />
                    </div>
                </div>
                <div class="filter-row">
                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="search-btn" OnClick="btnSearch_Click">
                        <i class="fas fa-search"></i>
                        <span>Rechercher</span>
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnClear" runat="server" CssClass="search-btn" OnClick="btnClear_Click" 
                        style="background: #64748b;">
                        <i class="fas fa-times"></i>
                        <span>Effacer les filtres</span>
                    </asp:LinkButton>
                </div>
            </div>
            <div class="products-table-container">
                <asp:GridView ID="gvProducts" runat="server" CssClass="grid-view" AutoGenerateColumns="False"
                    OnRowCommand="gvProducts_RowCommand" OnPageIndexChanging="gvProducts_PageIndexChanging"
                    GridLines="None" DataKeyNames="Id" AllowPaging="true" PageSize="10" PagerStyle-CssClass="pager">
                    <Columns>
                        <asp:TemplateField HeaderText="Image">
                            <ItemTemplate>
                                <img src='/Assets/Images/Products/<%# Eval("ImageUrl") %>' class="product-thumb"
                                    onerror="this.src='<%# ResolveUrl("~/Assets/Images/placeholder.svg") %>'" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Nom" />
                        <asp:BoundField DataField="Price" HeaderText="Prix" DataFormatString="{0:F2} MAD" />
                        <asp:BoundField DataField="StockQuantity" HeaderText="Stock" />
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
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditProd"
                                        CommandArgument='<%# Eval("Id") %>' CssClass="action-btn edit">
                                        <i class="fas fa-edit"></i>
                                        <span>Modifier</span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleActive"
                                        CommandArgument='<%# Eval("Id") %>' 
                                        CssClass='<%# GetToggleButtonClass(Eval("IsActive")) %>'
                                        OnClientClick='<%# GetToggleConfirmMessage(Eval("IsActive")) %>'>
                                        <i class='<%# GetToggleIcon(Eval("IsActive")) %>'></i>
                                        <span><%# GetToggleButtonText(Eval("IsActive")) %></span>
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
                    <asp:Label ID="lblTitle" runat="server"
                        Text="<i class=&quot;fas fa-plus&quot;></i> Ajouter un Produit"></asp:Label>
                </h2>

                <asp:HiddenField ID="hfProductId" runat="server" />

                <div class="form-group">
                    <label>Catégorie</label>
                    <asp:DropDownList ID="ddlCategories" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <label>Nom du produit</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Ex: Tomates Bio">
                    </asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4"
                        CssClass="form-control" placeholder="Décrivez votre produit..."></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Description courte</label>
                    <asp:TextBox ID="txtShortDescription" runat="server" TextMode="MultiLine" Rows="2"
                        CssClass="form-control" placeholder="Resume court pour les listes"></asp:TextBox>
                </div>

                <div class="form-group" style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
                    <div>
                        <label>Prix (MAD)</label>
                        <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" type="number" step="0.01"
                            placeholder="0.00"></asp:TextBox>
                    </div>
                    <div>
                        <label>Prix barre (MAD)</label>
                        <asp:TextBox ID="txtCompareAtPrice" runat="server" CssClass="form-control" type="number" step="0.01"
                            placeholder="0.00"></asp:TextBox>
                    </div>
                </div>

                <div class="form-group" style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
                    <div>
                        <label>Stock</label>
                        <asp:TextBox ID="txtStock" runat="server" CssClass="form-control" type="number" placeholder="0">
                        </asp:TextBox>
                    </div>
                    <div>
                        <label>Poids (kg)</label>
                        <asp:TextBox ID="txtWeight" runat="server" CssClass="form-control" type="number" step="0.01"
                            placeholder="0.00"></asp:TextBox>
                    </div>
                </div>

                <div class="form-group">
                    <label>SKU</label>
                    <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" placeholder="Ex: TOM-BIO-001">
                    </asp:TextBox>
                </div>

                <div class="form-group" style="display: flex; gap: 1.5rem; align-items: center;">
                    <label style="margin-bottom: 0;">Statut</label>
                    <label style="display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0;">
                        <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" />
                        <span>Actif</span>
                    </label>
                    <label style="display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0;">
                        <asp:CheckBox ID="chkIsFeatured" runat="server" Checked="true" />
                        <span>Mis en avant</span>
                    </label>
                </div>

                <div class="form-group">
                    <label><i class="fas fa-images"></i> Ajouter des images</label>
                    <input type="file" id="fuImages" name="fuImages" multiple="multiple" accept="image/*"
                        class="form-control" />
                    <small style="color: #94a3b8; margin-top: 0.5rem; display: block;">Vous pouvez sélectionner
                        plusieurs images à la fois (Ctrl+Click ou Cmd+Click)</small>
                </div>
                <div class="form-group">
                    <label>Apercu des images selectionnees</label>
                    <div id="selectedImagesPreview" class="images-grid"></div>
                    <small style="color: #94a3b8; margin-top: 0.5rem; display: block;">Les images s'affichent ici avant l'enregistrement.</small>
                </div>

                <script>
                    // S'assurer que le formulaire a l'enctype correct pour l'upload de fichiers
                    window.addEventListener('DOMContentLoaded', function () {
                        var form = document.querySelector('form');
                        if (form && !form.getAttribute('enctype')) {
                            form.setAttribute('enctype', 'multipart/form-data');
                        }
                    });
                </script>
                <script>
    window.addEventListener('DOMContentLoaded', function () {
        var input = document.getElementById('fuImages');
        var preview = document.getElementById('selectedImagesPreview');
        if (!input || !preview) return;

        var selectedFiles = [];
        var primaryIndex = 0;

        function rebuildInputFiles() {
            var dt = new DataTransfer();
            selectedFiles.forEach(function (file) {
                dt.items.add(file);
            });
            input.files = dt.files;
        }

        function renderPreview() {
            preview.innerHTML = '';
            selectedFiles.forEach(function (file, index) {
                if (!file || !file.type || file.type.indexOf('image/') !== 0) return;

                var wrapper = document.createElement('div');
                wrapper.className = 'image-item' + (index === primaryIndex ? ' primary' : '');

                var img = document.createElement('img');
                img.alt = file.name || 'Image';
                wrapper.appendChild(img);

                var overlay = document.createElement('div');
                overlay.className = 'image-overlay';

                var primaryBtn = document.createElement('button');
                primaryBtn.type = 'button';
                primaryBtn.className = 'image-action-btn primary-btn';
                primaryBtn.title = 'Definir comme principale';
                primaryBtn.innerHTML = '<i class="fas fa-star"></i>';
                primaryBtn.addEventListener('click', function () {
                    primaryIndex = index;
                    // Move primary to front so server marks it as primary
                    if (index !== 0) {
                        var file = selectedFiles.splice(index, 1)[0];
                        selectedFiles.unshift(file);
                        primaryIndex = 0;
                        rebuildInputFiles();
                    }
                    renderPreview();
                });

                var deleteBtn = document.createElement('button');
                deleteBtn.type = 'button';
                deleteBtn.className = 'image-action-btn delete-btn';
                deleteBtn.title = 'Supprimer';
                deleteBtn.innerHTML = '<i class="fas fa-trash"></i>';
                deleteBtn.addEventListener('click', function () {
                    selectedFiles.splice(index, 1);
                    if (primaryIndex >= selectedFiles.length) {
                        primaryIndex = 0;
                    }
                    rebuildInputFiles();
                    renderPreview();
                });

                overlay.appendChild(primaryBtn);
                overlay.appendChild(deleteBtn);
                wrapper.appendChild(overlay);

                if (index === primaryIndex) {
                    var badge = document.createElement('span');
                    badge.className = 'primary-badge';
                    badge.innerHTML = '<i class="fas fa-star"></i> Principale';
                    wrapper.appendChild(badge);
                }

                preview.appendChild(wrapper);

                var reader = new FileReader();
                reader.onload = function (e) {
                    img.src = e.target.result;
                };
                reader.readAsDataURL(file);
            });
        }

        input.addEventListener('change', function () {
            selectedFiles = Array.prototype.slice.call(input.files || []);
            primaryIndex = 0;
            renderPreview();
        });
    });
</script>

                <!-- Galerie des images existantes -->
                <div class="form-group" id="imagesGallery" runat="server">
                    <label><i class="fas fa-images"></i> Images du produit</label>
                    <div class="images-grid" id="imagesGrid" runat="server">
                        <asp:Repeater ID="rptProductImages" runat="server" OnItemCommand="rptProductImages_ItemCommand">
                            <ItemTemplate>
                                <div class="image-item <%# Eval(" IsPrimary").ToString()=="True" ? "primary" : "" %>">
                                    <img src='<%# BuildImageUrl(Eval("ImageUrl")) %>' alt="Produit" />
                                    <div class="image-overlay">
                                        <asp:LinkButton ID="btnSetPrimary" runat="server" CommandName="SetPrimary"
                                            CommandArgument='<%# Eval("Id") %>' CssClass="image-action-btn primary-btn"
                                            Visible='<%# Eval("IsPrimary").ToString() != "True" %>'
                                            title="Définir comme photo principale">
                                            <i class="fas fa-star"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDeleteImage" runat="server" CommandName="DeleteImage"
                                            CommandArgument='<%# Eval("Id") %>' CssClass="image-action-btn delete-btn"
                                            OnClientClick="return confirm('Êtes-vous sûr de vouloir supprimer cette image ?');"
                                            title="Supprimer">
                                            <i class="fas fa-trash"></i>
                                        </asp:LinkButton>
                                    </div>
                                    <%# Eval("IsPrimary").ToString()=="True"
                                        ? "<span class='primary-badge'><i class='fas fa-star'></i> Principale</span>"
                                        : "" %>
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
                    Style="display: block; margin-top: 1.5rem; padding: 1rem; border-radius: 8px; background: rgba(239, 68, 68, 0.1); color: #dc2626; border: 1px solid rgba(239, 68, 68, 0.3);">
                </asp:Label>
            </div>
        </asp:Panel>
    </asp:Content>


