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
                
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                SqlParameter[] checkParams = { new SqlParameter("@Email", email) };
                int count = (int)db.ExecuteScalar(checkQuery, checkParams);

                if (count > 0)
                {
                    ShowError("Cet email est déjà utilisé.");
                    return;
                }

                string passwordHash = SecurityHelper.HashPassword(password);

                // Ne pas insérer en base avant la vérification
                // Générer le code et le stocker en session
                string code = SecurityHelper.GenerateNumericCode(6);
                DateTime expiry = DateTime.Now.AddMinutes(15);

                var pending = new Dictionary<string, string>
                {
                    { "FullName", fullName },
                    { "Email", email },
                    { "Phone", string.IsNullOrWhiteSpace(phone) ? "" : phone },
                    { "PasswordHash", passwordHash },
                    { "Code", code },
                    { "Expiry", expiry.ToString("o") }
                };
                Session["PendingReg:" + email] = pending;

                // Envoyer l'email de vérification en utilisant EmailTemplates
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
