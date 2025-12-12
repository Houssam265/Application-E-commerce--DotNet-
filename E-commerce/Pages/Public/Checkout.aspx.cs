using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;

namespace Ecommerce.Pages.Public
{
    public partial class Checkout : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.TextBox txtStreet;
        protected global::System.Web.UI.WebControls.TextBox txtCity;
        protected global::System.Web.UI.WebControls.TextBox txtZip;
        protected global::System.Web.UI.WebControls.TextBox txtCountry;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.Repeater rptSummary;
        protected global::System.Web.UI.WebControls.Label lblTotal;
        protected global::System.Web.UI.WebControls.Button btnPlaceOrder;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserEmail"] == null)
            {
                Response.Redirect("Login.aspx?returnUrl=Checkout.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSummary();
            }
        }

        private void LoadSummary()
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart == null || cart.Count == 0)
            {
                Response.Redirect("Cart.aspx");
                return;
            }

            rptSummary.DataSource = cart;
            rptSummary.DataBind();
            lblTotal.Text = cart.Sum(i => i.Total).ToString("C");
        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                List<CartItem> cart = Session["Cart"] as List<CartItem>;
                decimal total = cart.Sum(i => i.Total);

                string street = txtStreet.Text;
                string city = txtCity.Text;
                string zip = txtZip.Text;
                string country = txtCountry.Text;

                DbContext db = new DbContext();
                string addrQuery = "INSERT INTO Addresses (UserId, Street, City, ZipCode, Country) OUTPUT INSERTED.Id VALUES (@UserId, @Street, @City, @Zip, @Country)";
                SqlParameter[] addrParams = {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Street", street),
                    new SqlParameter("@City", city),
                    new SqlParameter("@Zip", zip),
                    new SqlParameter("@Country", country)
                };

                int addrId = (int)db.ExecuteScalar(addrQuery, addrParams);

                string orderQuery = "INSERT INTO Orders (UserId, TotalAmount, Status, ShippingAddressId) OUTPUT INSERTED.Id VALUES (@UserId, @Total, 'Pending', @AddrId)";
                SqlParameter[] orderParams = {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Total", total),
                    new SqlParameter("@AddrId", addrId)
                };

                int orderId = (int)db.ExecuteScalar(orderQuery, orderParams);

                foreach (var item in cart)
                {
                    string itemQuery = "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice, VariantId) VALUES (@OrderId, @ProductId, @Qty, @Price, @VarId)";
                   
                    List<SqlParameter> itemParams = new List<SqlParameter> {
                        new SqlParameter("@OrderId", orderId),
                        new SqlParameter("@ProductId", item.ProductId),
                        new SqlParameter("@Qty", item.Quantity),
                        new SqlParameter("@Price", item.Price)
                    };

                    if (!string.IsNullOrEmpty(item.VariantId))
                        itemParams.Add(new SqlParameter("@VarId", item.VariantId));
                    else
                        itemParams.Add(new SqlParameter("@VarId", DBNull.Value));

                    db.ExecuteNonQuery(itemQuery, itemParams.ToArray());
                }

                Session["Cart"] = null;
                Response.Redirect("OrderConfirmation.aspx?id=" + orderId);

            }
            catch (Exception ex)
            {
                pnlError.Visible = true;
                litError.Text = "Erreur lors de la commande: " + ex.Message;
            }
        }
    }
}
