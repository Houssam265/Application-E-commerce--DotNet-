using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using Ecommerce.Data;
using Ecommerce.Utils;

namespace Ecommerce.Pages.Public
{
    public partial class ResetPassword : Page
    {
        protected global::System.Web.UI.WebControls.Panel pnlForm;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Panel pnlSuccess;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.Literal litSuccess;
        protected global::System.Web.UI.WebControls.TextBox txtNewPassword;
        protected global::System.Web.UI.WebControls.TextBox txtConfirmPassword;
        protected global::System.Web.UI.WebControls.Button btnReset;

        protected void Page_Load(object sender, EventArgs e)
        {
            string token = Request.QueryString["token"];
            if (string.IsNullOrEmpty(token))
            {
                pnlForm.Visible = false;
                litError.Text = "Token de réinitialisation invalide ou manquant.";
                pnlError.Visible = true;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                string token = Request.QueryString["token"];
                if (string.IsNullOrEmpty(token))
                {
                    litError.Text = "Token invalide.";
                    pnlError.Visible = true;
                    return;
                }

                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
                {
                    litError.Text = "Le mot de passe doit contenir au moins 6 caractères.";
                    pnlError.Visible = true;
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    litError.Text = "Les mots de passe ne correspondent pas.";
                    pnlError.Visible = true;
                    return;
                }

                DbContext db = new DbContext();
                
                // Verify token
                string verifyQuery = "SELECT Id FROM Users WHERE ResetPasswordToken = @Token AND ResetPasswordExpiry > GETDATE()";
                SqlParameter[] verifyParams = { new SqlParameter("@Token", token) };
                object userId = db.ExecuteScalar(verifyQuery, verifyParams);

                if (userId == null || userId == DBNull.Value)
                {
                    litError.Text = "Token invalide ou expiré. Veuillez demander un nouveau lien de réinitialisation.";
                    pnlError.Visible = true;
                    return;
                }

                // Update password
                string passwordHash = SecurityHelper.HashPassword(newPassword);
                string updateQuery = @"UPDATE Users SET PasswordHash = @PasswordHash, 
                                      ResetPasswordToken = NULL, ResetPasswordExpiry = NULL 
                                      WHERE Id = @UserId";
                SqlParameter[] updateParams = {
                    new SqlParameter("@PasswordHash", passwordHash),
                    new SqlParameter("@UserId", userId)
                };
                db.ExecuteNonQuery(updateQuery, updateParams);

                pnlForm.Visible = false;
                litSuccess.Text = "Votre mot de passe a été réinitialisé avec succès ! Vous pouvez maintenant vous connecter.";
                pnlSuccess.Visible = true;
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur: " + Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
            }
        }
    }
}

