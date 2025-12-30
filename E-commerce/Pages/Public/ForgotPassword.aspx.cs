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
        protected global::System.Web.UI.WebControls.Panel pnlVerifyCode;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.Literal litSuccess;
        protected global::System.Web.UI.WebControls.TextBox txtEmail;
        protected global::System.Web.UI.WebControls.TextBox txtVerificationCode;
        protected global::System.Web.UI.WebControls.Button btnSend;
        protected global::System.Web.UI.WebControls.Button btnVerifyCode;
        protected global::System.Web.UI.WebControls.LinkButton btnResendCode;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlForm.Visible = true;
                pnlVerifyCode.Visible = false;
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
                string query = "SELECT Id, FullName FROM Users WHERE Email = @Email";
                SqlParameter[] parameters = { new SqlParameter("@Email", email) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    // Generate 6-digit verification code
                    string verificationCode = SecurityHelper.GenerateNumericCode(6);
                    DateTime expiry = DateTime.Now.AddMinutes(15); // Code valid for 15 minutes

                    // Save code to database (using ResetPasswordToken field to store the code)
                    int userId = Convert.ToInt32(dt.Rows[0]["Id"]);
                    string userName = dt.Rows[0]["FullName"] != DBNull.Value ? dt.Rows[0]["FullName"].ToString() : email;
                    
                    string updateQuery = @"UPDATE Users SET ResetPasswordToken = @Code, ResetPasswordExpiry = @Expiry 
                                           WHERE Id = @UserId";
                    SqlParameter[] updateParams = {
                        new SqlParameter("@Code", verificationCode),
                        new SqlParameter("@Expiry", expiry),
                        new SqlParameter("@UserId", userId)
                    };
                    db.ExecuteNonQuery(updateQuery, updateParams);

                    // Send email with verification code
                    string emailSubject = "Code de vérification - Réinitialisation de mot de passe";
                    string emailBody = EmailTemplates.GetPasswordResetCodeEmailTemplate(userName, verificationCode);
                    
                    SecurityHelper.SendEmail(email, emailSubject, emailBody);

                    // Store email in session for verification step
                    Session["ResetPasswordEmail"] = email;
                    
                    // Show verification code form
                    pnlForm.Visible = false;
                    pnlVerifyCode.Visible = true;
                    pnlError.Visible = false;
                    pnlSuccess.Visible = false;
                }
                else
                {
                    // Don't reveal if email exists for security
                    litSuccess.Text = "Si cet email existe dans notre système, vous recevrez un code de vérification par email.";
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

        protected void btnVerifyCode_Click(object sender, EventArgs e)
        {
            try
            {
                string code = txtVerificationCode.Text.Trim();
                string email = Session["ResetPasswordEmail"]?.ToString();

                if (string.IsNullOrEmpty(code) || code.Length != 6)
                {
                    litError.Text = "Veuillez entrer un code de vérification valide (6 chiffres).";
                    pnlError.Visible = true;
                    return;
                }

                if (string.IsNullOrEmpty(email))
                {
                    litError.Text = "Session expirée. Veuillez recommencer.";
                    pnlError.Visible = true;
                    pnlVerifyCode.Visible = false;
                    pnlForm.Visible = true;
                    return;
                }

                DbContext db = new DbContext();
                string query = @"SELECT Id FROM Users 
                                WHERE Email = @Email 
                                AND ResetPasswordToken = @Code 
                                AND ResetPasswordExpiry > GETDATE()";
                SqlParameter[] parameters = {
                    new SqlParameter("@Email", email),
                    new SqlParameter("@Code", code)
                };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    // Code is valid, redirect to reset password page
                    int userId = Convert.ToInt32(dt.Rows[0]["Id"]);
                    string token = SecurityHelper.GenerateSecureToken();
                    
                    // Generate a secure token for password reset
                    string updateTokenQuery = @"UPDATE Users SET ResetPasswordToken = @Token, ResetPasswordExpiry = DATEADD(HOUR, 1, GETDATE())
                                               WHERE Id = @UserId";
                    db.ExecuteNonQuery(updateTokenQuery, new SqlParameter[] {
                        new SqlParameter("@Token", token),
                        new SqlParameter("@UserId", userId)
                    });
                    
                    Response.Redirect("ResetPassword.aspx?token=" + Server.UrlEncode(token));
                }
                else
                {
                    litError.Text = "Code de vérification invalide ou expiré. Veuillez réessayer.";
                    pnlError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur: " + Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
            }
        }

        protected void btnResendCode_Click(object sender, EventArgs e)
        {
            // Resend code by calling btnSend_Click logic
            btnSend_Click(sender, e);
        }
    }
}

