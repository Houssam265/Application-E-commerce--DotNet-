using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;
using Ecommerce.Utils;

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

                string insertQuery = "INSERT INTO Users (Email, PasswordHash, FullName, Phone, Role) VALUES (@Email, @PasswordHash, @FullName, @Phone, 'Customer')";
                SqlParameter[] insertParams = {
                    new SqlParameter("@Email", email),
                    new SqlParameter("@PasswordHash", passwordHash),
                    new SqlParameter("@FullName", fullName),
                    new SqlParameter("@Phone", phone ?? (object)DBNull.Value)
                };

                db.ExecuteNonQuery(insertQuery, insertParams);

                Response.Redirect("Login.aspx");
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
