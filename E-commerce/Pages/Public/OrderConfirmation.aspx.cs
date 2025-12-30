using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using Ecommerce.Data;

namespace Ecommerce.Pages.Public
{
    public partial class OrderConfirmation : Page
    {
        protected global::System.Web.UI.WebControls.Label lblOrderNumber;
        protected global::System.Web.UI.WebControls.Label lblOrderDate;
        protected global::System.Web.UI.WebControls.Label lblTotal;
        protected global::System.Web.UI.WebControls.Panel pnlOrderDetails;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string orderId = Request.QueryString["id"];
            if (string.IsNullOrEmpty(orderId))
            {
                Response.Redirect("Shop.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadOrderDetails(orderId);
            }
        }

        private void LoadOrderDetails(string orderId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                
                string query = @"SELECT OrderNumber, OrderDate, TotalAmount 
                                 FROM Orders 
                                 WHERE Id = @OrderId AND UserId = @UserId";
                SqlParameter[] parameters = {
                    new SqlParameter("@OrderId", orderId),
                    new SqlParameter("@UserId", userId)
                };

                DataTable dt = db.ExecuteQuery(query, parameters);
                
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    lblOrderNumber.Text = Server.HtmlEncode(row["OrderNumber"].ToString());
                    lblOrderDate.Text = Convert.ToDateTime(row["OrderDate"]).ToString("dd/MM/yyyy à HH:mm");
                    lblTotal.Text = Convert.ToDecimal(row["TotalAmount"]).ToString("F2");
                    pnlOrderDetails.Visible = true;
                }
                else
                {
                    Response.Redirect("Shop.aspx");
                }
            }
            catch
            {
                Response.Redirect("Shop.aspx");
            }
        }
    }
}
