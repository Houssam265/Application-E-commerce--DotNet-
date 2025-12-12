using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Ecommerce.Data;

namespace Ecommerce.Pages.Public
{
    public partial class Shop : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.Repeater rptCategories;
        protected global::System.Web.UI.WebControls.Repeater rptProducts;
        protected global::System.Web.UI.WebControls.TextBox txtSearch;
        protected global::System.Web.UI.WebControls.Button btnSearch;
        protected global::System.Web.UI.WebControls.Label lblNoResults;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();
                LoadProducts();
            }
        }

        private void LoadCategories()
        {
            try
            {
                DbContext db = new DbContext();
                DataTable dt = db.ExecuteQuery("SELECT * FROM Categories WHERE IsActive = 1");
                rptCategories.DataSource = dt;
                rptCategories.DataBind();
            }
            catch { }
        }

        private void LoadProducts()
        {
            try
            {
                string catId = Request.QueryString["cat"];
                string search = txtSearch.Text.Trim();
                
                string query = "SELECT * FROM Products WHERE IsActive = 1";
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(catId))
                {
                    query += " AND CategoryId = @CatId";
                    parameters.Add(new SqlParameter("@CatId", catId));
                }

                if (!string.IsNullOrEmpty(search))
                {
                    query += " AND (Name LIKE @Search OR Description LIKE @Search)";
                    parameters.Add(new SqlParameter("@Search", "%" + search + "%"));
                }

                query += " ORDER BY CreatedAt DESC";

                DbContext db = new DbContext();
                DataTable dt = db.ExecuteQuery(query, parameters.ToArray());

                if (dt.Rows.Count > 0)
                {
                    rptProducts.DataSource = dt;
                    rptProducts.DataBind();
                    lblNoResults.Visible = false;
                }
                else
                {
                    rptProducts.DataSource = null;
                    rptProducts.DataBind();
                    lblNoResults.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblNoResults.Text = "Erreur de chargement: " + ex.Message;
                lblNoResults.Visible = true;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        protected string IsActiveCategory(object id)
        {
            string currentCat = Request.QueryString["cat"];
            if (currentCat != null && id.ToString() == currentCat)
            {
                return "active";
            }
            return "";
        }
    }
}
