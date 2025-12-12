using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Ecommerce.Pages.Public
{
    public partial class OrderConfirmation : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.Label lblOrderId;

        protected void Page_Load(object sender, EventArgs e)
        {
             if (Request.QueryString["id"] != null)
            {
                lblOrderId.Text = Request.QueryString["id"];
            }
        }
    }
}
