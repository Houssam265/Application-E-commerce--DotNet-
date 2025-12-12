using System;
using System.Web.UI;

namespace Ecommerce
{
    public partial class AdminMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Security Check
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
               Response.Redirect("~/Pages/Public/Login.aspx");
            }
        }
    }
}
