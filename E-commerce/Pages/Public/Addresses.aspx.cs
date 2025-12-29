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

                if (string.IsNullOrEmpty(hfAddressId.Value))
                {
                    // Insert new address
                    // Check for duplicates (exact match including phone) - improved matching
                    string phoneValue = string.IsNullOrEmpty(txtPhone.Text) ? null : Server.HtmlEncode(txtPhone.Text.Trim());
                    string fullNameValue = Server.HtmlEncode(txtFullName.Text.Trim());
                    string streetValue = Server.HtmlEncode(txtStreet.Text.Trim());
                    string cityValue = Server.HtmlEncode(txtCity.Text.Trim());
                    string zipValue = Server.HtmlEncode(txtZipCode.Text.Trim());
                    string countryValue = ddlCountry.SelectedValue;

                    string checkDupQuery = @"SELECT COUNT(*) FROM Addresses 
                        WHERE UserId = @UserId 
                        AND LTRIM(RTRIM(FullName)) = LTRIM(RTRIM(@FullName))
                        AND LTRIM(RTRIM(Street)) = LTRIM(RTRIM(@Street))
                        AND LTRIM(RTRIM(City)) = LTRIM(RTRIM(@City))
                        AND LTRIM(RTRIM(ZipCode)) = LTRIM(RTRIM(@ZipCode))
                        AND LTRIM(RTRIM(Country)) = LTRIM(RTRIM(@Country))
                        AND (LTRIM(RTRIM(ISNULL(Phone, ''))) = LTRIM(RTRIM(ISNULL(@Phone, ''))) 
                             OR (Phone IS NULL AND @Phone IS NULL))";
                    
                    SqlParameter[] dupParams = {
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@FullName", fullNameValue),
                        new SqlParameter("@Street", streetValue),
                        new SqlParameter("@City", cityValue),
                        new SqlParameter("@ZipCode", zipValue),
                        new SqlParameter("@Country", countryValue),
                        new SqlParameter("@Phone", phoneValue ?? (object)DBNull.Value)
                    };
                    
                    int dupCount = Convert.ToInt32(db.ExecuteScalar(checkDupQuery, dupParams));
                    if (dupCount > 0)
                    {
                        litError.Text = "Cette adresse existe déjà dans votre liste.";
                        pnlError.Visible = true;
                        return;
                    }

                    // If setting as default, unset other defaults first
                    if (chkIsDefault.Checked)
                    {
                        string unsetQuery = "UPDATE Addresses SET IsDefault = 0 WHERE UserId = @UserId";
                        SqlParameter[] unsetParams = { new SqlParameter("@UserId", userId) };
                        db.ExecuteNonQuery(unsetQuery, unsetParams);
                    }

                    string insertQuery = @"INSERT INTO Addresses (UserId, FullName, Street, City, ZipCode, Country, Phone, IsDefault) 
                                           VALUES (@UserId, @FullName, @Street, @City, @ZipCode, @Country, @Phone, @IsDefault)";
                    SqlParameter[] insertParams = {
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@FullName", fullNameValue),
                        new SqlParameter("@Street", streetValue),
                        new SqlParameter("@City", cityValue),
                        new SqlParameter("@ZipCode", zipValue),
                        new SqlParameter("@Country", countryValue),
                        new SqlParameter("@Phone", phoneValue ?? (object)DBNull.Value),
                        new SqlParameter("@IsDefault", chkIsDefault.Checked)
                    };
                    db.ExecuteNonQuery(insertQuery, insertParams);
                    litSuccess.Text = "Adresse ajoutée avec succès !";
                }
                else
                {
                    // Update existing address
                    string fullNameValue = Server.HtmlEncode(txtFullName.Text.Trim());
                    string streetValue = Server.HtmlEncode(txtStreet.Text.Trim());
                    string cityValue = Server.HtmlEncode(txtCity.Text.Trim());
                    string zipValue = Server.HtmlEncode(txtZipCode.Text.Trim());
                    string countryValue = ddlCountry.SelectedValue;
                    string phoneValue = string.IsNullOrEmpty(txtPhone.Text) ? null : Server.HtmlEncode(txtPhone.Text.Trim());

                    // If setting as default, unset other defaults first (except current one)
                    if (chkIsDefault.Checked)
                    {
                        string unsetQuery = "UPDATE Addresses SET IsDefault = 0 WHERE UserId = @UserId AND Id != @Id";
                        SqlParameter[] unsetParams = { 
                            new SqlParameter("@UserId", userId),
                            new SqlParameter("@Id", hfAddressId.Value)
                        };
                        db.ExecuteNonQuery(unsetQuery, unsetParams);
                    }
                    else
                    {
                        // If unchecking default, check if there are other addresses. If not, keep this as default
                        string countQuery = "SELECT COUNT(*) FROM Addresses WHERE UserId = @UserId AND Id != @Id";
                        SqlParameter[] countParams = {
                            new SqlParameter("@UserId", userId),
                            new SqlParameter("@Id", hfAddressId.Value)
                        };
                        int otherAddressCount = Convert.ToInt32(db.ExecuteScalar(countQuery, countParams));
                        
                        if (otherAddressCount == 0)
                        {
                            // This is the only address, must be default
                            chkIsDefault.Checked = true;
                        }
                    }

                    string updateQuery = @"UPDATE Addresses SET FullName = @FullName, Street = @Street, City = @City, 
                                           ZipCode = @ZipCode, Country = @Country, Phone = @Phone, IsDefault = @IsDefault 
                                           WHERE Id = @Id AND UserId = @UserId";
                    SqlParameter[] updateParams = {
                        new SqlParameter("@Id", hfAddressId.Value),
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@FullName", fullNameValue),
                        new SqlParameter("@Street", streetValue),
                        new SqlParameter("@City", cityValue),
                        new SqlParameter("@ZipCode", zipValue),
                        new SqlParameter("@Country", countryValue),
                        new SqlParameter("@Phone", phoneValue ?? (object)DBNull.Value),
                        new SqlParameter("@IsDefault", chkIsDefault.Checked)
                    };
                    db.ExecuteNonQuery(updateQuery, updateParams);
                    litSuccess.Text = "Adresse modifiée avec succès !";
                }

                pnlSuccess.Visible = true;
                Response.AddHeader("REFRESH", "2;URL=Profile.aspx?tab=addresses");
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur: " + Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
            }
        }
    }
}

