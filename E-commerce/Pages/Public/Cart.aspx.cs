using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;
using Ecommerce.Utils;

namespace Ecommerce.Pages.Public
{
    public partial class Cart : Page
    {
        protected global::System.Web.UI.WebControls.Panel pnlEmptyCart;
        protected global::System.Web.UI.WebControls.Panel pnlCartItems;
        protected global::System.Web.UI.WebControls.Repeater rptCartItems;
        protected global::System.Web.UI.WebControls.Label lblSubTotal;
        protected global::System.Web.UI.WebControls.Label lblShipping;
        protected global::System.Web.UI.WebControls.Label lblTotal;
        protected global::System.Web.UI.WebControls.Button btnCheckout;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCart();
            }
        }

        private void LoadCart()
        {
            try
            {
                DataTable cartItems = CartHelper.GetCartItems();
                
                if (cartItems.Rows.Count == 0)
                {
                    pnlEmptyCart.Visible = true;
                    pnlCartItems.Visible = false;
                    return;
                }

                pnlEmptyCart.Visible = false;
                pnlCartItems.Visible = true;

                // Prepare data for repeater
                foreach (DataRow row in cartItems.Rows)
                {
                    decimal unitPrice = Convert.ToDecimal(row["Price"]);
                    decimal adjustment = row["PriceAdjustment"] != DBNull.Value ? Convert.ToDecimal(row["PriceAdjustment"]) : 0;
                    unitPrice += adjustment;
                    row["UnitPrice"] = unitPrice;
                    
                    int quantity = Convert.ToInt32(row["Quantity"]);
                    row["TotalPrice"] = unitPrice * quantity;
                    
                    // Variant info
                    if (row["VariantType"] != DBNull.Value && row["VariantValue"] != DBNull.Value)
                    {
                        row["VariantInfo"] = row["VariantType"] + ": " + row["VariantValue"];
                    }
                }

                rptCartItems.DataSource = cartItems;
                rptCartItems.DataBind();

                // Calculate totals
                decimal subTotal = CartHelper.GetCartTotal();
                decimal shippingCost = CalculateShipping(subTotal);
                decimal total = subTotal + shippingCost;

                lblSubTotal.Text = subTotal.ToString("F2");
                lblShipping.Text = shippingCost > 0 ? shippingCost.ToString("F2") + " MAD" : "Gratuit";
                lblTotal.Text = total.ToString("F2");

                // Update session cart count
                Session["CartCount"] = CartHelper.GetCartItemCount();
            }
            catch (Exception ex)
            {
                // Error handling
            }
        }

        private decimal CalculateShipping(decimal subTotal)
        {
            // Free shipping for orders over 500 MAD
            if (subTotal >= 500)
                return 0;

            // Standard shipping cost
            return 30.00m;
        }

        protected void rptCartItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int cartId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "Remove":
                    CartHelper.RemoveFromCart(cartId);
                    LoadCart();
                    break;

                case "Increase":
                    DataTable cartItems = CartHelper.GetCartItems();
                    foreach (DataRow row in cartItems.Rows)
                    {
                        if (Convert.ToInt32(row["Id"]) == cartId)
                        {
                            int currentQty = Convert.ToInt32(row["Quantity"]);
                            CartHelper.UpdateCartQuantity(cartId, currentQty + 1);
                            break;
                        }
                    }
                    LoadCart();
                    break;

                case "Decrease":
                    cartItems = CartHelper.GetCartItems();
                    foreach (DataRow row in cartItems.Rows)
                    {
                        if (Convert.ToInt32(row["Id"]) == cartId)
                        {
                            int currentQty = Convert.ToInt32(row["Quantity"]);
                            if (currentQty > 1)
                            {
                                CartHelper.UpdateCartQuantity(cartId, currentQty - 1);
                            }
                            break;
                        }
                    }
                    LoadCart();
                    break;
            }
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            DataTable cartItems = CartHelper.GetCartItems();
            if (cartItems.Rows.Count == 0)
            {
                return;
            }

            Response.Redirect("Checkout.aspx");
        }
    }
}
