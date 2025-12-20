using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;

namespace Ecommerce.Pages.Admin
{
    public partial class Categories : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.LinkButton btnAddNew;
        protected global::System.Web.UI.WebControls.Panel pnlList;
        protected global::System.Web.UI.WebControls.GridView gvCategories;
        protected global::System.Web.UI.WebControls.Panel pnlEdit;
        protected global::System.Web.UI.WebControls.Label lblTitle;
        protected global::System.Web.UI.WebControls.HiddenField hfCategoryId;
        protected global::System.Web.UI.WebControls.TextBox txtName;
        protected global::System.Web.UI.WebControls.TextBox txtDescription;
        protected global::System.Web.UI.WebControls.TextBox txtDisplayOrder;
        protected global::System.Web.UI.WebControls.DropDownList ddlIsActive;
        protected global::System.Web.UI.WebControls.FileUpload fuImage;
        protected global::System.Web.UI.WebControls.TextBox txtImageUrl;
        protected global::System.Web.UI.WebControls.LinkButton btnSave;
        protected global::System.Web.UI.WebControls.LinkButton btnCancel;
        protected global::System.Web.UI.WebControls.Label lblError;
        protected global::System.Web.UI.WebControls.Label lblSuccess;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvName;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();
            }
        }

        private void LoadCategories()
        {
            try
            {
                DbContext db = new DbContext();
                DataTable dt = db.ExecuteQuery("SELECT * FROM Categories ORDER BY DisplayOrder ASC, Name ASC");
                gvCategories.DataSource = dt;
                gvCategories.DataBind();
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur lors du chargement: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            pnlList.Visible = false;
            pnlEdit.Visible = true;
            btnAddNew.Visible = false;
            
            hfCategoryId.Value = "";
            txtName.Text = "";
            txtDescription.Text = "";
            txtDisplayOrder.Text = "0";
            ddlIsActive.SelectedValue = "1";
            txtImageUrl.Text = "";
            lblTitle.Text = "<i class=\"fas fa-plus\"></i> Ajouter une Catégorie";
            lblError.Visible = false;
            lblSuccess.Visible = false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlEdit.Visible = false;
            pnlList.Visible = true;
            btnAddNew.Visible = true;
            lblError.Visible = false;
            lblSuccess.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                string name = txtName.Text.Trim();
                string description = txtDescription.Text.Trim();
                int displayOrder = 0;
                int.TryParse(txtDisplayOrder.Text, out displayOrder);
                bool isActive = ddlIsActive.SelectedValue == "1";
                string imageUrl = txtImageUrl.Text.Trim();

                // Gérer l'upload d'image
                if (fuImage.HasFile)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(fuImage.FileName);
                    string path = Server.MapPath("~/Assets/Images/Categories/");
                    
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    fuImage.SaveAs(path + filename);
                    imageUrl = "/Assets/Images/Categories/" + filename;
                }

                DbContext db = new DbContext();

                if (string.IsNullOrEmpty(hfCategoryId.Value))
                {
                    // Insertion
                    string query = @"INSERT INTO Categories (Name, Description, ImageUrl, IsActive, DisplayOrder, CreatedAt) 
                                     VALUES (@Name, @Desc, @Img, @IsActive, @DisplayOrder, GETDATE())";
                    
                    SqlParameter[] parameters = {
                        new SqlParameter("@Name", name),
                        new SqlParameter("@Desc", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description),
                        new SqlParameter("@Img", string.IsNullOrEmpty(imageUrl) ? (object)DBNull.Value : imageUrl),
                        new SqlParameter("@IsActive", isActive),
                        new SqlParameter("@DisplayOrder", displayOrder)
                    };
                    
                    db.ExecuteNonQuery(query, parameters);
                    lblSuccess.Text = "Catégorie ajoutée avec succès!";
                    lblSuccess.Visible = true;
                }
                else
                {
                    // Mise à jour
                    string id = hfCategoryId.Value;
                    string query;
                    System.Collections.Generic.List<SqlParameter> parameters = new System.Collections.Generic.List<SqlParameter>();

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        query = @"UPDATE Categories SET Name=@Name, Description=@Desc, ImageUrl=@Img, 
                                 IsActive=@IsActive, DisplayOrder=@DisplayOrder WHERE Id=@Id";
                        parameters.Add(new SqlParameter("@Img", imageUrl));
                    }
                    else
                    {
                        query = @"UPDATE Categories SET Name=@Name, Description=@Desc, 
                                 IsActive=@IsActive, DisplayOrder=@DisplayOrder WHERE Id=@Id";
                    }

                    parameters.Add(new SqlParameter("@Name", name));
                    parameters.Add(new SqlParameter("@Desc", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description));
                    parameters.Add(new SqlParameter("@IsActive", isActive));
                    parameters.Add(new SqlParameter("@DisplayOrder", displayOrder));
                    parameters.Add(new SqlParameter("@Id", id));

                    db.ExecuteNonQuery(query, parameters.ToArray());
                    lblSuccess.Text = "Catégorie mise à jour avec succès!";
                    lblSuccess.Visible = true;
                }

                // Recharger la liste après un court délai
                System.Threading.Thread.Sleep(500);
                LoadCategories();
                
                pnlEdit.Visible = false;
                pnlList.Visible = true;
                btnAddNew.Visible = true;
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void gvCategories_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditCat")
            {
                string id = e.CommandArgument.ToString();
                LoadCategoryForEdit(id);
            }
            else if (e.CommandName == "DeleteCat")
            {
                string id = e.CommandArgument.ToString();
                try
                {
                    DbContext db = new DbContext();
                    
                    // Vérifier si la catégorie est utilisée par des produits
                    object productCount = db.ExecuteScalar("SELECT COUNT(*) FROM Products WHERE CategoryId = @CatId", 
                        new SqlParameter[] { new SqlParameter("@CatId", id) });
                    
                    if (Convert.ToInt32(productCount) > 0)
                    {
                        lblError.Text = "Impossible de supprimer cette catégorie car elle est utilisée par " + productCount + " produit(s).";
                        lblError.Visible = true;
                        return;
                    }

                    // Supprimer la catégorie
                    db.ExecuteNonQuery("DELETE FROM Categories WHERE Id = @Id", 
                        new SqlParameter[] { new SqlParameter("@Id", id) });
                    
                    lblSuccess.Text = "Catégorie supprimée avec succès!";
                    lblSuccess.Visible = true;
                    LoadCategories();
                }
                catch (Exception ex)
                {
                    lblError.Text = "Erreur lors de la suppression: " + ex.Message;
                    lblError.Visible = true;
                }
            }
        }

        private void LoadCategoryForEdit(string id)
        {
            try
            {
                DbContext db = new DbContext();
                DataTable dt = db.ExecuteQuery("SELECT * FROM Categories WHERE Id = @Id", 
                    new SqlParameter[] { new SqlParameter("@Id", id) });
                
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    hfCategoryId.Value = row["Id"].ToString();
                    txtName.Text = row["Name"].ToString();
                    txtDescription.Text = row["Description"] != DBNull.Value ? row["Description"].ToString() : "";
                    txtDisplayOrder.Text = row["DisplayOrder"] != DBNull.Value ? row["DisplayOrder"].ToString() : "0";
                    ddlIsActive.SelectedValue = Convert.ToBoolean(row["IsActive"]) ? "1" : "0";
                    
                    if (row["ImageUrl"] != DBNull.Value && !string.IsNullOrEmpty(row["ImageUrl"].ToString()))
                    {
                        txtImageUrl.Text = row["ImageUrl"].ToString();
                    }
                    
                    lblTitle.Text = "<i class=\"fas fa-edit\"></i> Modifier la Catégorie";
                    pnlList.Visible = false;
                    btnAddNew.Visible = false;
                    pnlEdit.Visible = true;
                    lblError.Visible = false;
                    lblSuccess.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur lors du chargement: " + ex.Message;
                lblError.Visible = true;
            }
        }

        // Méthode helper pour obtenir l'URL de l'image de la catégorie
        protected string GetCategoryImageUrl(object imageUrl)
        {
            if (imageUrl == null || imageUrl == DBNull.Value || string.IsNullOrEmpty(imageUrl.ToString()))
            {
                return "https://via.placeholder.com/60x60/e2e8f0/94a3b8?text=No+Image";
            }
            return imageUrl.ToString();
        }

        // Méthode helper pour obtenir la description de la catégorie (tronquée si nécessaire)
        protected string GetCategoryDescription(object description)
        {
            if (description == null || description == DBNull.Value || string.IsNullOrEmpty(description.ToString()))
            {
                return "-";
            }
            
            string desc = description.ToString();
            if (desc.Length > 50)
            {
                return desc.Substring(0, 50) + "...";
            }
            return desc;
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
    }
}

