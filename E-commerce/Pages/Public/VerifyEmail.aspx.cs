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
                // Lire les données d'inscription en attente depuis la session
                var pending = Session["PendingReg:" + email] as Dictionary<string, string>;
                if (pending == null)
                {
                    ShowError("Aucune inscription en attente trouvée pour cet email.");
                    return;
                }

                // Valider le code et l'expiration
                string storedCode = pending.ContainsKey("Code") ? pending["Code"] : null;
                DateTime expiry;
                if (!pending.ContainsKey("Expiry") || !DateTime.TryParse(pending["Expiry"], out expiry))
                {
                    ShowError("Données de vérification invalides.");
                    return;
                }

                if (string.IsNullOrEmpty(storedCode))
                {
                    ShowError("Aucun code n'est associé à cette inscription.");
                    return;
                }
                if (expiry < DateTime.Now)
                {
                    ShowError("Le code a expiré. Veuillez renvoyer un code.");
                    return;
                }
                if (!string.Equals(storedCode, code, StringComparison.Ordinal))
                {
                    ShowError("Code incorrect.");
                    return;
                }

                // Insérer le compte uniquement après la vérification réussie
                DbContext db = new DbContext();
                string insertQuery = "INSERT INTO Users (Email, PasswordHash, FullName, Phone, Role, EmailVerified) VALUES (@Email, @PasswordHash, @FullName, @Phone, 'Customer', 1)";
                SqlParameter[] insertParams = {
                    new SqlParameter("@Email", pending["Email"]),
                    new SqlParameter("@PasswordHash", pending["PasswordHash"]),
                    new SqlParameter("@FullName", pending["FullName"]),
                    new SqlParameter("@Phone", string.IsNullOrWhiteSpace(pending["Phone"]) ? (object)DBNull.Value : pending["Phone"]) 
                };
                db.ExecuteNonQuery(insertQuery, insertParams);

                // Nettoyer les données en session
                Session.Remove("PendingReg:" + email);

                // Rediriger vers la page de connexion avec message de succès et email pré-rempli
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
                // Regénérer un code en session (pas en base)
                var pending = Session["PendingReg:" + email] as Dictionary<string, string>;
                if (pending == null)
                {
                    ShowError("Aucune inscription en attente trouvée pour cet email.");
                    return;
                }

                string code = SecurityHelper.GenerateNumericCode(6);
                DateTime expiry = DateTime.Now.AddMinutes(15);

                pending["Code"] = code;
                pending["Expiry"] = expiry.ToString("o");
                Session["PendingReg:" + email] = pending; // réécrire

                string subject = "Nouveau code de vérification";
                string body = $"<p>Votre nouveau code: <strong>{code}</strong></p><p>Ce code expire dans 15 minutes.</p>";
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