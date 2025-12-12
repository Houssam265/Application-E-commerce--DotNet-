<%@ Page Title="Gestion Produits" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="Products.aspx.cs" Inherits="Ecommerce.Pages.Admin.Products" %>

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

            .product-thumb {
                width: 50px;
                height: 50px;
                object-fit: cover;
                border-radius: 4px;
            }

            .action-link {
                color: var(--accent);
                margin-right: 1rem;
                cursor: pointer;
            }

            .action-link.delete {
                color: #ef4444;
            }
        </style>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem;">
            <h1>Produits</h1>
            <asp:Button ID="btnAddNew" runat="server" Text="+ Nouveau Produit" CssClass="btn btn-primary"
                OnClick="btnAddNew_Click" />
        </div>

        <asp:Panel ID="pnlList" runat="server">
            <asp:GridView ID="gvProducts" runat="server" CssClass="grid-view" AutoGenerateColumns="False"
                OnRowCommand="gvProducts_RowCommand" GridLines="None" DataKeyNames="Id">
                <Columns>
                    <asp:TemplateField HeaderText="Image">
                        <ItemTemplate>
                            <img src='/Assets/Images/Products/<%# Eval("ImageUrl") %>' class="product-thumb" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Nom" />
                    <asp:BoundField DataField="Price" HeaderText="Prix" DataFormatString="{0:C}" />
                    <asp:BoundField DataField="StockQuantity" HeaderText="Stock" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditProd"
                                CommandArgument='<%# Eval("Id") %>' CssClass="action-link">Modifier</asp:LinkButton>
                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteProd"
                                CommandArgument='<%# Eval("Id") %>' CssClass="action-link delete"
                                OnClientClick="return confirm('Êtes-vous sûr ?');">Supprimer</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div style="padding: 2rem; text-align: center; color: var(--text-muted);">
                        Aucun produit trouvé.
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </asp:Panel>

        <asp:Panel ID="pnlEdit" runat="server" Visible="false">
            <div class="card" style="max-width: 600px;">
                <h2 style="margin-bottom: 1.5rem;">
                    <asp:Label ID="lblTitle" runat="server" Text="Ajouter/Modifier Produit"></asp:Label>
                </h2>

                <asp:HiddenField ID="hfProductId" runat="server" />

                <div class="form-group" style="margin-bottom: 1rem;">
                    <label style="display:block; margin-bottom:0.5rem">Catégorie</label>
                    <asp:DropDownList ID="ddlCategories" runat="server" CssClass="form-control"
                        style="background: rgba(255,255,255,0.05); color: white; border: 1px solid var(--glass-border); padding: 0.5rem; width: 100%; border-radius: 8px;">
                    </asp:DropDownList>
                </div>

                <div class="form-group" style="margin-bottom: 1rem;">
                    <label style="display:block; margin-bottom:0.5rem">Nom</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control"
                        style="background: rgba(255,255,255,0.05); color: white; border: 1px solid var(--glass-border); padding: 0.5rem; width: 100%; border-radius: 8px;">
                    </asp:TextBox>
                </div>

                <div class="form-group" style="margin-bottom: 1rem;">
                    <label style="display:block; margin-bottom:0.5rem">Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4"
                        CssClass="form-control"
                        style="background: rgba(255,255,255,0.05); color: white; border: 1px solid var(--glass-border); padding: 0.5rem; width: 100%; border-radius: 8px;">
                    </asp:TextBox>
                </div>

                <div class="form-group"
                    style="margin-bottom: 1rem; display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
                    <div>
                        <label style="display:block; margin-bottom:0.5rem">Prix</label>
                        <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control"
                            style="background: rgba(255,255,255,0.05); color: white; border: 1px solid var(--glass-border); padding: 0.5rem; width: 100%; border-radius: 8px;">
                        </asp:TextBox>
                    </div>
                    <div>
                        <label style="display:block; margin-bottom:0.5rem">Stock</label>
                        <asp:TextBox ID="txtStock" runat="server" CssClass="form-control"
                            style="background: rgba(255,255,255,0.05); color: white; border: 1px solid var(--glass-border); padding: 0.5rem; width: 100%; border-radius: 8px;">
                        </asp:TextBox>
                    </div>
                </div>

                <div class="form-group" style="margin-bottom: 1.5rem;">
                    <label style="display:block; margin-bottom:0.5rem">Image (Laisser vide pour garder
                        l'actuelle)</label>
                    <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control"
                        style="background: rgba(255,255,255,0.05); color: white; border: 1px solid var(--glass-border); padding: 0.5rem; width: 100%; border-radius: 8px;" />
                </div>

                <div style="display: flex; gap: 1rem;">
                    <asp:Button ID="btnSave" runat="server" Text="Enregistrer" CssClass="btn btn-primary"
                        OnClick="btnSave_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Annuler" CssClass="btn"
                        style="background: rgba(255,255,255,0.1);" OnClick="btnCancel_Click" />
                </div>

                <asp:Label ID="lblError" runat="server" Visible="false"
                    Style="display: block; margin-top: 1rem; color: #fca5a5;"></asp:Label>
            </div>
        </asp:Panel>
    </asp:Content>