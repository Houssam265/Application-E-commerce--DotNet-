using System;
using System.Data;
using System.Web.UI;
using Ecommerce.Data;

namespace Ecommerce
{
    public partial class Default : Page
    {
        // Controls declaration
        protected global::System.Web.UI.WebControls.Repeater rptFeaturedProducts;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFeaturedProducts();
            }
        }

        private void LoadFeaturedProducts()
        {
            try
            {
                DbContext db = new DbContext();
                string query = "SELECT TOP 4 * FROM Products WHERE IsActive = 1 ORDER BY CreatedAt DESC";
                DataTable dt = db.ExecuteQuery(query);
                
                rptFeaturedProducts.DataSource = dt;
                rptFeaturedProducts.DataBind();
            }
            catch
            {
                // Silent fail for now
            }
        }
    }
}
