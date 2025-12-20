using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using Ecommerce.Data;

namespace Ecommerce.Pages.Public
{
    public partial class Addresses : Page
    {
        protected global::System.Web.UI.WebControls.Label lblTitle;
        protected global::System.Web.UI.WebControls.HiddenField hfAddressId;
        protected global::System.Web.UI.WebControls.TextBox txtFullName;
        protected global::System.Web.UI.WebControls.TextBox txtStreet;
        protected global::System.Web.UI.WebControls.TextBox txtCity;
        protected global::System.Web.UI.WebControls.TextBox txtZipCode;
        protected global::System.Web.UI.WebControls.DropDownList ddlCountry;
        protected global::System.Web.UI.WebControls.TextBox txtPhone;
        protected global::System.Web.UI.WebControls.CheckBox chkIsDefault;
        protected global::System.Web.UI.WebControls.Button btnSave;
        protected global::System.Web.UI.WebControls.Panel pnlSuccess;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litSuccess;
        protected global::System.Web.UI.WebControls.Literal litError;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            string addressId = Request.QueryString["id"];
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(addressId))
                {
                    LoadAddress(addressId);
                    lblTitle.Text = "Modifier l'adresse";
                }
                else
                {
                    lblTitle.Text = "Ajouter une adresse";
                }
            }
        }

        private void LoadAddress(string addressId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                string query = "SELECT * FROM Addresses WHERE Id = @Id AND UserId = @UserId";
                SqlParameter[] parameters = {
                    new SqlParameter("@Id", addressId),
                    new SqlParameter("@UserId", userId)
                };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    hfAddressId.Value = addressId;
                    txtFullName.Text = row["FullName"].ToString();
                    txtStreet.Text = row["Street"].ToString();
                    txtCity.Text = row["City"].ToString();
                    txtZipCode.Text = row["ZipCode"].ToString();
                    ddlCountry.SelectedValue = row["Country"].ToString();
                    txtPhone.Text = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : "";
                    chkIsDefault.Checked = Convert.ToBoolean(row["IsDefault"]);
                }
                else
                {
                    Response.Redirect("Profile.aspx");
                }
            }
            catch
            {
                Response.Redirect("Profile.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();

                // If setting as default, unset other defaults
                if (chkIsDefault.Checked)
                {
                    string unsetQuery = "UPDATE Addresses SET IsDefault = 0 WHERE UserId = @UserId";
                    SqlParameter[] unsetParams = { new SqlParameter("@UserId", userId) };
                    db.ExecuteNonQuery(unsetQuery, unsetParams);
                }

                if (string.IsNullOrEmpty(hfAddressId.Value))
                {
                    // Insert new address
                    string insertQuery = @"INSERT INTO Addresses (UserId, FullName, Street, City, ZipCode, Country, Phone, IsDefault) 
                                           VALUES (@UserId, @FullName, @Street, @City, @ZipCode, @Country, @Phone, @IsDefault)";
                    SqlParameter[] insertParams = {
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@FullName", Server.HtmlEncode(txtFullName.Text.Trim())),
                        new SqlParameter("@Street", Server.HtmlEncode(txtStreet.Text.Trim())),
                        new SqlParameter("@City", Server.HtmlEncode(txtCity.Text.Trim())),
                        new SqlParameter("@ZipCode", Server.HtmlEncode(txtZipCode.Text.Trim())),
                        new SqlParameter("@Country", ddlCountry.SelectedValue),
                        new SqlParameter("@Phone", string.IsNullOrEmpty(txtPhone.Text) ? (object)DBNull.Value : Server.HtmlEncode(txtPhone.Text.Trim())),
                        new SqlParameter("@IsDefault", chkIsDefault.Checked)
                    };
                    db.ExecuteNonQuery(insertQuery, insertParams);
                    litSuccess.Text = "Adresse ajoutée avec succès !";
                }
                else
                {
                    // Update existing address
                    string updateQuery = @"UPDATE Addresses SET FullName = @FullName, Street = @Street, City = @City, 
                                           ZipCode = @ZipCode, Country = @Country, Phone = @Phone, IsDefault = @IsDefault 
                                           WHERE Id = @Id AND UserId = @UserId";
                    SqlParameter[] updateParams = {
                        new SqlParameter("@Id", hfAddressId.Value),
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@FullName", Server.HtmlEncode(txtFullName.Text.Trim())),
                        new SqlParameter("@Street", Server.HtmlEncode(txtStreet.Text.Trim())),
                        new SqlParameter("@City", Server.HtmlEncode(txtCity.Text.Trim())),
                        new SqlParameter("@ZipCode", Server.HtmlEncode(txtZipCode.Text.Trim())),
                        new SqlParameter("@Country", ddlCountry.SelectedValue),
                        new SqlParameter("@Phone", string.IsNullOrEmpty(txtPhone.Text) ? (object)DBNull.Value : Server.HtmlEncode(txtPhone.Text.Trim())),
                        new SqlParameter("@IsDefault", chkIsDefault.Checked)
                    };
                    db.ExecuteNonQuery(updateQuery, updateParams);
                    litSuccess.Text = "Adresse modifiée avec succès !";
                }

                pnlSuccess.Visible = true;
                Response.AddHeader("REFRESH", "2;URL=Profile.aspx");
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur: " + Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
            }
        }
    }
}

