using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;
using Ecommerce.Utils;

namespace Ecommerce.Pages.Admin
{
    public partial class Orders : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.Panel pnlList;
        protected global::System.Web.UI.WebControls.GridView gvOrders;
        protected global::System.Web.UI.WebControls.Panel pnlDetails;
        protected global::System.Web.UI.WebControls.Label lblOrderId;
        protected global::System.Web.UI.WebControls.LinkButton btnClose;
        protected global::System.Web.UI.WebControls.Label lblCustomer;
        protected global::System.Web.UI.WebControls.Label lblEmail;
        protected global::System.Web.UI.WebControls.Label lblAddress;
        protected global::System.Web.UI.WebControls.GridView gvItems;
        protected global::System.Web.UI.WebControls.DropDownList ddlStatus;
        protected global::System.Web.UI.WebControls.LinkButton btnUpdateStatus;
        protected global::System.Web.UI.WebControls.TextBox txtCancelReason;
        protected global::System.Web.UI.WebControls.Label lblStatusError;
        protected global::System.Web.UI.WebControls.Panel pnlReviewAdmin;
        protected global::System.Web.UI.WebControls.Literal litAdminReviewStars;
        protected global::System.Web.UI.WebControls.Literal litAdminReviewText;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl lblAdminReviewDate;
        protected global::System.Web.UI.WebControls.Label lblNoReview;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadOrders();
            }
        }

        private void LoadOrders()
        {
            DbContext db = new DbContext();
            string query = @"
                SELECT O.Id, O.OrderDate, O.TotalAmount, O.Status, U.FullName 
                FROM Orders O 
                INNER JOIN Users U ON O.UserId = U.Id 
                WHERE O.IsArchived = 0 
                AND O.Status NOT IN ('Delivered', 'Cancelled')
                ORDER BY O.OrderDate DESC";
            
            DataTable dt = db.ExecuteQuery(query);
            gvOrders.DataSource = dt;
            gvOrders.DataBind();
        }


        protected void gvOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewOrder")
            {
                string id = e.CommandArgument.ToString();
                LoadOrderDetails(id);
            }
        }

        private void LoadOrderDetails(string id)
        {
            DbContext db = new DbContext();
            
            string query = @"
                SELECT O.*, U.FullName, U.Email, A.Street, A.City, A.ZipCode, A.Country, A.FullName as AddressName
                FROM Orders O 
                INNER JOIN Users U ON O.UserId = U.Id 
                LEFT JOIN Addresses A ON O.ShippingAddressId = A.Id 
                WHERE O.Id = @Id";
            
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };
            DataTable dt = db.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                lblOrderId.Text = row["Id"].ToString();
                lblCustomer.Text = Server.HtmlEncode(row["FullName"].ToString());
                lblEmail.Text = Server.HtmlEncode(row["Email"].ToString());
                
                if (row["Street"] != DBNull.Value)
                {
                    string addressName = row["AddressName"] != DBNull.Value ? row["AddressName"].ToString() + "<br/>" : "";
                    lblAddress.Text = addressName + Server.HtmlEncode(row["Street"].ToString()) + ", " +
                                     Server.HtmlEncode(row["ZipCode"].ToString()) + " " +
                                     Server.HtmlEncode(row["City"].ToString()) + ", " +
                                     Server.HtmlEncode(row["Country"].ToString());
                }
                else
                {
                    lblAddress.Text = "Adresse non disponible";
                }
                
                string currentStatus = row["Status"].ToString();
                ddlStatus.SelectedValue = currentStatus;
                
                // CHECK IF ORDER STATUS IS LOCKED (Delivered or Cancelled)
                bool isStatusLocked = (currentStatus == "Delivered" || currentStatus == "Cancelled");
                
                if (isStatusLocked)
                {
                    // Disable status modification for locked orders
                    ddlStatus.Enabled = false;
                    btnUpdateStatus.Enabled = false;
                    btnUpdateStatus.CssClass = "btn btn-secondary"; // Gray out button
                    txtCancelReason.Enabled = false;
                    
                    // Show informational message
                    lblStatusError.Text = "⚠️ Cette commande est verrouillée et ne peut plus être modifiée (statut final : " + GetStatusLabel(currentStatus) + ").";
                    lblStatusError.CssClass = "alert alert-info";
                    lblStatusError.Visible = true;
                }
                else
                {
                    // Enable controls for non-locked orders
                    ddlStatus.Enabled = true;
                    btnUpdateStatus.Enabled = true;
                    btnUpdateStatus.CssClass = "btn btn-primary";
                    txtCancelReason.Enabled = true;
                    lblStatusError.Visible = false;
                }
                
                if (row["Status"].ToString() == "Cancelled")
                {
                    txtCancelReason.Style["display"] = "block";
                    txtCancelReason.Text = row["Notes"] != DBNull.Value ? row["Notes"].ToString() : "";
                }
                else
                {
                    txtCancelReason.Style["display"] = "none";
                    txtCancelReason.Text = "";
                }

            string itemsQuery = @"
                SELECT OI.ProductName as Name, OI.Quantity, OI.UnitPrice, OI.TotalPrice
                FROM OrderItems OI 
                WHERE OI.OrderId = @OrderId";
                
                SqlParameter[] itemParams = { new SqlParameter("@OrderId", id) };
                DataTable dtItems = db.ExecuteQuery(itemsQuery, itemParams);
                gvItems.DataSource = dtItems;
                gvItems.DataBind();

                pnlList.Visible = false;
                pnlDetails.Visible = true;

                LoadOrderReview(id);
            }
        }


        protected void btnClose_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            pnlList.Visible = true;
        }

        protected void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            string id = lblOrderId.Text;
            string status = ddlStatus.SelectedValue;

            DbContext db = new DbContext();
            lblStatusError.Visible = false;
            
            // CHECK IF ORDER IS ALREADY LOCKED
            string checkLockQuery = "SELECT Status FROM Orders WHERE Id = @Id";
            DataTable lockCheck = db.ExecuteQuery(checkLockQuery, new SqlParameter[] { new SqlParameter("@Id", id) });
            if (lockCheck.Rows.Count > 0)
            {
                string currentStatus = lockCheck.Rows[0]["Status"].ToString();
                if (currentStatus == "Delivered" || currentStatus == "Cancelled")
                {
                    lblStatusError.Text = "❌ Impossible de modifier le statut : cette commande est verrouillée (statut final : " + GetStatusLabel(currentStatus) + ").";
                    lblStatusError.CssClass = "alert alert-danger";
                    lblStatusError.Visible = true;
                    pnlDetails.Visible = true;
                    pnlList.Visible = false;
                    return;
                }
            }
            
            string cancelReason = txtCancelReason.Text.Trim();
            if (status == "Cancelled" && string.IsNullOrWhiteSpace(cancelReason))
            {
                lblStatusError.Text = "Veuillez préciser la raison de l'annulation.";
                lblStatusError.CssClass = "alert alert-warning";
                lblStatusError.Visible = true;
                pnlDetails.Visible = true;
                pnlList.Visible = false;
                return;
            }

            string updateQuery = "UPDATE Orders SET Status = @Status, Notes = @Notes, UpdatedAt = GETDATE() WHERE Id = @Id";
            SqlParameter[] updateParams = {
                new SqlParameter("@Status", status),
                new SqlParameter("@Notes", status == "Cancelled" ? (object)cancelReason : DBNull.Value),
                new SqlParameter("@Id", id)
            };
            db.ExecuteNonQuery(updateQuery, updateParams);

            // GET ORDER AND USER DETAILS
            DataTable odt = db.ExecuteQuery(@"SELECT O.OrderNumber, O.UserId, U.Email, U.FullName 
                                              FROM Orders O INNER JOIN Users U ON O.UserId = U.Id 
                                              WHERE O.Id = @Id", new SqlParameter[] { new SqlParameter("@Id", id) });
            if (odt.Rows.Count > 0)
            {
                var orow = odt.Rows[0];
                int userId = Convert.ToInt32(orow["UserId"]);
                string email = orow["Email"].ToString();
                string fullName = orow["FullName"].ToString();
                string orderNumber = orow["OrderNumber"].ToString();

                // CREATE NOTIFICATION
                string title = "Mise à jour de votre commande " + orderNumber;
                string message = "OrderId=" + id + ";Status=" + status + (status == "Cancelled" ? ";Reason=" + cancelReason : "");
                db.ExecuteNonQuery(@"INSERT INTO Notifications (UserId, Title, Message, Type) 
                                     VALUES (@UserId, @Title, @Message, 'Order')",
                                     new SqlParameter[] {
                                         new SqlParameter("@UserId", userId),
                                         new SqlParameter("@Title", title),
                                         new SqlParameter("@Message", message)
                                     });

                // GET ORDER ITEMS FOR EMAIL
                string itemsQuery = @"SELECT ProductName, Quantity, UnitPrice, TotalPrice 
                                     FROM OrderItems WHERE OrderId = @OrderId";
                DataTable dtProducts = db.ExecuteQuery(itemsQuery, new SqlParameter[] { new SqlParameter("@OrderId", id) });
                
                // GENERATE BEAUTIFUL HTML EMAIL WITH PRODUCTS
                string productsHtml = EmailTemplates.GenerateProductTableHtml(dtProducts, false);
                string subject = "Commande " + orderNumber + " - " + GetStatusLabel(status);
                string body = EmailTemplates.GetOrderStatusEmailTemplate(fullName, orderNumber, status, productsHtml, cancelReason);
                
                SecurityHelper.SendEmail(email, subject, body);

                // ARCHIVE ORDER IF STATUS IS DELIVERED OR CANCELLED
                if (status == "Delivered" || status == "Cancelled")
                {
                    try
                    {
                        // Use stored procedure to archive order
                        db.ExecuteNonQuery("EXEC sp_ArchiveOrder @OrderId", new SqlParameter[] { new SqlParameter("@OrderId", id) });
                    }
                    catch
                    {
                        // Fallback: manual archiving if stored procedure doesn't exist
                        string archiveQuery = @"
                            IF NOT EXISTS (SELECT 1 FROM OrderHistory WHERE OrderId = @OrderId)
                            BEGIN
                                INSERT INTO OrderHistory (OrderId, UserId, OrderNumber, TotalAmount, Status, OrderDate, Notes)
                                SELECT Id, UserId, OrderNumber, TotalAmount, Status, OrderDate, Notes
                                FROM Orders WHERE Id = @OrderId;
                                
                                UPDATE Orders SET IsArchived = 1, ArchivedAt = GETDATE() WHERE Id = @OrderId;
                            END";
                        db.ExecuteNonQuery(archiveQuery, new SqlParameter[] { new SqlParameter("@OrderId", id) });
                    }
                }
            }

            LoadOrders();
            pnlDetails.Visible = false;
            pnlList.Visible = true;
        }


        private void LoadOrderReview(string orderId)
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
                    DateTime date = Convert.ToDateTime(dt.Rows[0]["ReviewDate"]);
                    litAdminReviewStars.Text = RenderStars(rating);
                    litAdminReviewText.Text = Server.HtmlEncode(comment);
                    lblAdminReviewDate.InnerText = date.ToString("dd/MM/yyyy HH:mm");
                    pnlReviewAdmin.Visible = true;
                    lblNoReview.Visible = false;
                }
                else
                {
                    pnlReviewAdmin.Visible = false;
                    lblNoReview.Visible = true;
                }
            }
            catch
            {
                pnlReviewAdmin.Visible = false;
                lblNoReview.Visible = true;
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
            return "<div style='font-size:1.2rem;'>" + stars + "</div>";
        }
        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStatus.SelectedValue == "Cancelled")
            {
                txtCancelReason.Style["display"] = "block";
            }
            else
            {
                txtCancelReason.Style["display"] = "none";
                txtCancelReason.Text = "";
            }
        }

        protected string GetItemTotal(object unitPrice, object quantity)
        {
            if (unitPrice != null && unitPrice != DBNull.Value && quantity != null && quantity != DBNull.Value)
            {
                decimal price = Convert.ToDecimal(unitPrice);
                int qty = Convert.ToInt32(quantity);
                return (price * qty).ToString("C");
            }
            return "0.00 €";
        }

        private string GetStatusLabel(string status)
        {
            switch (status)
            {
                case "Pending": return "En attente";
                case "Processing": return "En préparation";
                case "Shipped": return "Expédié";
                case "Delivered": return "Livré";
                case "Cancelled": return "Annulé";
                default: return status;
            }
        }

        private string BuildStatusEmailBody(string fullName, string orderNumber, string status, string cancelReason)
        {
            string statusLabel = GetStatusLabel(status);
            string reasonSection = status == "Cancelled" && !string.IsNullOrWhiteSpace(cancelReason)
                ? "<p>Raison de l'annulation: <strong>" + Server.HtmlEncode(cancelReason) + "</strong></p>"
                : "";
            string trackingSection = status == "Shipped"
                ? "<p>Vous pouvez suivre votre commande depuis votre profil.</p>"
                : "";
            string body = "<h2>Bonjour " + Server.HtmlEncode(fullName) + ",</h2>"
                        + "<p>Le statut de votre commande <strong>" + Server.HtmlEncode(orderNumber) + "</strong> a été mis à jour: <strong>" + statusLabel + "</strong>.</p>"
                        + reasonSection
                        + trackingSection
                        + "<p>Accédez à vos commandes: <a href='" + ResolveUrl("~/Pages/Public/Profile.aspx?tab=orders") + "'>Mes commandes</a></p>"
                        + "<p>Merci de votre confiance.</p>";
            return body;
        }
    }
}
