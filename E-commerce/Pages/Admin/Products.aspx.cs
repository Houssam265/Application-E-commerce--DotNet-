using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;

namespace Ecommerce.Pages.Admin
{
    public partial class Products : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.LinkButton btnAddNew;
        protected global::System.Web.UI.WebControls.Panel pnlList;
        protected global::System.Web.UI.WebControls.GridView gvProducts;
        protected global::System.Web.UI.WebControls.Panel pnlEdit;
        protected global::System.Web.UI.WebControls.Label lblTitle;
        protected global::System.Web.UI.WebControls.HiddenField hfProductId;
        protected global::System.Web.UI.WebControls.DropDownList ddlCategories;
        protected global::System.Web.UI.WebControls.TextBox txtName;
        protected global::System.Web.UI.WebControls.TextBox txtDescription;
        protected global::System.Web.UI.WebControls.TextBox txtPrice;
        protected global::System.Web.UI.WebControls.TextBox txtStock;
        protected global::System.Web.UI.WebControls.LinkButton btnSave;
        protected global::System.Web.UI.WebControls.LinkButton btnCancel;
        protected global::System.Web.UI.WebControls.Label lblError;
        protected global::System.Web.UI.WebControls.Repeater rptProductImages;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl imagesGallery;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl imagesGrid;
        protected global::System.Web.UI.WebControls.Label lblNoImages;

        protected global::System.Web.UI.WebControls.TextBox txtSearch;
        protected global::System.Web.UI.WebControls.LinkButton btnSearch;
        protected global::System.Web.UI.WebControls.LinkButton btnClear;
        protected global::System.Web.UI.WebControls.DropDownList ddlCategoryFilter;
        protected global::System.Web.UI.WebControls.DropDownList ddlStatusFilter;
        protected global::System.Web.UI.WebControls.TextBox txtPriceMin;
        protected global::System.Web.UI.WebControls.TextBox txtPriceMax;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();
                LoadCategoriesFilter();
                LoadProducts();
            }
        }

        private void LoadCategoriesFilter()
        {
            DbContext db = new DbContext();
            DataTable dt = db.ExecuteQuery("SELECT Id, Name FROM Categories WHERE IsActive = 1 ORDER BY Name ASC");
            ddlCategoryFilter.Items.Clear();
            ddlCategoryFilter.Items.Add(new ListItem("Toutes les catégories", ""));
            foreach (DataRow row in dt.Rows)
            {
                ddlCategoryFilter.Items.Add(new ListItem(row["Name"].ToString(), row["Id"].ToString()));
            }
        }

        private void LoadProducts(string searchTerm = "", string categoryId = "", string status = "", string priceMin = "", string priceMax = "")
        {
            DbContext db = new DbContext();
            string query = "SELECT * FROM Products WHERE 1=1";
            System.Collections.Generic.List<SqlParameter> parameters = new System.Collections.Generic.List<SqlParameter>();
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (Name LIKE @Search OR Description LIKE @Search)";
                parameters.Add(new SqlParameter("@Search", "%" + searchTerm + "%"));
            }
            
            if (!string.IsNullOrEmpty(categoryId))
            {
                query += " AND CategoryId = @CategoryId";
                parameters.Add(new SqlParameter("@CategoryId", categoryId));
            }
            
            if (!string.IsNullOrEmpty(status))
            {
                query += " AND IsActive = @Status";
                parameters.Add(new SqlParameter("@Status", status == "1"));
            }
            
            if (!string.IsNullOrEmpty(priceMin))
            {
                decimal minPrice;
                if (decimal.TryParse(priceMin, out minPrice))
                {
                    query += " AND Price >= @PriceMin";
                    parameters.Add(new SqlParameter("@PriceMin", minPrice));
                }
            }
            
            if (!string.IsNullOrEmpty(priceMax))
            {
                decimal maxPrice;
                if (decimal.TryParse(priceMax, out maxPrice))
                {
                    query += " AND Price <= @PriceMax";
                    parameters.Add(new SqlParameter("@PriceMax", maxPrice));
                }
            }
            
            query += " ORDER BY CreatedAt DESC";
            
            DataTable dt;
            if (parameters.Count > 0)
            {
                dt = db.ExecuteQuery(query, parameters.ToArray());
            }
            else
            {
                dt = db.ExecuteQuery(query);
            }
            
            gvProducts.DataSource = dt;
            gvProducts.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch != null ? txtSearch.Text.Trim() : "";
                string categoryId = ddlCategoryFilter != null && ddlCategoryFilter.SelectedValue != null ? ddlCategoryFilter.SelectedValue : "";
                string status = ddlStatusFilter != null && ddlStatusFilter.SelectedValue != null ? ddlStatusFilter.SelectedValue : "";
                string priceMin = txtPriceMin != null ? txtPriceMin.Text.Trim() : "";
                string priceMax = txtPriceMax != null ? txtPriceMax.Text.Trim() : "";
                LoadProducts(searchTerm, categoryId, status, priceMin, priceMax);
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            if (ddlCategoryFilter.Items.Count > 0)
                ddlCategoryFilter.SelectedIndex = 0;
            if (ddlStatusFilter.Items.Count > 0)
                ddlStatusFilter.SelectedIndex = 0;
            txtPriceMin.Text = "";
            txtPriceMax.Text = "";
            LoadProducts();
        }

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProducts.PageIndex = e.NewPageIndex;
            string searchTerm = txtSearch != null ? txtSearch.Text.Trim() : "";
            string categoryId = ddlCategoryFilter != null && ddlCategoryFilter.SelectedValue != null ? ddlCategoryFilter.SelectedValue : "";
            string status = ddlStatusFilter != null && ddlStatusFilter.SelectedValue != null ? ddlStatusFilter.SelectedValue : "";
            string priceMin = txtPriceMin != null ? txtPriceMin.Text.Trim() : "";
            string priceMax = txtPriceMax != null ? txtPriceMax.Text.Trim() : "";
            LoadProducts(searchTerm, categoryId, status, priceMin, priceMax);
        }

        private void LoadCategories()
        {
            DbContext db = new DbContext();
            DataTable dt = db.ExecuteQuery("SELECT * FROM Categories");
            ddlCategories.DataSource = dt;
            ddlCategories.DataTextField = "Name";
            ddlCategories.DataValueField = "Id";
            ddlCategories.DataBind();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            pnlList.Visible = false;
            pnlEdit.Visible = true;
            btnAddNew.Visible = false;
            
            hfProductId.Value = "";
            txtName.Text = "";
            txtDescription.Text = "";
            txtPrice.Text = "0.00";
            txtStock.Text = "0";
            lblTitle.Text = "<i class=\"fas fa-plus\"></i> Ajouter un Produit";
            
            // Masquer la galerie pour un nouveau produit
            imagesGallery.Visible = false;
            lblNoImages.Visible = false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlEdit.Visible = false;
            pnlList.Visible = true;
            btnAddNew.Visible = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text;
                string desc = txtDescription.Text;
                decimal price = decimal.Parse(txtPrice.Text);
                int stock = int.Parse(txtStock.Text);
                int catId = int.Parse(ddlCategories.SelectedValue);

                DbContext db = new DbContext();
                string productId = "";

                if (string.IsNullOrEmpty(hfProductId.Value))
                {
                    // Nouveau produit
                    string query = "INSERT INTO Products (CategoryId, Name, Description, Price, StockQuantity, ImageUrl) VALUES (@CatId, @Name, @Desc, @Price, @Stock, @Img); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    string defaultImage = "placeholder.jpg";
                    
                    SqlParameter[] p = {
                        new SqlParameter("@CatId", catId),
                        new SqlParameter("@Name", name),
                        new SqlParameter("@Desc", desc),
                        new SqlParameter("@Price", price),
                        new SqlParameter("@Stock", stock),
                        new SqlParameter("@Img", defaultImage)
                    };
                    
                    object result = db.ExecuteScalar(query, p);
                    productId = result?.ToString() ?? "";
                    
                    if (!string.IsNullOrEmpty(productId))
                    {
                        db.ExecuteNonQuery(
                            @"INSERT INTO Notifications (UserId, Title, Message, Type) 
                              SELECT Id, 'Nouveau produit: ' + @ProdName, 'ProductId=' + CAST(@ProductId AS NVARCHAR(20)), 'Product'
                              FROM Users WHERE Role = 'Customer' AND IsActive = 1",
                            new SqlParameter[] {
                                new SqlParameter("@ProdName", name),
                                new SqlParameter("@ProductId", productId)
                            });
                    }
                }
                else
                {
                    // Mise à jour produit existant
                    productId = hfProductId.Value;
                    string query = "UPDATE Products SET CategoryId=@CatId, Name=@Name, Description=@Desc, Price=@Price, StockQuantity=@Stock WHERE Id=@Id";
                    
                    SqlParameter[] p = {
                        new SqlParameter("@CatId", catId),
                        new SqlParameter("@Name", name),
                        new SqlParameter("@Desc", desc),
                        new SqlParameter("@Price", price),
                        new SqlParameter("@Stock", stock),
                        new SqlParameter("@Id", productId)
                    };
                    
                    db.ExecuteNonQuery(query, p);
                }

                // Gérer l'upload des images multiples
                HttpFileCollection files = Request.Files;
                if (files.Count > 0)
                {
                    string uploadPath = Server.MapPath("~/Assets/Images/Products/");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    bool isFirstImage = true;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFile file = files[i];
                        if (file != null && file.ContentLength > 0 && file.FileName != "")
                        {
                            string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string fullPath = uploadPath + filename;
                            file.SaveAs(fullPath);
                            
                            // Insérer dans ProductImages
                            string imageQuery = @"INSERT INTO ProductImages (ProductId, ImageUrl, IsPrimary, DisplayOrder) 
                                                VALUES (@ProductId, @ImageUrl, @IsPrimary, @DisplayOrder)";
                            
                            // La première image devient principale si aucune image principale n'existe
                            bool isPrimary = false;
                            if (isFirstImage)
                            {
                                object primaryCount = db.ExecuteScalar(
                                    "SELECT COUNT(*) FROM ProductImages WHERE ProductId = @ProductId AND IsPrimary = 1",
                                    new SqlParameter[] { new SqlParameter("@ProductId", productId) });
                                
                                if (Convert.ToInt32(primaryCount) == 0)
                                {
                                    isPrimary = true;
                                }
                            }
                            
                            // Obtenir le DisplayOrder maximum
                            object maxOrder = db.ExecuteScalar(
                                "SELECT ISNULL(MAX(DisplayOrder), 0) FROM ProductImages WHERE ProductId = @ProductId",
                                new SqlParameter[] { new SqlParameter("@ProductId", productId) });
                            
                            int displayOrder = Convert.ToInt32(maxOrder) + 1;
                            
                            SqlParameter[] imgParams = {
                                new SqlParameter("@ProductId", productId),
                                new SqlParameter("@ImageUrl", filename),
                                new SqlParameter("@IsPrimary", isPrimary),
                                new SqlParameter("@DisplayOrder", displayOrder)
                            };
                            
                            db.ExecuteNonQuery(imageQuery, imgParams);
                            
                            // Mettre à jour ImageUrl du produit avec la première image principale
                            if (isPrimary)
                            {
                                db.ExecuteNonQuery(
                                    "UPDATE Products SET ImageUrl = @ImageUrl WHERE Id = @ProductId",
                                    new SqlParameter[] {
                                        new SqlParameter("@ImageUrl", filename),
                                        new SqlParameter("@ProductId", productId)
                                    });
                            }
                            
                            isFirstImage = false;
                        }
                    }
                }

                // Recharger les images si on est en mode édition
                if (!string.IsNullOrEmpty(hfProductId.Value))
                {
                    LoadProductImages(productId);
                }

                pnlEdit.Visible = false;
                pnlList.Visible = true;
                btnAddNew.Visible = true;
                string searchTerm = txtSearch != null ? txtSearch.Text.Trim() : "";
                string categoryId = ddlCategoryFilter != null ? ddlCategoryFilter.SelectedValue : "";
                string status = ddlStatusFilter != null ? ddlStatusFilter.SelectedValue : "";
                string priceMin = txtPriceMin != null ? txtPriceMin.Text.Trim() : "";
                string priceMax = txtPriceMax != null ? txtPriceMax.Text.Trim() : "";
                LoadProducts(searchTerm, categoryId, status, priceMin, priceMax); 
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditProd")
            {
                string id = e.CommandArgument.ToString();
                LoadProductForEdit(id);
            }
            else if (e.CommandName == "ToggleActive")
            {
                string id = e.CommandArgument.ToString();
                try
                {
                    DbContext db = new DbContext();
                    // Récupérer le statut actuel
                    DataTable dt = db.ExecuteQuery("SELECT IsActive FROM Products WHERE Id = @Id", 
                        new SqlParameter[] { new SqlParameter("@Id", id) });
                    
                    if (dt.Rows.Count > 0)
                    {
                        bool currentStatus = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                        bool newStatus = !currentStatus;
                        
                        // Mettre à jour le statut
                        db.ExecuteNonQuery("UPDATE Products SET IsActive = @IsActive WHERE Id = @Id", 
                            new SqlParameter[] { 
                                new SqlParameter("@IsActive", newStatus),
                                new SqlParameter("@Id", id)
                            });
                        
                        string searchTerm = txtSearch != null ? txtSearch.Text.Trim() : "";
                        string categoryId = ddlCategoryFilter != null ? ddlCategoryFilter.SelectedValue : "";
                        string status = ddlStatusFilter != null ? ddlStatusFilter.SelectedValue : "";
                        string priceMin = txtPriceMin != null ? txtPriceMin.Text.Trim() : "";
                        string priceMax = txtPriceMax != null ? txtPriceMax.Text.Trim() : "";
                        LoadProducts(searchTerm, categoryId, status, priceMin, priceMax);
                    }
                }
                catch (Exception ex)
                {
                    // Gérer l'erreur si nécessaire
                }
            }
        }

        private void LoadProductForEdit(string id)
        {
            DbContext db = new DbContext();
            DataTable dt = db.ExecuteQuery("SELECT * FROM Products WHERE Id = " + id);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                hfProductId.Value = row["Id"].ToString();
                txtName.Text = row["Name"].ToString();
                txtDescription.Text = row["Description"].ToString();
                txtPrice.Text = Convert.ToDecimal(row["Price"]).ToString("N2");
                txtStock.Text = row["StockQuantity"].ToString();
                ddlCategories.SelectedValue = row["CategoryId"].ToString();
                
                lblTitle.Text = "<i class=\"fas fa-edit\"></i> Modifier le Produit";
                pnlList.Visible = false;
                btnAddNew.Visible = false;
                pnlEdit.Visible = true;
                
                // Charger les images du produit
                LoadProductImages(id);
            }
        }

        private void LoadProductImages(string productId)
        {
            try
            {
                DbContext db = new DbContext();
                string query = "SELECT * FROM ProductImages WHERE ProductId = @ProductId ORDER BY IsPrimary DESC, DisplayOrder ASC";
                DataTable dt = db.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@ProductId", productId) });
                
                if (dt.Rows.Count > 0)
                {
                    rptProductImages.DataSource = dt;
                    rptProductImages.DataBind();
                    imagesGallery.Visible = true;
                    lblNoImages.Visible = false;
                }
                else
                {
                    imagesGallery.Visible = false;
                    lblNoImages.Visible = true;
                }
            }
            catch
            {
                imagesGallery.Visible = false;
                lblNoImages.Visible = true;
            }
        }

        protected void rptProductImages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string productId = hfProductId.Value;
            if (string.IsNullOrEmpty(productId))
            {
                return;
            }

            string imageId = e.CommandArgument.ToString();
            DbContext db = new DbContext();

            if (e.CommandName == "SetPrimary")
            {
                // Désactiver toutes les images principales
                db.ExecuteNonQuery(
                    "UPDATE ProductImages SET IsPrimary = 0 WHERE ProductId = @ProductId",
                    new SqlParameter[] { new SqlParameter("@ProductId", productId) });

                // Définir cette image comme principale
                DataTable dt = db.ExecuteQuery(
                    "SELECT ImageUrl FROM ProductImages WHERE Id = @ImageId",
                    new SqlParameter[] { new SqlParameter("@ImageId", imageId) });

                if (dt.Rows.Count > 0)
                {
                    string imageUrl = dt.Rows[0]["ImageUrl"].ToString();
                    
                    db.ExecuteNonQuery(
                        "UPDATE ProductImages SET IsPrimary = 1 WHERE Id = @ImageId",
                        new SqlParameter[] { new SqlParameter("@ImageId", imageId) });

                    // Mettre à jour l'ImageUrl du produit
                    db.ExecuteNonQuery(
                        "UPDATE Products SET ImageUrl = @ImageUrl WHERE Id = @ProductId",
                        new SqlParameter[] {
                            new SqlParameter("@ImageUrl", imageUrl),
                            new SqlParameter("@ProductId", productId)
                        });
                }
            }
            else if (e.CommandName == "DeleteImage")
            {
                // Récupérer l'URL de l'image pour la supprimer du serveur
                DataTable dt = db.ExecuteQuery(
                    "SELECT ImageUrl, IsPrimary FROM ProductImages WHERE Id = @ImageId",
                    new SqlParameter[] { new SqlParameter("@ImageId", imageId) });

                if (dt.Rows.Count > 0)
                {
                    bool wasPrimary = Convert.ToBoolean(dt.Rows[0]["IsPrimary"]);
                    string imageUrl = dt.Rows[0]["ImageUrl"].ToString();

                    // Supprimer l'image de la base de données
                    db.ExecuteNonQuery(
                        "DELETE FROM ProductImages WHERE Id = @ImageId",
                        new SqlParameter[] { new SqlParameter("@ImageId", imageId) });

                    // Supprimer le fichier du serveur
                    try
                    {
                        string filePath = Server.MapPath("~/Assets/Images/Products/" + imageUrl);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                    catch { }

                    // Si c'était l'image principale, définir la première image restante comme principale
                    if (wasPrimary)
                    {
                        DataTable remainingImages = db.ExecuteQuery(
                            "SELECT TOP 1 Id, ImageUrl FROM ProductImages WHERE ProductId = @ProductId ORDER BY DisplayOrder ASC",
                            new SqlParameter[] { new SqlParameter("@ProductId", productId) });

                        if (remainingImages.Rows.Count > 0)
                        {
                            string newPrimaryId = remainingImages.Rows[0]["Id"].ToString();
                            string newPrimaryUrl = remainingImages.Rows[0]["ImageUrl"].ToString();

                            db.ExecuteNonQuery(
                                "UPDATE ProductImages SET IsPrimary = 1 WHERE Id = @ImageId",
                                new SqlParameter[] { new SqlParameter("@ImageId", newPrimaryId) });

                            db.ExecuteNonQuery(
                                "UPDATE Products SET ImageUrl = @ImageUrl WHERE Id = @ProductId",
                                new SqlParameter[] {
                                    new SqlParameter("@ImageUrl", newPrimaryUrl),
                                    new SqlParameter("@ProductId", productId)
                                });
                        }
                    }
                }
            }

            // Recharger les images
            LoadProductImages(productId);
        }

        public string BuildImageUrl(object imageUrl)
        {
            try
            {
                if (imageUrl == null || imageUrl == DBNull.Value || string.IsNullOrEmpty(imageUrl.ToString()))
                {
                    return ResolveUrl("~/Assets/Images/placeholder.svg");
                }
                
                string url = imageUrl.ToString().Trim();
                if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return url;
                }
                
                // Construire l'URL relative
                if (!url.StartsWith("/"))
                {
                    if (string.IsNullOrWhiteSpace(url)) 
                    {
                         return ResolveUrl("~/Assets/Images/placeholder.svg");
                    }
                    url = "~/Assets/Images/Products/" + url;
                    return ResolveUrl(url);
                }
                
                return url;
            }
            catch
            {
                return ResolveUrl("~/Assets/Images/placeholder.svg");
            }
        }

        // Méthode helper pour obtenir la classe CSS du statut
        protected string GetStatusClass(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "active" : "inactive";
        }

        // Méthode helper pour obtenir le texte du statut
        protected string GetStatusText(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "Actif" : "Inactif";
        }

        // Méthode helper pour obtenir la classe CSS du bouton toggle
        protected string GetToggleButtonClass(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "action-btn toggle" : "action-btn activate";
        }

        // Méthode helper pour obtenir le message de confirmation
        protected string GetToggleConfirmMessage(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "return confirm(\"Êtes-vous sûr de vouloir désactiver ce produit ?\");" : "return confirm(\"Êtes-vous sûr de vouloir activer ce produit ?\");";
        }

        // Méthode helper pour obtenir l'icône du bouton toggle
        protected string GetToggleIcon(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "fas fa-ban" : "fas fa-check";
        }

        // Méthode helper pour obtenir le texte du bouton toggle
        protected string GetToggleButtonText(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "Désactiver" : "Activer";
        }
    }
}
