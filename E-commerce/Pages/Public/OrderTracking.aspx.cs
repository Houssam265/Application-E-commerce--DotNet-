using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.UI;
using Ecommerce.Data;

namespace Ecommerce.Pages.Public
{
    public partial class OrderTracking : Page
    {
        protected global::System.Web.UI.WebControls.Panel pnlNotFound;
        protected global::System.Web.UI.WebControls.Panel pnlTracking;
        protected global::System.Web.UI.WebControls.Label lblOrderNumber;
        protected global::System.Web.UI.WebControls.Repeater rptTrackingSteps;
        protected global::System.Web.UI.WebControls.Panel pnlReview;
        protected global::System.Web.UI.WebControls.Panel pnlReviewDisplay;
        protected global::System.Web.UI.WebControls.DropDownList ddlRating;
        protected global::System.Web.UI.WebControls.TextBox txtReview;
        protected global::System.Web.UI.WebControls.Button btnSubmitReview;
        protected global::System.Web.UI.WebControls.Label lblReviewError;
        protected global::System.Web.UI.WebControls.Label lblReviewSuccess;
        protected global::System.Web.UI.WebControls.Literal litReviewStars;
        protected global::System.Web.UI.WebControls.Literal litReviewText;

        private string currentStatus = "";
        private string orderId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            orderId = Request.QueryString["id"];
            if (string.IsNullOrEmpty(orderId))
            {
                pnlNotFound.Visible = true;
                pnlTracking.Visible = false;
                return;
            }

            if (!IsPostBack)
            {
                LoadTracking(orderId);
                LoadReview(orderId);
            }
        }

        private void LoadTracking(string orderId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                
                string query = @"SELECT OrderNumber, Status, OrderDate, UpdatedAt 
                                 FROM Orders 
                                 WHERE Id = @OrderId AND UserId = @UserId";
                SqlParameter[] parameters = {
                    new SqlParameter("@OrderId", orderId),
                    new SqlParameter("@UserId", userId)
                };

                DataTable dt = db.ExecuteQuery(query, parameters);
                
                if (dt.Rows.Count == 0)
                {
                    pnlNotFound.Visible = true;
                    pnlTracking.Visible = false;
                    return;
                }

                DataRow row = dt.Rows[0];
                lblOrderNumber.Text = Server.HtmlEncode(row["OrderNumber"].ToString());
                currentStatus = row["Status"].ToString();

                // Create tracking steps
                List<TrackingStep> steps = new List<TrackingStep>();
                DateTime orderDate = Convert.ToDateTime(row["OrderDate"]);
                
                steps.Add(new TrackingStep
                {
                    Status = "Pending",
                    Title = "Commande reçue",
                    Description = "Votre commande a été reçue et est en attente de traitement",
                    Date = orderDate,
                    IsCompleted = currentStatus != "Cancelled"
                });

                steps.Add(new TrackingStep
                {
                    Status = "Processing",
                    Title = "En préparation",
                    Description = "Votre commande est en cours de préparation",
                    Date = currentStatus == "Processing" || currentStatus == "Shipped" || currentStatus == "Delivered" ? orderDate.AddHours(2) : (DateTime?)null,
                    IsCompleted = currentStatus == "Shipped" || currentStatus == "Delivered"
                });

                steps.Add(new TrackingStep
                {
                    Status = "Shipped",
                    Title = "Expédiée",
                    Description = "Votre commande a été expédiée",
                    Date = currentStatus == "Shipped" || currentStatus == "Delivered" ? orderDate.AddDays(1) : (DateTime?)null,
                    IsCompleted = currentStatus == "Delivered"
                });

                steps.Add(new TrackingStep
                {
                    Status = "Delivered",
                    Title = "Livrée",
                    Description = "Votre commande a été livrée",
                    Date = currentStatus == "Delivered" ? orderDate.AddDays(3) : (DateTime?)null,
                    IsCompleted = currentStatus == "Delivered"
                });

                rptTrackingSteps.DataSource = steps;
                rptTrackingSteps.DataBind();
            }
            catch
            {
                pnlNotFound.Visible = true;
                pnlTracking.Visible = false;
            }
        }

        private void LoadReview(string orderId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                
                // Load existing review
                DataTable dt = db.ExecuteQuery(
                    @"SELECT TOP 1 Rating, Comment 
                      FROM Reviews 
                      WHERE OrderId = @OrderId AND UserId = @UserId AND ProductId IS NULL 
                      ORDER BY ReviewDate DESC",
                    new SqlParameter[] {
                        new SqlParameter("@OrderId", orderId),
                        new SqlParameter("@UserId", userId)
                    });

                if (dt.Rows.Count > 0)
                {
                    // Display existing review
                    int rating = Convert.ToInt32(dt.Rows[0]["Rating"]);
                    string comment = dt.Rows[0]["Comment"] != DBNull.Value ? dt.Rows[0]["Comment"].ToString() : "";
                    litReviewStars.Text = RenderStars(rating);
                    litReviewText.Text = Server.HtmlEncode(comment);
                    pnlReviewDisplay.Visible = true;
                    pnlReview.Visible = false;
                }
                else
                {
                    // Allow review if order is Delivered or Cancelled
                    bool canShowForm = (currentStatus == "Delivered" || currentStatus == "Cancelled");
                    pnlReview.Visible = canShowForm;
                    pnlReviewDisplay.Visible = false;
                }
            }
            catch
            {
                pnlReview.Visible = false;
                pnlReviewDisplay.Visible = false;
            }
        }

        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int rating = int.TryParse(ddlRating.SelectedValue, out var r) ? r : 0;
                string comment = txtReview.Text.Trim();

                if (rating < 1 || rating > 5)
                {
                    lblReviewError.Text = "Veuillez sélectionner une note valide.";
                    lblReviewError.Visible = true;
                    lblReviewSuccess.Visible = false;
                    return;
                }

                DbContext db = new DbContext();
                
                // Load status if not already loaded (e.g. on postback)
                if (string.IsNullOrEmpty(currentStatus))
                {
                    object statusObj = db.ExecuteScalar(
                        "SELECT Status FROM Orders WHERE Id = @OrderId AND UserId = @UserId",
                        new SqlParameter[] { 
                            new SqlParameter("@OrderId", orderId),
                            new SqlParameter("@UserId", userId)
                        });
                    currentStatus = statusObj?.ToString() ?? "";
                }

                bool canSubmit = (currentStatus == "Delivered" || currentStatus == "Cancelled");
                if (!canSubmit)
                {
                    lblReviewError.Text = "Vous ne pouvez donner un avis que pour une commande livrée ou annulée.";
                    lblReviewError.Visible = true;
                    lblReviewSuccess.Visible = false;
                    return;
                }
                
                object exists = db.ExecuteScalar(
                    @"SELECT COUNT(*) FROM Reviews 
                      WHERE OrderId = @OrderId AND UserId = @UserId AND ProductId IS NULL",
                    new SqlParameter[] {
                        new SqlParameter("@OrderId", orderId),
                        new SqlParameter("@UserId", userId)
                    });
                if (Convert.ToInt32(exists) > 0)
                {
                    lblReviewError.Text = "Vous avez déjà donné votre avis pour cette commande.";
                    lblReviewError.Visible = true;
                    lblReviewSuccess.Visible = false;
                    return;
                }

                db.ExecuteNonQuery(
                    @"INSERT INTO Reviews (ProductId, UserId, OrderId, Rating, Comment, ReviewDate, IsApproved, IsVerifiedPurchase) 
                      VALUES (NULL, @UserId, @OrderId, @Rating, @Comment, GETDATE(), 1, 1)",
                    new SqlParameter[] {
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@OrderId", orderId),
                        new SqlParameter("@Rating", rating),
                        new SqlParameter("@Comment", string.IsNullOrWhiteSpace(comment) ? (object)DBNull.Value : comment)
                    });

                lblReviewSuccess.Text = "Merci pour votre avis !";
                lblReviewSuccess.Visible = true;
                lblReviewError.Visible = false;
                pnlReview.Visible = false;
                LoadReview(orderId);
            }
            catch (Exception ex)
            {
                lblReviewError.Text = "Erreur: " + Server.HtmlEncode(ex.Message);
                lblReviewError.Visible = true;
                lblReviewSuccess.Visible = false;
            }
        }

        protected string GetStepClass(object status, int index)
        {
            if (status == null) return "pending";
            
            string stepStatus = status.ToString();

            if (currentStatus == "Cancelled")
            {
                return stepStatus == "Pending" ? "completed" : "pending";
            }

            string[] statusOrder = { "Pending", "Processing", "Shipped", "Delivered" };
            int currentIndex = Array.IndexOf(statusOrder, currentStatus);
            int stepIndex = Array.IndexOf(statusOrder, stepStatus);

            if (stepIndex < currentIndex)
                return "completed";
            else if (stepIndex == currentIndex)
                return "active";
            else
                return "pending";
        }

        protected string GetStepIcon(int index)
        {
            string[] icons = { "fa-check-circle", "fa-box", "fa-truck", "fa-check-circle" };
            if (index >= 0 && index < icons.Length)
            {
                return "<i class='fas " + icons[index] + "'></i>";
            }
            return "<i class='fas fa-circle'></i>";
        }

        protected string GetStepDate(object dateObj)
        {
            if (dateObj == null || dateObj == DBNull.Value) return "";
            
            try
            {
                DateTime? date = dateObj as DateTime?;
                if (date.HasValue)
                {
                    return "<p style='font-size: 0.85rem; color: var(--text-light); margin-top: 0.25rem;'>" + 
                           date.Value.ToString("dd/MM/yyyy à HH:mm") + "</p>";
                }
            }
            catch { }
            
            return "";
        }

        protected string GetStepLineClass(object status, int index)
        {
            return GetStepClass(status, index) == "completed" ? "completed" : "";
        }

        protected int GetTotalSteps()
        {
            return 4;
        }

        private class TrackingStep
        {
            public string Status { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? Date { get; set; }
            public bool IsCompleted { get; set; }
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
    }
}
