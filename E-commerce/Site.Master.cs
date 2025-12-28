using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Ecommerce.Utils;
using System.Data;
using System.Data.SqlClient;
using Ecommerce.Data;

namespace Ecommerce
{
    public partial class SiteMaster : MasterPage
    {
        protected global::System.Web.UI.WebControls.ContentPlaceHolder HeadContent;
        protected global::System.Web.UI.WebControls.ContentPlaceHolder MainContent;
        protected global::System.Web.UI.WebControls.LinkButton LogoutBtn;
        protected global::System.Web.UI.WebControls.Literal litNotifCount;
        protected global::System.Web.UI.WebControls.Repeater rptNotifications;
        protected global::System.Web.UI.WebControls.Label lblNoNotifications;
        protected global::System.Web.UI.WebControls.LinkButton btnMarkAllRead;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl notifDot;

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

            if (Session["UserId"] != null)
            {
                LoadNotifications();
            }
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("/");
        }

        private void LoadNotifications()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                DataTable dt = db.ExecuteQuery(
                    @"SELECT TOP 10 Id, Title, Message, Type, IsRead, CreatedAt 
                      FROM Notifications 
                      WHERE UserId = @UserId 
                      ORDER BY CreatedAt DESC",
                    new SqlParameter[] { new SqlParameter("@UserId", userId) });

                object unreadObj = db.ExecuteScalar(
                    "SELECT COUNT(*) FROM Notifications WHERE UserId = @UserId AND IsRead = 0",
                    new SqlParameter[] { new SqlParameter("@UserId", userId) });
                int unread = Convert.ToInt32(unreadObj);
                litNotifCount.Text = unread > 0 ? "<span class='badge badge-primary' style='margin-left:6px;'>" + unread + "</span>" : "";
                if (notifDot != null)
                {
                    notifDot.Style["display"] = unread > 0 ? "block" : "none";
                    notifDot.Style["background"] = unread > 0 ? "#ef4444" : "transparent";
                }

                if (dt.Rows.Count > 0)
                {
                    rptNotifications.DataSource = dt;
                    rptNotifications.ItemDataBound += rptNotifications_ItemDataBound;
                    rptNotifications.DataBind();
                    lblNoNotifications.Visible = false;
                }
                else
                {
                    lblNoNotifications.Visible = true;
                }
            }
            catch
            {
                lblNoNotifications.Visible = true;
            }
        }

        protected void rptNotifications_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "MarkRead")
            {
                try
                {
                    int id = Convert.ToInt32(e.CommandArgument);
                    DbContext db = new DbContext();
                    db.ExecuteNonQuery("UPDATE Notifications SET IsRead = 1 WHERE Id = @Id",
                        new SqlParameter[] { new SqlParameter("@Id", id) });
                    LoadNotifications();
                }
                catch { }
            }
        }

        protected void MarkAllRead_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                db.ExecuteNonQuery("UPDATE Notifications SET IsRead = 1 WHERE UserId = @UserId",
                    new SqlParameter[] { new SqlParameter("@UserId", userId) });
                LoadNotifications();
            }
            catch { }
        }

        protected void rptNotifications_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = e.Item.DataItem as DataRowView;
                if (row == null) return;
                string type = row["Type"].ToString();
                string message = row["Message"].ToString();
                bool isRead = row["IsRead"] != DBNull.Value && Convert.ToBoolean(row["IsRead"]);
                HtmlGenericControl container = e.Item.FindControl("notifItem") as HtmlGenericControl;
                if (container != null)
                {
                    container.Attributes["style"] += isRead 
                        ? "; background:#ffffff;" 
                        : "; background:#f0f9ff; border-left:4px solid #38bdf8;";
                }
                HyperLink link = e.Item.FindControl("lnkNotifAction") as HyperLink;
                if (link != null)
                {
                    if (type == "Order")
                    {
                        string orderId = ExtractValue(message, "OrderId");
                        string status = ExtractValue(message, "Status");
                        link.Text = "Voir la commande (" + status + ")";
                        link.NavigateUrl = ResolveUrl("~/Pages/Public/OrderDetails.aspx?id=" + orderId);
                    }
                    else if (type == "Product")
                    {
                        string productId = ExtractValue(message, "ProductId");
                        link.Text = "Voir le produit";
                        link.NavigateUrl = ResolveUrl("~/Pages/Public/ProductDetails.aspx?id=" + productId);
                    }
                    else
                    {
                        link.Text = "Voir";
                        link.NavigateUrl = ResolveUrl("~/");
                    }
                }
                LinkButton markBtn = e.Item.FindControl("btnMarkRead") as LinkButton;
                if (markBtn != null)
                {
                    markBtn.Text = isRead ? "Lu" : "Marquer comme lu";
                    markBtn.Style["color"] = isRead ? "#64748b" : "#2563eb";
                }
            }
        }

        private string ExtractValue(string message, string key)
        {
            try
            {
                string[] parts = message.Split(';');
                foreach (var part in parts)
                {
                    var kv = part.Split('=');
                    if (kv.Length == 2 && kv[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        return kv[1].Trim();
                    }
                }
                return "";
            }
            catch { return ""; }
        }
    }
}
