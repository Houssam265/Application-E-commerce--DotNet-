using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Ecommerce.Utils;

namespace Ecommerce
{
    public partial class SiteMaster : MasterPage
    {
        protected global::System.Web.UI.WebControls.ContentPlaceHolder HeadContent;
        protected global::System.Web.UI.WebControls.ContentPlaceHolder MainContent;
        protected global::System.Web.UI.WebControls.LinkButton LogoutBtn;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Garantir l'encodage UTF-8 pour l'affichage des accents
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Charset = "utf-8";

            // Update cart count in session if not set
            if (Session["CartCount"] == null)
            {
                Session["CartCount"] = CartHelper.GetCartItemCount();
            }

            // Merge cart on login
            if (Session["UserId"] != null && Session["CartMerged"] == null)
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                CartHelper.MergeCartOnLogin(userId);
                Session["CartMerged"] = true;
            }

            if (Page != null && Page.Header != null)
            {
                bool hasChatCss = false;
                foreach (Control c in Page.Header.Controls)
                {
                    var link = c as HtmlLink;
                    if (link != null && !string.IsNullOrEmpty(link.Href) && link.Href.IndexOf("/Assets/Styles/chatbot.css", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        hasChatCss = true;
                        break;
                    }
                }
                if (!hasChatCss)
                {
                    var css = new HtmlLink();
                    css.Href = ResolveUrl("~/Assets/Styles/chatbot.css");
                    css.Attributes["rel"] = "stylesheet";
                    Page.Header.Controls.Add(css);
                }
            }

            if (Page != null && !Page.ClientScript.IsClientScriptIncludeRegistered("chatbot-js-include"))
            {
                Page.ClientScript.RegisterClientScriptInclude("chatbot-js-include", ResolveUrl("~/Assets/Js/chatbot.js"));
            }
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("/");
        }
    }
}
