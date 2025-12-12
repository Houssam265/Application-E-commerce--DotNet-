using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Ecommerce
{
    public partial class SiteMaster : MasterPage
    {
        // Controls
        protected global::System.Web.UI.WebControls.ContentPlaceHolder HeadContent;
        protected global::System.Web.UI.WebControls.ContentPlaceHolder MainContent;
        protected global::System.Web.UI.WebControls.LinkButton LogoutBtn;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("/");
        }
    }
}
