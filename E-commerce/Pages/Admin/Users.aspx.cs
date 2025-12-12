using System;
using System.Data;
using System.Web.UI;
using Ecommerce.Data;

namespace Ecommerce.Pages.Admin
{
    public partial class Users : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.GridView gvUsers;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            DbContext db = new DbContext();
            DataTable dt = db.ExecuteQuery("SELECT * FROM Users ORDER BY CreatedAt DESC");
            gvUsers.DataSource = dt;
            gvUsers.DataBind();
        }
    }
}
