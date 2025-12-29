using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;
using Ecommerce.Utils;

namespace Ecommerce.Pages.Public
{
    public partial class Profile : Page
    {
        protected global::System.Web.UI.WebControls.Label lblFullName;
        protected global::System.Web.UI.WebControls.Label lblEmail;
        protected global::System.Web.UI.WebControls.Label lblPhone;
        protected global::System.Web.UI.WebControls.TextBox txtFullName;
        protected global::System.Web.UI.WebControls.TextBox txtEmail;
        protected global::System.Web.UI.WebControls.TextBox txtPhone;
        protected global::System.Web.UI.WebControls.TextBox txtNewPassword;
        protected global::System.Web.UI.WebControls.TextBox txtConfirmPassword;
        protected global::System.Web.UI.WebControls.Button btnSave;
        protected global::System.Web.UI.WebControls.Panel pnlSuccess;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litSuccess;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.Panel pnlAddressSuccess;
        protected global::System.Web.UI.WebControls.Panel pnlAddressError;
        protected global::System.Web.UI.WebControls.Literal litAddressSuccess;
        protected global::System.Web.UI.WebControls.Literal litAddressError;
        protected global::System.Web.UI.WebControls.Repeater rptOrders;
        protected global::System.Web.UI.WebControls.Repeater rptAddresses;
        protected global::System.Web.UI.WebControls.Label lblNoOrders;
        protected global::System.Web.UI.WebControls.Label lblNoAddresses;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            if (!IsPostBack)
            {
                LoadUserInfo();
                LoadOrders();
                LoadAddresses();
            }
            else
            {
                // On postback, reload addresses if we're on the addresses tab
                string currentTab = Request.QueryString["tab"];
                if (currentTab == "addresses")
                {
                    LoadAddresses();
                }
            }
        }

        private void LoadUserInfo()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                string query = "SELECT FullName, Email, Phone FROM Users WHERE Id = @UserId";
                SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    lblFullName.Text = Server.HtmlEncode(row["FullName"].ToString());
                    lblEmail.Text = Server.HtmlEncode(row["Email"].ToString());
                    lblPhone.Text = row["Phone"] != DBNull.Value ? Server.HtmlEncode(row["Phone"].ToString()) : "Non renseigné";
                    
                    txtFullName.Text = row["FullName"].ToString();
                    txtEmail.Text = row["Email"].ToString();
                    txtPhone.Text = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : "";
                }
            }
            catch { }
        }

        private void LoadOrders()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                string query = @"SELECT Id, OrderNumber, OrderDate, TotalAmount, Status, Notes 
                                 FROM Orders 
                                 WHERE UserId = @UserId 
                                 ORDER BY OrderDate DESC";
                SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    rptOrders.DataSource = dt;
                    rptOrders.DataBind();
                    lblNoOrders.Visible = false;
                }
                else
                {
                    lblNoOrders.Visible = true;
                }
            }
            catch { }
        }

        private void LoadAddresses()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                string query = @"SELECT Id, FullName, Street, City, ZipCode, Country, IsDefault 
                                 FROM Addresses 
                                 WHERE UserId = @UserId 
                                 ORDER BY IsDefault DESC, Id DESC";
                SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    rptAddresses.DataSource = dt;
                    rptAddresses.DataBind();
                    lblNoAddresses.Visible = false;
                }
                else
                {
                    lblNoAddresses.Visible = true;
                }
            }
            catch { }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();

                // Update basic info
                string updateQuery = @"UPDATE Users SET FullName = @FullName, Email = @Email, Phone = @Phone 
                                      WHERE Id = @UserId";
                SqlParameter[] parameters = {
                    new SqlParameter("@FullName", Server.HtmlEncode(txtFullName.Text.Trim())),
                    new SqlParameter("@Email", Server.HtmlEncode(txtEmail.Text.Trim())),
                    new SqlParameter("@Phone", string.IsNullOrEmpty(txtPhone.Text) ? (object)DBNull.Value : Server.HtmlEncode(txtPhone.Text.Trim())),
                    new SqlParameter("@UserId", userId)
                };
                db.ExecuteNonQuery(updateQuery, parameters);

                // Update password if provided
                if (!string.IsNullOrEmpty(txtNewPassword.Text))
                {
                    if (txtNewPassword.Text != txtConfirmPassword.Text)
                    {
                        litError.Text = "Les mots de passe ne correspondent pas.";
                        pnlError.Visible = true;
                        return;
                    }

                    string passwordHash = SecurityHelper.HashPassword(txtNewPassword.Text);
                    string pwdQuery = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Id = @UserId";
                    SqlParameter[] pwdParams = {
                        new SqlParameter("@PasswordHash", passwordHash),
                        new SqlParameter("@UserId", userId)
                    };
                    db.ExecuteNonQuery(pwdQuery, pwdParams);
                }

                // Update session
                Session["FullName"] = txtFullName.Text.Trim();
                Session["UserEmail"] = txtEmail.Text.Trim();

                litSuccess.Text = "Vos informations ont été mises à jour avec succès !";
                pnlSuccess.Visible = true;
                LoadUserInfo();
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur lors de la mise à jour: " + Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
            }
        }

        protected string GetOrderDate(object orderDate)
        {
            if (orderDate != null && orderDate != DBNull.Value)
            {
                return Convert.ToDateTime(orderDate).ToString("dd/MM/yyyy à HH:mm");
            }
            return "";
        }

        protected string GetTrackingButton(object status, object orderId)
        {
            string statusStr = status?.ToString() ?? "";
            if (statusStr == "Shipped" || statusStr == "Delivered")
            {
                return $"<a href='OrderTracking.aspx?id={orderId}' class='btn btn-outline' style='padding: 8px 15px;'><i class='fas fa-truck'></i> Suivre</a>";
            }
            return "";
        }

        protected string GetCancelReason(object status, object notes)
        {
            string s = status?.ToString() ?? "";
            if (s == "Cancelled")
            {
                string reason = notes == null || notes == DBNull.Value ? "Aucune raison fournie." : Server.HtmlEncode(notes.ToString());
                return $"<div style='margin-top:0.5rem; color: var(--danger-color);'><i class='fas fa-ban'></i> Raison: {reason}</div>";
            }
            return "";
        }

        protected string GetDefaultBadge(object isDefault)
        {
            if (isDefault != null && isDefault != DBNull.Value && Convert.ToBoolean(isDefault))
            {
                return "<span class='badge badge-primary' style='margin-bottom: 0.5rem; display: inline-block;'>Adresse par défaut</span>";
            }
            return "";
        }

        protected string GetActiveTabClass(string tabName)
        {
            string currentTab = Request.QueryString["tab"];
            if (string.IsNullOrEmpty(currentTab))
                currentTab = "info"; // Default tab
            
            return currentTab == tabName ? "active" : "";
        }

        protected void rptAddresses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                try
                {
                    int addressId = Convert.ToInt32(e.CommandArgument);
                    int userId = Convert.ToInt32(Session["UserId"]);

                    DbContext db = new DbContext();

                    // Verify that the address belongs to the user before deleting
                    string verifyQuery = "SELECT Id, IsDefault FROM Addresses WHERE Id = @Id AND UserId = @UserId";
                    SqlParameter[] verifyParams = {
                        new SqlParameter("@Id", addressId),
                        new SqlParameter("@UserId", userId)
                    };
                    DataTable verifyDt = db.ExecuteQuery(verifyQuery, verifyParams);

                    if (verifyDt.Rows.Count > 0)
                    {
                        // Check if this is the default address
                        bool isDefault = false;
                        if (verifyDt.Rows[0]["IsDefault"] != DBNull.Value)
                        {
                            isDefault = Convert.ToBoolean(verifyDt.Rows[0]["IsDefault"]);
                        }
                        
                        // Check if address is used in any orders (for informational purposes)
                        string checkOrdersQuery = "SELECT COUNT(*) FROM Orders WHERE ShippingAddressId = @AddressId";
                        SqlParameter[] checkParams = { new SqlParameter("@AddressId", addressId) };
                        int orderCount = Convert.ToInt32(db.ExecuteScalar(checkOrdersQuery, checkParams));

                        // If this is the default address, set another address as default (if any exist)
                        if (isDefault)
                        {
                            string setNewDefaultQuery = @"
                                UPDATE TOP (1) Addresses 
                                SET IsDefault = 1 
                                WHERE UserId = @UserId AND Id != @Id AND IsDefault = 0";
                            SqlParameter[] setDefaultParams = {
                                new SqlParameter("@UserId", userId),
                                new SqlParameter("@Id", addressId)
                            };
                            db.ExecuteNonQuery(setNewDefaultQuery, setDefaultParams);
                        }

                        // Delete the address (even if used in orders - orders will keep the reference)
                        string deleteQuery = "DELETE FROM Addresses WHERE Id = @Id AND UserId = @UserId";
                        SqlParameter[] deleteParams = {
                            new SqlParameter("@Id", addressId),
                            new SqlParameter("@UserId", userId)
                        };
                        int rowsAffected = db.ExecuteNonQuery(deleteQuery, deleteParams);

                        if (rowsAffected > 0)
                        {
                            // Reload addresses
                            LoadAddresses();
                            
                            string message = "Adresse supprimée avec succès !";
                            if (isDefault)
                            {
                                message += " Une autre adresse a été définie comme adresse par défaut.";
                            }
                            if (orderCount > 0)
                            {
                                message += " Note: Cette adresse était utilisée dans " + orderCount + " commande(s).";
                            }
                            
                            // Use address-specific panels
                            litAddressSuccess.Text = message;
                            pnlAddressSuccess.Visible = true;
                            pnlAddressError.Visible = false;
                            
                            // Ensure addresses tab is active by redirecting
                            Response.Redirect("Profile.aspx?tab=addresses");
                            return;
                        }
                        else
                        {
                            litAddressError.Text = "Erreur: L'adresse n'a pas pu être supprimée. Veuillez réessayer.";
                            pnlAddressError.Visible = true;
                            pnlAddressSuccess.Visible = false;
                        }
                    }
                    else
                    {
                        litAddressError.Text = "Adresse introuvable ou vous n'avez pas l'autorisation de la supprimer.";
                        pnlAddressError.Visible = true;
                        pnlAddressSuccess.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    litAddressError.Text = "Erreur lors de la suppression: " + Server.HtmlEncode(ex.Message);
                    pnlAddressError.Visible = true;
                    pnlAddressSuccess.Visible = false;
                }
            }
        }
    }
}

