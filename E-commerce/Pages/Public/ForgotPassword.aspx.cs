using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using Ecommerce.Data;
using Ecommerce.Utils;

namespace Ecommerce.Pages.Public
{
    public partial class ForgotPassword : Page
    {
        protected global::System.Web.UI.WebControls.Panel pnlForm;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Panel pnlSuccess;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.Literal litSuccess;
        protected global::System.Web.UI.WebControls.TextBox txtEmail;
        protected global::System.Web.UI.WebControls.Button btnSend;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if reset token is provided
            string token = Request.QueryString["token"];
            if (!string.IsNullOrEmpty(token))
            {
                pnlForm.Visible = false;
                ShowResetForm(token);
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                if (string.IsNullOrEmpty(email))
                {
                    litError.Text = "Veuillez entrer votre email.";
                    pnlError.Visible = true;
                    return;
                }

                DbContext db = new DbContext();
                string query = "SELECT Id FROM Users WHERE Email = @Email";
                SqlParameter[] parameters = { new SqlParameter("@Email", email) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    // Generate reset token
                    string token = SecurityHelper.GenerateSecureToken();
                    DateTime expiry = DateTime.Now.AddHours(24);

                    // Save token to database
                    int userId = Convert.ToInt32(dt.Rows[0]["Id"]);
                    string updateQuery = @"UPDATE Users SET ResetPasswordToken = @Token, ResetPasswordExpiry = @Expiry 
                                           WHERE Id = @UserId";
                    SqlParameter[] updateParams = {
                        new SqlParameter("@Token", token),
                        new SqlParameter("@Expiry", expiry),
                        new SqlParameter("@UserId", userId)
                    };
                    db.ExecuteNonQuery(updateQuery, updateParams);

                    // In a real application, send email here
                    // For demo, show the link
                    string resetUrl = Request.Url.GetLeftPart(UriPartial.Authority) + 
                                     "/Pages/Public/ForgotPassword.aspx?token=" + token;
                    
                    litSuccess.Text = $"Un lien de réinitialisation a été généré. " +
                                     $"<br/><br/>Pour la démo, utilisez ce lien : " +
                                     $"<a href='{resetUrl}' style='color: var(--primary-color);'>{resetUrl}</a>" +
                                     $"<br/><br/><strong>Note:</strong> En production, ce lien serait envoyé par email.";
                    pnlSuccess.Visible = true;
                    pnlForm.Visible = false;
                }
                else
                {
                    // Don't reveal if email exists for security
                    litSuccess.Text = "Si cet email existe dans notre système, vous recevrez un lien de réinitialisation.";
                    pnlSuccess.Visible = true;
                    pnlForm.Visible = false;
                }
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur: " + Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
            }
        }

        private void ShowResetForm(string token)
        {
            // This would show a form to enter new password
            // For now, redirect to a reset page
            Response.Redirect("ResetPassword.aspx?token=" + token);
        }
    }
}

