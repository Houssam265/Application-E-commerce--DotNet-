using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;

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
                
                ddlStatus.SelectedValue = row["Status"].ToString();

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
            db.ExecuteNonQuery("UPDATE Orders SET Status = @Status WHERE Id = @Id", new SqlParameter[] {
                new SqlParameter("@Status", status),
                new SqlParameter("@Id", id)
            });

            LoadOrders();
            pnlDetails.Visible = false;
            pnlList.Visible = true;
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
    }
}
