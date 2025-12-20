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

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string name = SecurityHelper.SanitizeInput(txtName.Text.Trim());
                string email = txtEmail.Text.Trim();
                string subject = SecurityHelper.SanitizeInput(txtSubject.Text.Trim());
                string message = SecurityHelper.SanitizeInput(txtMessage.Text.Trim());

                if (!SecurityHelper.IsValidEmail(email))
                {
                    litError.Text = "Veuillez entrer une adresse email valide.";
                    pnlError.Visible = true;
                    return;
                }

                // In a real application, send email here
                // For demo, just show success message
                litSuccess.Text = "Merci pour votre message ! Nous vous répondrons dans les plus brefs délais.";
                pnlSuccess.Visible = true;
                
                // Clear form
                txtName.Text = "";
                txtEmail.Text = "";
                txtSubject.Text = "";
                txtMessage.Text = "";
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur lors de l'envoi: " + Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
            }
        }
    }
}

