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
                string query = @"SELECT Id, OrderNumber, OrderDate, TotalAmount, Status 
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
                    string verifyQuery = "SELECT Id FROM Addresses WHERE Id = @Id AND UserId = @UserId";
                    SqlParameter[] verifyParams = {
                        new SqlParameter("@Id", addressId),
                        new SqlParameter("@UserId", userId)
                    };
                    DataTable verifyDt = db.ExecuteQuery(verifyQuery, verifyParams);

                    if (verifyDt.Rows.Count > 0)
                    {
                        // Check if address is used in any orders
                        string checkOrdersQuery = "SELECT COUNT(*) FROM Orders WHERE ShippingAddressId = @AddressId";
                        SqlParameter[] checkParams = { new SqlParameter("@AddressId", addressId) };
                        int orderCount = Convert.ToInt32(db.ExecuteScalar(checkOrdersQuery, checkParams));

                        if (orderCount > 0)
                        {
                            // Address is used in orders, don't delete but show error
                            litError.Text = "Cette adresse ne peut pas être supprimée car elle est associée à une ou plusieurs commandes.";
                            pnlError.Visible = true;
                            LoadAddresses(); // Reload to refresh the list
                            return;
                        }

                        // Delete the address
                        string deleteQuery = "DELETE FROM Addresses WHERE Id = @Id AND UserId = @UserId";
                        SqlParameter[] deleteParams = {
                            new SqlParameter("@Id", addressId),
                            new SqlParameter("@UserId", userId)
                        };
                        db.ExecuteNonQuery(deleteQuery, deleteParams);

                        // Reload addresses
                        LoadAddresses();
                        litSuccess.Text = "Adresse supprimée avec succès !";
                        pnlSuccess.Visible = true;
                    }
                    else
                    {
                        litError.Text = "Adresse introuvable ou vous n'avez pas l'autorisation de la supprimer.";
                        pnlError.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    litError.Text = "Erreur lors de la suppression: " + Server.HtmlEncode(ex.Message);
                    pnlError.Visible = true;
                }
            }
        }
    }
}

