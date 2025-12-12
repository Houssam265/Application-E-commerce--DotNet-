using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace Ecommerce.Pages.Public
{
    public partial class Cart : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.Panel pnlEmptyCart;
        protected global::System.Web.UI.WebControls.Panel pnlCartItems;
        protected global::System.Web.UI.WebControls.Repeater rptCartItems;
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
            List<CartItem> cart = Session["Cart"] as List<CartItem>;

            if (cart != null && cart.Count > 0)
            {
                pnlCartItems.Visible = true;
                pnlEmptyCart.Visible = false;

                rptCartItems.DataSource = cart;
                rptCartItems.DataBind();

                decimal total = cart.Sum(i => i.Total);
                lblTotal.Text = total.ToString("C");
            }
            else
            {
                pnlCartItems.Visible = false;
                pnlEmptyCart.Visible = true;
            }
        }

        protected void rptCartItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                string productId = args[0];
                string variantId = args.Length > 1 ? args[1] : "";

                List<CartItem> cart = Session["Cart"] as List<CartItem>;
                if (cart != null)
                {
                    var item = cart.FirstOrDefault(i => i.ProductId == productId && i.VariantId == variantId);
                    if (item != null)
                    {
                        cart.Remove(item);
                        Session["Cart"] = cart;
                        LoadCart();
                    }
                }
            }
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            if (Session["UserEmail"] == null)
            {
                Response.Redirect("Login.aspx?returnUrl=Checkout.aspx");
            }
            else
            {
                Response.Redirect("Checkout.aspx");
            }
        }
    }
}
