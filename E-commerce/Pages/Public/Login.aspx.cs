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
    public partial class Login : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.TextBox txtEmail;
        protected global::System.Web.UI.WebControls.TextBox txtPassword;
        protected global::System.Web.UI.WebControls.Button btnLogin;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserEmail"] != null)
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowError("Veuillez remplir tous les champs.");
                return;
            }

            try
            {
                DbContext db = new DbContext();
                string query = "SELECT * FROM Users WHERE Email = @Email";
                SqlParameter[] parameters = {
                    new SqlParameter("@Email", email)
                };

                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string storedHash = row["PasswordHash"].ToString();

                    if (SecurityHelper.VerifyPassword(password, storedHash))
                    {
                        Session["UserId"] = row["Id"];
                        Session["UserEmail"] = row["Email"];
                        Session["FullName"] = row["FullName"];
                        Session["Role"] = row["Role"];
                        
                        if (row["Role"].ToString() == "Admin")
                        {
                            Response.Redirect("~/Pages/Admin/Dashboard.aspx");
                        }
                        else
                        {
                            Response.Redirect("~/Default.aspx");
                        }
                    }
                    else
                    {
                        ShowError("Mot de passe incorrect.");
                    }
                }
                else
                {
                    ShowError("Aucun compte trouvé avec cet email.");
                }
            }
            catch (Exception ex)
            {
                ShowError("Une erreur est survenue: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            litError.Text = message;
            pnlError.Visible = true;
        }
    }
}
