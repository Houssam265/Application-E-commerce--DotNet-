using System;
using System.Web;
using System.Web.UI;
using System.Configuration;

namespace Ecommerce.Auth.Google
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var clientId = ConfigurationManager.AppSettings["GOOGLE_CLIENT_ID"];
            var redirectUri = ConfigurationManager.AppSettings["GOOGLE_REDIRECT_URL"];
            var scope = "openid email profile";
            var state = Guid.NewGuid().ToString("N");
            Session["GoogleOAuthState"] = state;

            string url = "https://accounts.google.com/o/oauth2/v2/auth" +
                         "?client_id=" + Uri.EscapeDataString(clientId) +
                         "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                         "&response_type=code" +
                         "&scope=" + Uri.EscapeDataString(scope) +
                         "&access_type=online&include_granted_scopes=true" +
                         "&state=" + state;

            Response.Redirect(url);
        }
    }
}