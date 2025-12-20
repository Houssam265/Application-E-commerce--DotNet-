using System;
using System.Data;
using System.Web.UI;
using Ecommerce.Data;

namespace Ecommerce
{
    public partial class Default : Page
    {
        protected global::System.Web.UI.WebControls.Repeater rptCategories;
        protected global::System.Web.UI.WebControls.Repeater rptFeaturedProducts;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();
                LoadFeaturedProducts();
            }
        }

        private void LoadCategories()
        {
            try
            {
                DbContext db = new DbContext();
                string query = "SELECT TOP 6 * FROM Categories WHERE IsActive = 1 ORDER BY DisplayOrder, Name";
                DataTable dt = db.ExecuteQuery(query);
                
                rptCategories.DataSource = dt;
                rptCategories.DataBind();
            }
            catch
            {
                // Silent fail
            }
        }

        private void LoadFeaturedProducts()
        {
            try
            {
                DbContext db = new DbContext();
                string query = @"SELECT TOP 8 p.*, c.Name as CooperativeName 
                                 FROM Products p
                                 LEFT JOIN Cooperatives c ON p.CooperativeId = c.Id
                                 WHERE p.IsActive = 1 AND p.IsFeatured = 1
                                 ORDER BY p.CreatedAt DESC";
                DataTable dt = db.ExecuteQuery(query);
                
                rptFeaturedProducts.DataSource = dt;
                rptFeaturedProducts.DataBind();
            }
            catch
            {
                // Silent fail
            }
        }

        protected string GetFeaturedBadge(object isFeatured)
        {
            if (isFeatured != null && isFeatured != DBNull.Value)
            {
                bool featured = Convert.ToBoolean(isFeatured);
                if (featured)
                {
                    return "<span class='product-badge'>Nouveau</span>";
                }
            }
            return "";
        }
    }
}
