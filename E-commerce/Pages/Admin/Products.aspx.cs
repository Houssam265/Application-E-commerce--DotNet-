using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;

namespace Ecommerce.Pages.Admin
{
    public partial class Products : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.Button btnAddNew;
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
        protected global::System.Web.UI.WebControls.FileUpload fuImage;
        protected global::System.Web.UI.WebControls.Button btnSave;
        protected global::System.Web.UI.WebControls.Button btnCancel;
        protected global::System.Web.UI.WebControls.Label lblError;

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
            lblTitle.Text = "Nouveau Produit";
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
                string imageUrl = "";

                if (fuImage.HasFile)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(fuImage.FileName);
                    string path = Server.MapPath("~/Assets/Images/Products/") + filename;
                    if (!Directory.Exists(Server.MapPath("~/Assets/Images/Products/")))
                        Directory.CreateDirectory(Server.MapPath("~/Assets/Images/Products/"));

                    fuImage.SaveAs(path);
                    imageUrl = filename;
                }

                DbContext db = new DbContext();

                if (string.IsNullOrEmpty(hfProductId.Value))
                {
                    if (string.IsNullOrEmpty(imageUrl)) imageUrl = "placeholder.jpg";

                    string query = "INSERT INTO Products (CategoryId, Name, Description, Price, StockQuantity, ImageUrl) VALUES (@CatId, @Name, @Desc, @Price, @Stock, @Img)";
                    SqlParameter[] p = {
                        new SqlParameter("@CatId", catId),
                        new SqlParameter("@Name", name),
                        new SqlParameter("@Desc", desc),
                        new SqlParameter("@Price", price),
                        new SqlParameter("@Stock", stock),
                        new SqlParameter("@Img", imageUrl)
                    };
                    db.ExecuteNonQuery(query, p);
                }
                else
                {
                    string id = hfProductId.Value;
                    string query = "UPDATE Products SET CategoryId=@CatId, Name=@Name, Description=@Desc, Price=@Price, StockQuantity=@Stock WHERE Id=@Id";
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        query = "UPDATE Products SET CategoryId=@CatId, Name=@Name, Description=@Desc, Price=@Price, StockQuantity=@Stock, ImageUrl=@Img WHERE Id=@Id";
                    }
                    
                    System.Collections.Generic.List<SqlParameter> p = new System.Collections.Generic.List<SqlParameter> {
                        new SqlParameter("@CatId", catId),
                        new SqlParameter("@Name", name),
                        new SqlParameter("@Desc", desc),
                        new SqlParameter("@Price", price),
                        new SqlParameter("@Stock", stock),
                        new SqlParameter("@Id", id)
                    };
                    if (!string.IsNullOrEmpty(imageUrl)) p.Add(new SqlParameter("@Img", imageUrl));

                    db.ExecuteNonQuery(query, p.ToArray());
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
                
                lblTitle.Text = "Modifier Produit";
                pnlList.Visible = false;
                btnAddNew.Visible = false;
                pnlEdit.Visible = true;
            }
        }
    }
}
