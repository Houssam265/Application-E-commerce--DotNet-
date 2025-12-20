using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using Ecommerce.Data;

namespace Ecommerce.Pages.Public
{
    public partial class OrderDetails : Page
    {
        protected global::System.Web.UI.WebControls.Panel pnlNotFound;
        protected global::System.Web.UI.WebControls.Panel pnlOrderDetails;
        protected global::System.Web.UI.WebControls.Label lblOrderNumber;
        protected global::System.Web.UI.WebControls.Label lblOrderDate;
        protected global::System.Web.UI.WebControls.Label lblCustomerName;
        protected global::System.Web.UI.WebControls.Label lblCustomerEmail;
        protected global::System.Web.UI.WebControls.Label lblShippingAddress;
        protected global::System.Web.UI.WebControls.Label lblShippingMethod;
        protected global::System.Web.UI.WebControls.Panel pnlTracking;
        protected global::System.Web.UI.WebControls.Label lblTrackingNumber;
        protected global::System.Web.UI.WebControls.Repeater rptOrderItems;
        protected global::System.Web.UI.WebControls.Label lblSubTotal;
        protected global::System.Web.UI.WebControls.Label lblShippingCost;
        protected global::System.Web.UI.WebControls.Label lblTotal;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl statusBadge;

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
                pnlOrderDetails.Visible = false;
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
                
                string query = @"SELECT O.*, U.FullName, U.Email, A.FullName as AddressName, A.Street, A.City, A.ZipCode, A.Country
                                 FROM Orders O
                                 INNER JOIN Users U ON O.UserId = U.Id
                                 LEFT JOIN Addresses A ON O.ShippingAddressId = A.Id
                                 WHERE O.Id = @OrderId AND O.UserId = @UserId";
                SqlParameter[] parameters = {
                    new SqlParameter("@OrderId", orderId),
                    new SqlParameter("@UserId", userId)
                };

                DataTable dt = db.ExecuteQuery(query, parameters);
                
                if (dt.Rows.Count == 0)
                {
                    pnlNotFound.Visible = true;
                    pnlOrderDetails.Visible = false;
                    return;
                }

                DataRow row = dt.Rows[0];
                lblOrderNumber.Text = Server.HtmlEncode(row["OrderNumber"].ToString());
                lblOrderDate.Text = Convert.ToDateTime(row["OrderDate"]).ToString("dd/MM/yyyy à HH:mm");
                lblCustomerName.Text = Server.HtmlEncode(row["FullName"].ToString());
                lblCustomerEmail.Text = Server.HtmlEncode(row["Email"].ToString());
                
                if (row["Street"] != DBNull.Value)
                {
                    string addressName = row["AddressName"] != DBNull.Value ? row["AddressName"].ToString() + "<br/>" : "";
                    lblShippingAddress.Text = addressName + Server.HtmlEncode(row["Street"].ToString()) + ", " +
                                             Server.HtmlEncode(row["ZipCode"].ToString()) + " " +
                                             Server.HtmlEncode(row["City"].ToString()) + ", " +
                                             Server.HtmlEncode(row["Country"].ToString());
                }
                else
                {
                    lblShippingAddress.Text = "Adresse non disponible";
                }

                lblShippingMethod.Text = row["ShippingMethod"] != DBNull.Value ? row["ShippingMethod"].ToString() : "Standard";
                
                string status = row["Status"].ToString();
                statusBadge.InnerHtml = GetStatusBadge(status);
                statusBadge.Attributes["class"] = "status-badge status-" + status;

                if (row["TrackingNumber"] != DBNull.Value && !string.IsNullOrEmpty(row["TrackingNumber"].ToString()))
                {
                    lblTrackingNumber.Text = Server.HtmlEncode(row["TrackingNumber"].ToString());
                    pnlTracking.Visible = true;
                }

                // Load order items
                string itemsQuery = @"SELECT ProductName, Quantity, UnitPrice, TotalPrice 
                                      FROM OrderItems 
                                      WHERE OrderId = @OrderId";
                SqlParameter[] itemParams = { new SqlParameter("@OrderId", orderId) };
                DataTable itemsDt = db.ExecuteQuery(itemsQuery, itemParams);
                rptOrderItems.DataSource = itemsDt;
                rptOrderItems.DataBind();

                // Calculate totals
                decimal subTotal = Convert.ToDecimal(row["SubTotal"]);
                decimal shippingCost = Convert.ToDecimal(row["ShippingCost"]);
                decimal total = Convert.ToDecimal(row["TotalAmount"]);

                lblSubTotal.Text = subTotal.ToString("F2");
                lblShippingCost.Text = shippingCost > 0 ? shippingCost.ToString("F2") + " MAD" : "Gratuit";
                lblTotal.Text = total.ToString("F2");
            }
            catch
            {
                pnlNotFound.Visible = true;
                pnlOrderDetails.Visible = false;
            }
        }

        private string GetStatusBadge(string status)
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
    }
}

