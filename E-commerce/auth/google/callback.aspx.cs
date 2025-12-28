using System;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Data;
using System.Data.SqlClient;
using Ecommerce.Data;
using Ecommerce.Utils;

namespace Ecommerce.Auth.Google
{
    public partial class Callback : Page
    {
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litError;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            string expectedState = Convert.ToString(Session["GoogleOAuthState"]);
            string state = Request.QueryString["state"];
            string code = Request.QueryString["code"];

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state) || !string.Equals(expectedState, state, StringComparison.Ordinal))
            {
                ShowError("Flux OAuth invalide ou annulé.");
                return;
            }

            try
            {
                var clientId = ConfigurationManager.AppSettings["GOOGLE_CLIENT_ID"];
                var clientSecret = ConfigurationManager.AppSettings["GOOGLE_CLIENT_SECRET"];
                var redirectUri = ConfigurationManager.AppSettings["GOOGLE_REDIRECT_URL"];

                using (var wc = new WebClient())
                {
                    var data = new System.Collections.Specialized.NameValueCollection();
                    data["client_id"] = clientId;
                    data["client_secret"] = clientSecret;
                    data["code"] = code;
                    data["redirect_uri"] = redirectUri;
                    data["grant_type"] = "authorization_code";

                    byte[] resp = wc.UploadValues("https://oauth2.googleapis.com/token", "POST", data);
                    string json = System.Text.Encoding.UTF8.GetString(resp);

                    var jss = new JavaScriptSerializer();
                    var tokenObj = jss.Deserialize<System.Collections.Generic.Dictionary<string, object>>(json);
                    if (!tokenObj.ContainsKey("access_token"))
                    {
                        ShowError("Impossible d'obtenir le jeton d'accès Google.");
                        return;
                    }

                    string accessToken = Convert.ToString(tokenObj["access_token"]);
                    wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                    string userinfoJson = wc.DownloadString("https://www.googleapis.com/oauth2/v3/userinfo");
                    var userinfo = jss.Deserialize<System.Collections.Generic.Dictionary<string, object>>(userinfoJson);

                    string email = userinfo.ContainsKey("email") ? Convert.ToString(userinfo["email"]) : null;
                    string name = userinfo.ContainsKey("name") ? Convert.ToString(userinfo["name"]) : null;

                    if (string.IsNullOrEmpty(email))
                    {
                        ShowError("Email Google introuvable.");
                        return;
                    }

                    var db = new DbContext();
                    var dt = db.ExecuteQuery("SELECT TOP 1 * FROM Users WHERE Email = @Email", new[]
                    {
                        new SqlParameter("@Email", email)
                    });

                    int userId;
                    string role = "Customer";

                    if (dt.Rows.Count == 0)
                    {
                        string passHash = SecurityHelper.HashPassword(SecurityHelper.GenerateSecureToken());
                        string insertQuery = "INSERT INTO Users (Email, PasswordHash, FullName, Role, EmailVerified) VALUES (@Email, @PasswordHash, @FullName, 'Customer', 1)";
                        SqlParameter[] insertParams = {
                            new SqlParameter("@Email", email),
                            new SqlParameter("@PasswordHash", passHash),
                            new SqlParameter("@FullName", string.IsNullOrWhiteSpace(name) ? (object)DBNull.Value : name)
                        };
                        db.ExecuteNonQuery(insertQuery, insertParams);

                        var idDt = db.ExecuteQuery("SELECT TOP 1 Id, FullName, Role FROM Users WHERE Email = @Email", new[] { new SqlParameter("@Email", email) });
                        userId = Convert.ToInt32(idDt.Rows[0]["Id"]);
                        name = Convert.ToString(idDt.Rows[0]["FullName"]);
                        role = Convert.ToString(idDt.Rows[0]["Role"]);
                    }
                    else
                    {
                        userId = Convert.ToInt32(dt.Rows[0]["Id"]);
                        role = Convert.ToString(dt.Rows[0]["Role"]);

                        if (dt.Rows[0]["EmailVerified"] == DBNull.Value || Convert.ToInt32(dt.Rows[0]["EmailVerified"]) != 1)
                        {
                            // Marquer vérifié sans toucher à des colonnes inexistantes
                            db.ExecuteNonQuery("UPDATE Users SET EmailVerified = 1 WHERE Id = @Id", new[]
                            {
                                new SqlParameter("@Id", userId)
                            });
                        }

                        if (string.IsNullOrWhiteSpace(Convert.ToString(dt.Rows[0]["FullName"])) && !string.IsNullOrWhiteSpace(name))
                        {
                            db.ExecuteNonQuery("UPDATE Users SET FullName = @FullName WHERE Id = @Id", new[]
                            {
                                new SqlParameter("@FullName", name),
                                new SqlParameter("@Id", userId)
                            });
                        }
                    }

                    // Set session and redirect
                    Session["UserId"] = userId;
                    Session["UserEmail"] = email;
                    Session["FullName"] = string.IsNullOrWhiteSpace(name) ? email : name;
                    Session["Role"] = string.IsNullOrWhiteSpace(role) ? "Customer" : role;

                    if (string.Equals(Session["Role"].ToString(), "Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        Response.Redirect("~/Pages/Admin/Dashboard.aspx");
                    }
                    else
                    {
                        Response.Redirect("~/Default.aspx");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Erreur: " + Server.HtmlEncode(ex.Message));
            }
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            litError.Text = message;
        }
    }
}