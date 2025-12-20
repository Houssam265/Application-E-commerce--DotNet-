using System;
using System.Text;
using System.Web;

namespace Ecommerce
{
    public class Global : HttpApplication
    {
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Configurer l'encodage UTF-8 pour toutes les requêtes
            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "utf-8";
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            // Configuration au démarrage de l'application
        }
    }
}

