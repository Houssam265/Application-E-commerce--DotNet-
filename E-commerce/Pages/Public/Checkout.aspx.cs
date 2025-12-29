using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;
using Ecommerce.Utils;

namespace Ecommerce.Pages.Public
{
    public partial class Checkout : Page
    {
        protected global::System.Web.UI.WebControls.TextBox txtFullName;
        protected global::System.Web.UI.WebControls.TextBox txtStreet;
        protected global::System.Web.UI.WebControls.TextBox txtCity;
        protected global::System.Web.UI.WebControls.TextBox txtZip;
        protected global::System.Web.UI.WebControls.DropDownList ddlCountry;
        protected global::System.Web.UI.WebControls.TextBox txtPhone;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.Repeater rptSummary;
        protected global::System.Web.UI.WebControls.Label lblSubTotal;
        protected global::System.Web.UI.WebControls.Label lblShipping;
        protected global::System.Web.UI.WebControls.Label lblTotal;
        protected global::System.Web.UI.WebControls.Button btnPlaceOrder;
        protected global::System.Web.UI.WebControls.Panel pnlEmptyCart;
        protected global::System.Web.UI.WebControls.Panel pnlCheckout;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            if (!IsPostBack)
            {
                LoadCheckoutData();
            }
        }

        private void LoadCheckoutData()
        {
            try
            {
                DataTable cartItems = CartHelper.GetCartItems();
                
                if (cartItems.Rows.Count == 0)
                {
                    pnlEmptyCart.Visible = true;
                    pnlCheckout.Visible = false;
                    return;
                }

                pnlEmptyCart.Visible = false;
                pnlCheckout.Visible = true;

                // Load user info
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                string userQuery = "SELECT FullName, Phone FROM Users WHERE Id = @UserId";
                SqlParameter[] userParams = { new SqlParameter("@UserId", userId) };
                DataTable userDt = db.ExecuteQuery(userQuery, userParams);
                
                if (userDt.Rows.Count > 0)
                {
                    txtFullName.Text = userDt.Rows[0]["FullName"].ToString();
                    if (userDt.Rows[0]["Phone"] != DBNull.Value)
                        txtPhone.Text = userDt.Rows[0]["Phone"].ToString();
                }

                // Load default address if exists
                string addrQuery = "SELECT TOP 1 * FROM Addresses WHERE UserId = @UserId AND IsDefault = 1 ORDER BY Id DESC";
                SqlParameter[] addrParams = { new SqlParameter("@UserId", userId) };
                DataTable addrDt = db.ExecuteQuery(addrQuery, addrParams);
                if (addrDt.Rows.Count > 0)
                {
                    txtStreet.Text = addrDt.Rows[0]["Street"].ToString();
                    txtCity.Text = addrDt.Rows[0]["City"].ToString();
                    txtZip.Text = addrDt.Rows[0]["ZipCode"].ToString();
                    ddlCountry.SelectedValue = addrDt.Rows[0]["Country"].ToString();
                }

                // Create a new DataTable with TotalPrice column
                DataTable dtSummary = cartItems.Clone();
                dtSummary.Columns.Add("TotalPrice", typeof(decimal));

                // Prepare cart items for summary
                foreach (DataRow row in cartItems.Rows)
                {
                    DataRow newRow = dtSummary.NewRow();
                    newRow["Id"] = row["Id"];
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
                    int quantity = Convert.ToInt32(row["Quantity"]);
                    newRow["TotalPrice"] = unitPrice * quantity;

                    dtSummary.Rows.Add(newRow);
                }

                rptSummary.DataSource = dtSummary;
                rptSummary.DataBind();

                // Calculate totals
                decimal subTotal = CartHelper.GetCartTotal();
                decimal shippingCost = CalculateShipping(subTotal);
                decimal total = subTotal + shippingCost;

                lblSubTotal.Text = subTotal.ToString("F2");
                lblShipping.Text = shippingCost > 0 ? shippingCost.ToString("F2") + " MAD" : "Gratuit";
                lblTotal.Text = total.ToString("F2");
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur lors du chargement: " + Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
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

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate form
                if (string.IsNullOrEmpty(txtFullName.Text) || string.IsNullOrEmpty(txtStreet.Text) || 
                    string.IsNullOrEmpty(txtCity.Text) || string.IsNullOrEmpty(txtZip.Text))
                {
                    litError.Text = "Veuillez remplir tous les champs obligatoires.";
                    pnlError.Visible = true;
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                DataTable cartItems = CartHelper.GetCartItems();
                
                if (cartItems.Rows.Count == 0)
                {
                    litError.Text = "Votre panier est vide.";
                    pnlError.Visible = true;
                    return;
                }

                DbContext db = new DbContext();

                // *** CRITICAL FIX: VALIDATE STOCK BEFORE CREATING ORDER ***
                // This prevents creating orders with insufficient stock
                foreach (DataRow row in cartItems.Rows)
                {
                    int productId = Convert.ToInt32(row["ProductId"]);
                    int? variantId = row["VariantId"] != DBNull.Value ? (int?)Convert.ToInt32(row["VariantId"]) : null;
                    int quantity = Convert.ToInt32(row["Quantity"]);
                    int available = GetAvailableStock(db, productId, variantId);
                    
                    if (quantity > available)
                    {
                        litError.Text = $"❌ La quantité demandée pour '{Server.HtmlEncode(row["Name"].ToString())}' ({quantity}) dépasse le stock disponible ({available}). Veuillez ajuster votre panier.";
                        pnlError.Visible = true;
                        return;
                    }
                }

                // Check if address already exists (to avoid duplicates)
                string phoneValue = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : Server.HtmlEncode(txtPhone.Text.Trim());
                string checkAddrQuery = @"SELECT Id FROM Addresses 
                                         WHERE UserId = @UserId 
                                         AND FullName = @FullName 
                                         AND Street = @Street 
                                         AND City = @City 
                                         AND ZipCode = @Zip 
                                         AND Country = @Country
                                         AND (Phone = @Phone OR (Phone IS NULL AND @Phone IS NULL))";
                SqlParameter[] checkAddrParams = {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@FullName", Server.HtmlEncode(txtFullName.Text.Trim())),
                    new SqlParameter("@Street", Server.HtmlEncode(txtStreet.Text.Trim())),
                    new SqlParameter("@City", Server.HtmlEncode(txtCity.Text.Trim())),
                    new SqlParameter("@Zip", Server.HtmlEncode(txtZip.Text.Trim())),
                    new SqlParameter("@Country", ddlCountry.SelectedValue),
                    new SqlParameter("@Phone", phoneValue ?? (object)DBNull.Value)
                };

                object existingAddrId = db.ExecuteScalar(checkAddrQuery, checkAddrParams);
                int addrId;

                if (existingAddrId != null && existingAddrId != DBNull.Value)
                {
                    // Address already exists, use it
                    addrId = Convert.ToInt32(existingAddrId);
                    
                    // Set it as default if it's not already
                    string updateDefaultQuery = "UPDATE Addresses SET IsDefault = 1 WHERE Id = @Id AND UserId = @UserId";
                    SqlParameter[] updateDefaultParams = {
                        new SqlParameter("@Id", addrId),
                        new SqlParameter("@UserId", userId)
                    };
                    db.ExecuteNonQuery(updateDefaultQuery, updateDefaultParams);
                    
                    // Unset other default addresses
                    string unsetDefaultQuery = "UPDATE Addresses SET IsDefault = 0 WHERE UserId = @UserId AND Id != @Id";
                    SqlParameter[] unsetParams = { 
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@Id", addrId)
                    };
                    db.ExecuteNonQuery(unsetDefaultQuery, unsetParams);
                }
                else
                {
                    // Address doesn't exist, create new one
                    // Unset other default addresses before creating new one
                    string unsetDefaultQuery = "UPDATE Addresses SET IsDefault = 0 WHERE UserId = @UserId";
                    SqlParameter[] unsetParams = { new SqlParameter("@UserId", userId) };
                    db.ExecuteNonQuery(unsetDefaultQuery, unsetParams);

                    // Create address
                    string addrQuery = @"INSERT INTO Addresses (UserId, FullName, Street, City, ZipCode, Country, Phone, IsDefault) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@UserId, @FullName, @Street, @City, @Zip, @Country, @Phone, 1)";
                    SqlParameter[] addrParams = {
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@FullName", Server.HtmlEncode(txtFullName.Text.Trim())),
                        new SqlParameter("@Street", Server.HtmlEncode(txtStreet.Text.Trim())),
                        new SqlParameter("@City", Server.HtmlEncode(txtCity.Text.Trim())),
                        new SqlParameter("@Zip", Server.HtmlEncode(txtZip.Text.Trim())),
                        new SqlParameter("@Country", ddlCountry.SelectedValue),
                        new SqlParameter("@Phone", phoneValue ?? (object)DBNull.Value)
                    };

                    addrId = (int)db.ExecuteScalar(addrQuery, addrParams);
                }

                // Calculate totals
                decimal subTotal = CartHelper.GetCartTotal();
                decimal shippingCost = CalculateShipping(subTotal);
                decimal total = subTotal + shippingCost;

                // Generate order number
                string orderNumber = "CMD-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                // Create order
                string orderQuery = @"INSERT INTO Orders (OrderNumber, UserId, TotalAmount, SubTotal, ShippingCost, Status, ShippingAddressId, ShippingMethod) 
                                      OUTPUT INSERTED.Id 
                                      VALUES (@OrderNumber, @UserId, @Total, @SubTotal, @Shipping, 'Pending', @AddrId, 'Standard')";
                SqlParameter[] orderParams = {
                    new SqlParameter("@OrderNumber", orderNumber),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Total", total),
                    new SqlParameter("@SubTotal", subTotal),
                    new SqlParameter("@Shipping", shippingCost),
                    new SqlParameter("@AddrId", addrId)
                };

                int orderId = (int)db.ExecuteScalar(orderQuery, orderParams);

                // Create order items and update stock
                foreach (DataRow row in cartItems.Rows)
                {
                    int productId = Convert.ToInt32(row["ProductId"]);
                    int? variantId = row["VariantId"] != DBNull.Value ? (int?)Convert.ToInt32(row["VariantId"]) : null;
                    int quantity = Convert.ToInt32(row["Quantity"]);
                    decimal unitPrice = Convert.ToDecimal(row["Price"]);
                    decimal adjustment = row["PriceAdjustment"] != DBNull.Value ? Convert.ToDecimal(row["PriceAdjustment"]) : 0;
                    unitPrice += adjustment;
                    decimal totalPrice = unitPrice * quantity;
                    string productName = row["Name"].ToString();

                    string itemQuery = @"INSERT INTO OrderItems (OrderId, ProductId, VariantId, ProductName, Quantity, UnitPrice, TotalPrice) 
                                        VALUES (@OrderId, @ProductId, @VariantId, @ProductName, @Qty, @UnitPrice, @TotalPrice)";
                    SqlParameter[] itemParams = {
                        new SqlParameter("@OrderId", orderId),
                        new SqlParameter("@ProductId", productId),
                        new SqlParameter("@VariantId", variantId ?? (object)DBNull.Value),
                        new SqlParameter("@ProductName", Server.HtmlEncode(productName)),
                        new SqlParameter("@Qty", quantity),
                        new SqlParameter("@UnitPrice", unitPrice),
                        new SqlParameter("@TotalPrice", totalPrice)
                    };

                    db.ExecuteNonQuery(itemQuery, itemParams);

                    // Update stock (variant first if exists)
                    if (variantId.HasValue)
                    {
                        string updateVarStock = "UPDATE ProductVariants SET StockQuantity = CASE WHEN StockQuantity >= @Qty THEN StockQuantity - @Qty ELSE StockQuantity END WHERE Id = @VarId";
                        db.ExecuteNonQuery(updateVarStock, new SqlParameter[] {
                            new SqlParameter("@Qty", quantity),
                            new SqlParameter("@VarId", variantId.Value)
                        });
                    }

                    string updateStockQuery = "UPDATE Products SET StockQuantity = CASE WHEN StockQuantity >= @Qty THEN StockQuantity - @Qty ELSE StockQuantity END WHERE Id = @ProductId";
                    SqlParameter[] stockParams = {
                        new SqlParameter("@Qty", quantity),
                        new SqlParameter("@ProductId", productId)
                    };
                    db.ExecuteNonQuery(updateStockQuery, stockParams);
                }

                // Clear cart
                CartHelper.ClearCart();
                Session["CartCount"] = 0;

                // Send Confirmation Email
                try 
                {
                    string userEmailQuery = "SELECT Email FROM Users WHERE Id = @UserId";
                    DataTable userEmailDt = db.ExecuteQuery(userEmailQuery, new SqlParameter[] { new SqlParameter("@UserId", userId) });
                    
                    if (userEmailDt.Rows.Count > 0)
                    {
                        string userEmail = userEmailDt.Rows[0]["Email"].ToString();
                        string productsHtml = EmailTemplates.GenerateProductTableHtml(dtSummary, true); // Use dtSummary which already has totals
                        
                        string emailBody = EmailTemplates.GetOrderConfirmationEmailTemplate(
                            txtFullName.Text.Trim(),
                            orderNumber,
                            productsHtml,
                            total
                        );

                        SecurityHelper.SendEmail(userEmail, $"Confirmation de commande {orderNumber}", emailBody);
                    }
                }
                catch (Exception emailEx)
                {
                    // Log error but don't stop the order process
                    Console.WriteLine("Failed to send order confirmation email: " + emailEx.Message);
                }

                // Redirect to confirmation
                Response.Redirect("OrderConfirmation.aspx?id=" + orderId);
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur lors de la commande: " + Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
            }
        }


        private int GetAvailableStock(DbContext db, int productId, int? variantId)
        {
            if (variantId.HasValue)
            {
                object v = db.ExecuteScalar("SELECT StockQuantity FROM ProductVariants WHERE Id = @Id", new SqlParameter[] { new SqlParameter("@Id", variantId.Value) });
                if (v != null && v != DBNull.Value)
                    return Convert.ToInt32(v);
            }
            object p = db.ExecuteScalar("SELECT StockQuantity FROM Products WHERE Id = @Id", new SqlParameter[] { new SqlParameter("@Id", productId) });
            return (p != null && p != DBNull.Value) ? Convert.ToInt32(p) : 0;
        }
    }
}
