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

        private string currentStatus = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            string orderId = Request.QueryString["id"];
            if (string.IsNullOrEmpty(orderId))
            {
                pnlNotFound.Visible = true;
                pnlTracking.Visible = false;
                return;
            }

            if (!IsPostBack)
            {
                LoadTracking(orderId);
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

        protected string GetStepClass(object stepData, int index)
        {
            if (stepData == null) return "pending";
            
            TrackingStep step = stepData as TrackingStep;
            if (step == null) return "pending";

            if (currentStatus == "Cancelled")
            {
                return step.Status == "Pending" ? "completed" : "pending";
            }

            string[] statusOrder = { "Pending", "Processing", "Shipped", "Delivered" };
            int currentIndex = Array.IndexOf(statusOrder, currentStatus);
            int stepIndex = Array.IndexOf(statusOrder, step.Status);

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
            return "<i class='fas " + icons[index] + "'></i>";
        }

        protected string GetStepTitle(object stepData)
        {
            if (stepData == null) return "";
            TrackingStep step = stepData as TrackingStep;
            return step?.Title ?? "";
        }

        protected string GetStepDescription(object stepData)
        {
            if (stepData == null) return "";
            TrackingStep step = stepData as TrackingStep;
            return step?.Description ?? "";
        }

        protected string GetStepDate(object stepData)
        {
            if (stepData == null) return "";
            TrackingStep step = stepData as TrackingStep;
            if (step?.Date != null)
            {
                return "<p style='font-size: 0.85rem; color: var(--text-light); margin-top: 0.25rem;'>" + 
                       step.Date.Value.ToString("dd/MM/yyyy à HH:mm") + "</p>";
            }
            return "";
        }

        protected string GetStepLineClass(object stepData, int index)
        {
            return GetStepClass(stepData, index) == "completed" ? "completed" : "";
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
    }
}
