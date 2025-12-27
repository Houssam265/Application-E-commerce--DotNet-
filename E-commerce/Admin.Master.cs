using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

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
    }
}
