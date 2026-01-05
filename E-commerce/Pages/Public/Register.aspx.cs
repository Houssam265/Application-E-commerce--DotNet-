using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;
using Ecommerce.Utils;
using System.Collections.Generic;

namespace Ecommerce.Pages.Public
{
    public partial class Register : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.TextBox txtFullName;
        protected global::System.Web.UI.WebControls.TextBox txtEmail;
        protected global::System.Web.UI.WebControls.TextBox txtPhone;
        protected global::System.Web.UI.WebControls.TextBox txtPassword;
        protected global::System.Web.UI.WebControls.TextBox txtConfirmPassword;
        protected global::System.Web.UI.WebControls.Button btnRegister;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowError("Veuillez remplir tous les champs obligatoires.");
                return;
            }

            if (!SecurityHelper.IsValidEmail(email))
            {
                ShowError("L'email n'est pas valide.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Les mots de passe ne correspondent pas.");
                return;
            }

            try
            {
                DbContext db = new DbContext();
                
                // Check if user exists
                string checkQuery = "SELECT Id, EmailVerified FROM Users WHERE Email = @Email";
                SqlParameter[] checkParams = { new SqlParameter("@Email", email) };
                DataTable dt = db.ExecuteQuery(checkQuery, checkParams);

                string code = SecurityHelper.GenerateNumericCode(6);
                DateTime expiry = DateTime.Now.AddMinutes(15);
                string passwordHash = SecurityHelper.HashPassword(password);

                if (dt.Rows.Count > 0)
                {
                    bool isVerified = Convert.ToBoolean(dt.Rows[0]["EmailVerified"]);
                    if (isVerified)
                    {
                        ShowError("Cet email est déjà utilisé.");
                        return;
                    }
                    else
                    {
                        // Update existing unverified user
                        string updateQuery = @"UPDATE Users 
                                               SET FullName = @FullName, 
                                                   PasswordHash = @PasswordHash, 
                                                   Phone = @Phone, 
                                                   EmailVerificationCode = @Code, 
                                                   EmailVerificationExpiry = @Expiry 
                                               WHERE Email = @Email";
                        SqlParameter[] updateParams = {
                            new SqlParameter("@FullName", fullName),
                            new SqlParameter("@PasswordHash", passwordHash),
                            new SqlParameter("@Phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone),
                            new SqlParameter("@Code", code),
                            new SqlParameter("@Expiry", expiry),
                            new SqlParameter("@Email", email)
                        };
                        db.ExecuteNonQuery(updateQuery, updateParams);
                    }
                }
                else
                {
                    // Create new user (unverified)
                    string insertQuery = @"INSERT INTO Users (Email, PasswordHash, FullName, Phone, Role, EmailVerified, IsActive, EmailVerificationCode, EmailVerificationExpiry) 
                                           VALUES (@Email, @PasswordHash, @FullName, @Phone, 'Customer', 0, 1, @Code, @Expiry)";
                    SqlParameter[] insertParams = {
                        new SqlParameter("@Email", email),
                        new SqlParameter("@PasswordHash", passwordHash),
                        new SqlParameter("@FullName", fullName),
                        new SqlParameter("@Phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone),
                        new SqlParameter("@Code", code),
                        new SqlParameter("@Expiry", expiry)
                    };
                    db.ExecuteNonQuery(insertQuery, insertParams);
                }

                // Envoyer l'email de vérification
                string subject = "Vérification de votre email";
                string verifyUrl = ResolveUrl("~/Pages/Public/VerifyEmail.aspx?email=" + Server.UrlEncode(email));
                string body = EmailTemplates.GetEmailVerificationTemplate(fullName, code, verifyUrl);

                SecurityHelper.SendEmail(email, subject, body);

                Response.Redirect("VerifyEmail.aspx?email=" + Server.UrlEncode(email));
            }
            catch (Exception ex)
            {
                ShowError("Erreur lors de l'inscription: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            litError.Text = message;
            pnlError.Visible = true;
        }
    }
}
