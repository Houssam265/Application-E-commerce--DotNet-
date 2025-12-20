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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProducts();
                LoadCategories();
            }
        }

        private void LoadProducts()
        {
            DbContext db = new DbContext();
            DataTable dt = db.ExecuteQuery("SELECT * FROM Products WHERE IsActive = 1 ORDER BY CreatedAt DESC");
            gvProducts.DataSource = dt;
            gvProducts.DataBind();
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
            txtPrice.Text = "";
            txtStock.Text = "";
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
                LoadProducts(); 
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
            else if (e.CommandName == "DeleteProd")
            {
                string id = e.CommandArgument.ToString();
                DbContext db = new DbContext();
                db.ExecuteNonQuery("UPDATE Products SET IsActive = 0 WHERE Id = " + id);
                LoadProducts();
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
                    return "https://via.placeholder.com/150/e2e8f0/94a3b8?text=No+Image";
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
                    url = "/Assets/Images/Products/" + url;
                }
                
                return url;
            }
            catch
            {
                return "https://via.placeholder.com/150/e2e8f0/94a3b8?text=No+Image";
            }
        }
    }
}
