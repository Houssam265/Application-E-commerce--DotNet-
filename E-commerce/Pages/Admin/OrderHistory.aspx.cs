using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;

namespace Ecommerce.Pages.Admin
{
    public partial class OrderHistory : Page
    {
        protected global::System.Web.UI.WebControls.DropDownList ddlStatusFilter;
        protected global::System.Web.UI.WebControls.TextBox txtSearch;
        protected global::System.Web.UI.WebControls.Button btnSearch;
        protected global::System.Web.UI.WebControls.Label lblTotalArchived;
        protected global::System.Web.UI.WebControls.Repeater rptOrders;
        protected global::System.Web.UI.WebControls.Panel pnlNoOrders;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Admin")
            {
                Response.Redirect("~/Pages/Public/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadOrders();
            }
        }

        private void LoadOrders()
        {
            DbContext db = new DbContext();

            // Get orders from OrderHistory (archived orders)
            string query = @"
                SELECT 
                    OH.Id, OH.OrderId, OH.OrderNumber, OH.UserId, OH.OrderDate, 
                    OH.CompletedDate, OH.TotalAmount, OH.Status, OH.Notes,
                    U.FullName, U.Email
                FROM OrderHistory OH
                INNER JOIN Users U ON OH.UserId = U.Id
                WHERE 1=1";

            System.Collections.Generic.List<SqlParameter> parameters = new System.Collections.Generic.List<SqlParameter>();

            // Apply filters
            if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
            {
                query += " AND OH.Status = @Status";
                parameters.Add(new SqlParameter("@Status", ddlStatusFilter.SelectedValue));
            }

            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                query += " AND (OH.OrderNumber LIKE @Search OR U.FullName LIKE @Search OR U.Email LIKE @Search)";
                parameters.Add(new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%"));
            }

            query += " ORDER BY OH.CompletedDate DESC";

            DataTable dt = db.ExecuteQuery(query, parameters.ToArray());

            if (dt.Rows.Count > 0)
            {
                rptOrders.DataSource = dt;
                rptOrders.DataBind();
                pnlNoOrders.Visible = false;
                lblTotalArchived.Text = $"{dt.Rows.Count} commande(s) archivée(s)";
            }
            else
            {
                pnlNoOrders.Visible = true;
                lblTotalArchived.Text = "0 commande archivée";
            }
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadOrders();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        protected string GetStatusLabel(string status)
        {
            switch (status)
            {
                case "Delivered": return "Livrée";
                case "Cancelled": return "Annulée";
                default: return status;
            }
        }

        protected void rptOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DownloadInvoice")
            {
                int orderId;
                if (int.TryParse(e.CommandArgument.ToString(), out orderId))
                {
                    // Redirect to invoice download page
                    Response.Redirect($"~/Pages/Public/DownloadInvoice.aspx?id={orderId}&format=pdf");
                }
            }
        }

        protected void rptOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int orderId = Convert.ToInt32(row["OrderId"]);
                
                // Load review for this order
                Panel pnlReview = (Panel)e.Item.FindControl("pnlReview");
                System.Web.UI.HtmlControls.HtmlGenericControl reviewStars = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("reviewStars");
                System.Web.UI.HtmlControls.HtmlGenericControl reviewComment = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("reviewComment");
                System.Web.UI.HtmlControls.HtmlGenericControl reviewDate = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("reviewDate");
                
                if (pnlReview != null)
                {
                    LoadOrderReview(orderId, pnlReview, reviewStars, reviewComment, reviewDate);
                }
            }
        }

        private void LoadOrderReview(int orderId, Panel pnlReview, System.Web.UI.HtmlControls.HtmlGenericControl reviewStars, 
            System.Web.UI.HtmlControls.HtmlGenericControl reviewComment, System.Web.UI.HtmlControls.HtmlGenericControl reviewDate)
        {
            try
            {
                DbContext db = new DbContext();
                DataTable dt = db.ExecuteQuery(
                    @"SELECT TOP 1 Rating, Comment, ReviewDate 
                      FROM Reviews 
                      WHERE OrderId = @OrderId AND ProductId IS NULL 
                      ORDER BY ReviewDate DESC",
                    new SqlParameter[] { new SqlParameter("@OrderId", orderId) });
                
                if (dt.Rows.Count > 0)
                {
                    int rating = Convert.ToInt32(dt.Rows[0]["Rating"]);
                    string comment = dt.Rows[0]["Comment"] != DBNull.Value ? dt.Rows[0]["Comment"].ToString() : "";
                    DateTime reviewDateValue = Convert.ToDateTime(dt.Rows[0]["ReviewDate"]);
                    
                    reviewStars.InnerHtml = RenderStars(rating);
                    reviewComment.InnerHtml = Server.HtmlEncode(comment);
                    reviewDate.InnerText = "Le " + reviewDateValue.ToString("dd/MM/yyyy à HH:mm");
                    pnlReview.Visible = true;
                }
                else
                {
                    pnlReview.Visible = false;
                }
            }
            catch
            {
                pnlReview.Visible = false;
            }
        }

        private string RenderStars(int rating)
        {
            rating = Math.Max(1, Math.Min(5, rating));
            string stars = "";
            for (int i = 0; i < 5; i++)
            {
                if (i < rating) stars += "<i class='fas fa-star' style='color:#f59e0b;'></i>";
                else stars += "<i class='far fa-star' style='color:#f59e0b;'></i>";
            }
            return "<div style='font-size:1.2rem; margin:5px 0;'>" + stars + "</div>";
        }
    }
}
