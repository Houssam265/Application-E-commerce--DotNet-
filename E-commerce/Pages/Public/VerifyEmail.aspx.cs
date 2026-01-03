using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using Ecommerce.Data;
using Ecommerce.Utils;
using System.Collections.Generic;

namespace Ecommerce.Pages.Public
{
    public partial class VerifyEmail : Page
    {
        protected global::System.Web.UI.WebControls.Label lblEmail;
        protected global::System.Web.UI.WebControls.TextBox txtCode;
        protected global::System.Web.UI.WebControls.Button btnVerify;
        protected global::System.Web.UI.WebControls.Button btnResend;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Panel pnlSuccess;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.Literal litSuccess;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string email = Request.QueryString["email"];
                if (string.IsNullOrEmpty(email))
                {
                    ShowError("Email manquant.");
                    return;
                }
                lblEmail.Text = Server.HtmlEncode(email);
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            string email = Request.QueryString["email"];
            string code = txtCode.Text.Trim();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                ShowError("Veuillez saisir le code envoyé par email.");
                return;
            }

            try
            {
                DbContext db = new DbContext();
                string query = "SELECT Id, EmailVerificationCode, EmailVerificationExpiry, EmailVerified FROM Users WHERE Email = @Email";
                SqlParameter[] parameters = { new SqlParameter("@Email", email) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count == 0)
                {
                    ShowError("Utilisateur introuvable.");
                    return;
                }

                DataRow row = dt.Rows[0];
                
                if (Convert.ToBoolean(row["EmailVerified"]))
                {
                    // Already verified
                     Response.Redirect("~/Pages/Public/Login.aspx?verified=1&email=" + Server.UrlEncode(email));
                     return;
                }

                string storedCode = row["EmailVerificationCode"] != DBNull.Value ? row["EmailVerificationCode"].ToString() : null;
                DateTime expiry = row["EmailVerificationExpiry"] != DBNull.Value ? Convert.ToDateTime(row["EmailVerificationExpiry"]) : DateTime.MinValue;

                if (string.IsNullOrEmpty(storedCode))
                {
                    ShowError("Aucun code de vérification en attente.");
                    return;
                }
                
                if (expiry < DateTime.Now)
                {
                    ShowError("Le code a expiré. Veuillez renvoyer un code.");
                    return;
                }

                if (!string.Equals(storedCode, code, StringComparison.OrdinalIgnoreCase))
                {
                    ShowError("Code incorrect.");
                    return;
                }

                // Verify User
                string updateQuery = "UPDATE Users SET EmailVerified = 1, EmailVerificationCode = NULL, EmailVerificationExpiry = NULL WHERE Email = @Email";
                db.ExecuteNonQuery(updateQuery, new SqlParameter[] { new SqlParameter("@Email", email) });

                // Rediriger vers la page de connexion
                Response.Redirect("~/Pages/Public/Login.aspx?verified=1&email=" + Server.UrlEncode(email));
            }
            catch (Exception ex)
            {
                ShowError("Erreur: " + Server.HtmlEncode(ex.Message));
            }
        }

        protected void btnResend_Click(object sender, EventArgs e)
        {
            string email = Request.QueryString["email"];
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Email manquant.");
                return;
            }
            try
            {
                DbContext db = new DbContext();
                string query = "SELECT FullName FROM Users WHERE Email = @Email";
                DataTable dt = db.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@Email", email) });

                if (dt.Rows.Count == 0)
                {
                    ShowError("Utilisateur introuvable.");
                    return;
                }

                string fullName = dt.Rows[0]["FullName"].ToString();
                string code = SecurityHelper.GenerateNumericCode(6);
                DateTime expiry = DateTime.Now.AddMinutes(15);

                string updateQuery = "UPDATE Users SET EmailVerificationCode = @Code, EmailVerificationExpiry = @Expiry WHERE Email = @Email";
                SqlParameter[] updateParams = {
                    new SqlParameter("@Code", code),
                    new SqlParameter("@Expiry", expiry),
                    new SqlParameter("@Email", email)
                };
                db.ExecuteNonQuery(updateQuery, updateParams);

                string subject = "Nouveau code de vérification";
                string verifyUrl = ResolveUrl("~/Pages/Public/VerifyEmail.aspx?email=" + Server.UrlEncode(email));
                string body = EmailTemplates.GetEmailVerificationTemplate(fullName, code, verifyUrl);
                SecurityHelper.SendEmail(email, subject, body);

                litSuccess.Text = "Un nouveau code vous a été envoyé.";
                pnlSuccess.Visible = true;
                pnlError.Visible = false;
            }
            catch (Exception ex)
            {
                ShowError("Erreur: " + Server.HtmlEncode(ex.Message));
            }
        }

        private void ShowError(string message)
        {
            litError.Text = message;
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
        }
    }
}