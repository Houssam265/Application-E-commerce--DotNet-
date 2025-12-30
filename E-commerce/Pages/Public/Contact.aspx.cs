using System;
using System.Web.UI;
using Ecommerce.Utils;

namespace Ecommerce.Pages.Public
{
    public partial class Contact : Page
    {
        protected global::System.Web.UI.WebControls.TextBox txtName;
        protected global::System.Web.UI.WebControls.TextBox txtEmail;
        protected global::System.Web.UI.WebControls.TextBox txtSubject;
        protected global::System.Web.UI.WebControls.TextBox txtMessage;
        protected global::System.Web.UI.WebControls.Button btnSend;
        protected global::System.Web.UI.WebControls.Panel pnlSuccess;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litSuccess;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.Panel pnlEmailField;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // If user is logged in, hide the email field and prefill name
                if (Session["UserId"] != null)
                {
                    if (pnlEmailField != null) pnlEmailField.Visible = false;
                    if (Session["FullName"] != null) txtName.Text = Session["FullName"].ToString();
                }
            }
        }

        private void EnsureContactTables()
        {
            try
            {
                var db = new Ecommerce.Data.DbContext();
                string createTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContactMessages]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[ContactMessages](
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [UserId] INT NULL,
                            [SenderName] NVARCHAR(200) NOT NULL,
                            [SenderEmail] NVARCHAR(255) NOT NULL,
                            [Subject] NVARCHAR(200) NOT NULL,
                            [Message] NVARCHAR(MAX) NOT NULL,
                            [CreatedAt] DATETIME NOT NULL DEFAULT(GETDATE())
                        );
                    END";
                db.ExecuteNonQuery(createTable);
            }
            catch { }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureContactTables();
                var db = new Ecommerce.Data.DbContext();

                string name = SecurityHelper.SanitizeInput(txtName.Text.Trim());
                string subject = SecurityHelper.SanitizeInput(txtSubject.Text.Trim());
                string message = SecurityHelper.SanitizeInput(txtMessage.Text.Trim());

                int? userId = null;
                string email;
                if (Session["UserId"] != null)
                {
                    userId = Convert.ToInt32(Session["UserId"]);
                    // get user email from DB
                    var dt = db.ExecuteQuery("SELECT Email, FullName FROM Users WHERE Id = @Id", new System.Data.SqlClient.SqlParameter[] { new System.Data.SqlClient.SqlParameter("@Id", userId.Value) });
                    if (dt.Rows.Count > 0)
                    {
                        email = dt.Rows[0]["Email"].ToString();
                        if (string.IsNullOrWhiteSpace(name)) name = dt.Rows[0]["FullName"].ToString();
                    }
                    else
                    {
                        throw new Exception("Utilisateur introuvable.");
                    }
                }
                else
                {
                    email = txtEmail.Text.Trim();
                    if (!SecurityHelper.IsValidEmail(email))
                    {
                        litError.Text = "Veuillez entrer une adresse email valide.";
                        pnlError.Visible = true;
                        return;
                    }
                }

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
                {
                    litError.Text = "Veuillez remplir tous les champs obligatoires.";
                    pnlError.Visible = true;
                    return;
                }

                // Save the message
                db.ExecuteNonQuery(@"INSERT INTO ContactMessages (UserId, SenderName, SenderEmail, Subject, Message) 
                                     VALUES (@UserId, @Name, @Email, @Subject, @Message)",
                    new System.Data.SqlClient.SqlParameter[] {
                        new System.Data.SqlClient.SqlParameter("@UserId", (object)userId ?? DBNull.Value),
                        new System.Data.SqlClient.SqlParameter("@Name", name),
                        new System.Data.SqlClient.SqlParameter("@Email", email),
                        new System.Data.SqlClient.SqlParameter("@Subject", subject),
                        new System.Data.SqlClient.SqlParameter("@Message", message)
                    });

                // Email admins
                var admins = db.ExecuteQuery("SELECT Email, FullName FROM Users WHERE Role = 'Admin' AND IsActive = 1");
                string htmlBody = EmailTemplates.GetContactMessageEmailTemplate(name, email, subject, message);
                
                foreach (System.Data.DataRow r in admins.Rows)
                {
                    string adminEmail = r["Email"].ToString();
                    SecurityHelper.SendEmail(adminEmail, "Contact: " + subject, htmlBody);
                }

                litSuccess.Text = "Merci pour votre message ! Nous vous répondrons dans les plus brefs délais.";
                pnlSuccess.Visible = true;

                // Clear form
                txtSubject.Text = "";
                txtMessage.Text = "";
                if (Session["UserId"] == null) txtEmail.Text = "";
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur lors de l'envoi: " + Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
            }
        }
    }
}

