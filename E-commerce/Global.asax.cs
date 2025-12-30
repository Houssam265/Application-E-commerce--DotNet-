using System;
using System.Text;
using System.Web;
using Ecommerce.Data;
using System.Data.SqlClient;

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
            EnsureEmailVerificationSchema();
        }

        private void EnsureEmailVerificationSchema()
        {
            try
            {
                var db = new DbContext();

                // EmailVerified
                var objVerified = db.ExecuteScalar("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'EmailVerified'");
                int existsVerified = (objVerified == null || objVerified == DBNull.Value) ? 0 : Convert.ToInt32(objVerified);
                if (existsVerified == 0)
                {
                    db.ExecuteNonQuery("ALTER TABLE Users ADD EmailVerified BIT NULL");
                    db.ExecuteNonQuery("UPDATE Users SET EmailVerified = ISNULL(EmailVerified, 0)");
                }
            }
            catch (Exception)
            {
                // Ne pas bloquer le démarrage de l'application en cas d'erreur de schéma
            }
        }
    }
}

