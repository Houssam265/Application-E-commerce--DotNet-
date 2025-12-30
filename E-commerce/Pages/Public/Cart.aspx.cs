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
        protected global::System.Web.UI.WebControls.Panel pnlStockError;
        protected global::System.Web.UI.WebControls.Literal litStockError;
        protected global::System.Web.UI.WebControls.Repeater rptCartItems;
        protected global::System.Web.UI.WebControls.Label lblSubTotal;
        protected global::System.Web.UI.WebControls.Label lblShipping;
        protected global::System.Web.UI.WebControls.Label lblTotal;
        protected global::System.Web.UI.WebControls.Button btnCheckout;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadCart();
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

                // Create a new DataTable with additional columns
                DataTable dtCart = cartItems.Clone();
                dtCart.Columns.Add("UnitPrice", typeof(decimal));
                dtCart.Columns.Add("TotalPrice", typeof(decimal));
                dtCart.Columns.Add("VariantInfo", typeof(string));
                dtCart.Columns.Add("CartId", typeof(int));

                // Prepare data for repeater
                foreach (DataRow row in cartItems.Rows)
                {
                    DataRow newRow = dtCart.NewRow();
                    newRow["Id"] = row["Id"];
                    newRow["CartId"] = row["Id"];
                    newRow["ProductId"] = row["ProductId"];
                    newRow["VariantId"] = row["VariantId"];
                    newRow["Quantity"] = row["Quantity"];
                    newRow["Name"] = row["Name"];
                    newRow["Price"] = row["Price"];
                    newRow["ImageUrl"] = row["ImageUrl"];
                    newRow["StockQuantity"] = row["StockQuantity"];
                    newRow["PriceAdjustment"] = row["PriceAdjustment"];
                    newRow["VariantType"] = row["VariantType"];
                    newRow["VariantValue"] = row["VariantValue"];

                    decimal unitPrice = Convert.ToDecimal(row["Price"]);
                    decimal adjustment = row["PriceAdjustment"] != DBNull.Value ? Convert.ToDecimal(row["PriceAdjustment"]) : 0;
                    unitPrice += adjustment;
                    newRow["UnitPrice"] = unitPrice;
                    
                    int quantity = Convert.ToInt32(row["Quantity"]);
                    newRow["TotalPrice"] = unitPrice * quantity;
                    
                    // Variant info
                    if (row["VariantType"] != DBNull.Value && row["VariantValue"] != DBNull.Value)
                    {
                        newRow["VariantInfo"] = row["VariantType"] + ": " + row["VariantValue"];
                    }
                    else
                    {
                        newRow["VariantInfo"] = DBNull.Value;
                    }

                    dtCart.Rows.Add(newRow);
                }

                rptCartItems.DataSource = dtCart;
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
                // Log error and show empty cart
                System.Diagnostics.Debug.WriteLine($"Error loading cart: {ex.Message}");
                pnlEmptyCart.Visible = true;
                pnlCartItems.Visible = false;
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
            
            // Hide error panel initially
            pnlStockError.Visible = false;

            switch (e.CommandName)
            {
                case "Remove":
                    CartHelper.RemoveFromCart(cartId);
                    LoadCart();
                    break;

                case "Increase":
                    try
                    {
                        DataTable cartItems = CartHelper.GetCartItems();
                        foreach (DataRow row in cartItems.Rows)
                        {
                            if (Convert.ToInt32(row["Id"]) == cartId)
                            {
                                int currentQty = Convert.ToInt32(row["Quantity"]);
                                int stockQuantity = Convert.ToInt32(row["StockQuantity"]);
                                
                                if (currentQty + 1 > stockQuantity)
                                {
                                    // Show professional error message in page
                                    ShowStockError(stockQuantity, currentQty);
                                    return;
                                }
                                
                                CartHelper.UpdateCartQuantity(cartId, currentQty + 1);
                                break;
                            }
                        }
                        LoadCart();
                    }
                    catch (Exception ex)
                    {
                        // Show professional error message in page
                        ShowStockError(-1, -1, ex.Message);
                    }
                    break;

                case "Decrease":
                    try
                    {
                        DataTable cartItems = CartHelper.GetCartItems();
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
                    }
                    catch (Exception ex)
                    {
                        ShowStockError(-1, -1, ex.Message);
                    }
                    break;
            }
        }

        private void ShowStockError(int availableStock, int currentQty, string customMessage = null)
        {
            pnlStockError.Visible = true;
            
            if (!string.IsNullOrEmpty(customMessage))
            {
                litStockError.Text = Server.HtmlEncode(customMessage);
            }
            else if (availableStock > 0 && currentQty > 0)
            {
                litStockError.Text = $"Quantité disponible : <strong>{availableStock}</strong> unité(s). " +
                                   $"Quantité actuelle dans votre panier : <strong>{currentQty}</strong> unité(s). " +
                                   $"Vous ne pouvez pas ajouter plus d'articles que le stock disponible.";
            }
            else
            {
                litStockError.Text = "Impossible de modifier la quantité. Veuillez réessayer.";
            }
            
            // Scroll to error message
            string script = @"<script>
                setTimeout(function() {
                    var errorPanel = document.getElementById('" + pnlStockError.ClientID + @"');
                    if (errorPanel) {
                        errorPanel.scrollIntoView({ behavior: 'smooth', block: 'center' });
                    }
                }, 100);
            </script>";
            ClientScript.RegisterStartupScript(this.GetType(), "ScrollToError", script);
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

        protected string GetImageUrl(object imageUrl)
        {
            if (imageUrl == null || imageUrl == DBNull.Value)
            {
                return ResolveUrl("~/Assets/Images/placeholder.svg");
            }

            string imageUrlStr = imageUrl.ToString().Trim();
            
            if (string.IsNullOrEmpty(imageUrlStr))
            {
                return ResolveUrl("~/Assets/Images/placeholder.svg");
            }

            // Si c'est une URL absolue (http/https), la retourner telle quelle
            if (imageUrlStr.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                imageUrlStr.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return imageUrlStr;
            }

            // Si c'est un simple nom de fichier (sans slash), ajouter le chemin de base
            if (!imageUrlStr.Contains("/") && !imageUrlStr.Contains("\\"))
            {
                return ResolveUrl("~/Assets/Images/Products/" + imageUrlStr);
            }

            // Si c'est un chemin relatif, le résoudre
            if (imageUrlStr.StartsWith("~/") || imageUrlStr.StartsWith("../") || imageUrlStr.StartsWith("./"))
            {
                return ResolveUrl(imageUrlStr);
            }

            // Si le chemin commence par Assets/, le résoudre avec ~/
            if (imageUrlStr.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                return ResolveUrl("~/" + imageUrlStr);
            }

            // Sinon, essayer de résoudre tel quel
            return ResolveUrl("~/Assets/Images/Products/" + imageUrlStr);
        }
    }
}
