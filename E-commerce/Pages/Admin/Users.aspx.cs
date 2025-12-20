using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;

namespace Ecommerce.Pages.Admin
{
    public partial class Users : Page
    {
        protected global::System.Web.UI.WebControls.GridView gvUsers;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            DbContext db = new DbContext();
            DataTable dt = db.ExecuteQuery("SELECT Id, FullName, Email, Role, CreatedAt, IsActive FROM Users ORDER BY CreatedAt DESC");
            gvUsers.DataSource = dt;
            gvUsers.DataBind();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleActive")
            {
                string userId = e.CommandArgument.ToString();
                DbContext db = new DbContext();
                
                // Get current status
                string checkQuery = "SELECT IsActive FROM Users WHERE Id = @Id";
                SqlParameter[] checkParams = { new SqlParameter("@Id", userId) };
                object currentStatus = db.ExecuteScalar(checkQuery, checkParams);
                
                bool newStatus = !Convert.ToBoolean(currentStatus);
                
                // Update status
                string updateQuery = "UPDATE Users SET IsActive = @IsActive WHERE Id = @Id";
                SqlParameter[] updateParams = {
                    new SqlParameter("@IsActive", newStatus),
                    new SqlParameter("@Id", userId)
                };
                db.ExecuteNonQuery(updateQuery, updateParams);
                
                LoadUsers();
            }
        }

        protected string GetActiveStatus(object isActive)
        {
            if (isActive != null && isActive != DBNull.Value)
            {
                bool active = Convert.ToBoolean(isActive);
                return active ? "<span style='color:#10b981'>Oui</span>" : "<span style='color:#ef4444'>Non</span>";
            }
            return "<span style='color:#ef4444'>Non</span>";
        }

        protected string GetToggleButtonText(object isActive)
        {
            if (isActive != null && isActive != DBNull.Value)
            {
                bool active = Convert.ToBoolean(isActive);
                return active ? "<span style='background: rgba(220,53,69,0.1); color: var(--danger-color); padding: 5px 10px; border-radius: 5px;'>Désactiver</span>" 
                             : "<span style='background: rgba(16,185,129,0.1); color: var(--success-color); padding: 5px 10px; border-radius: 5px;'>Activer</span>";
            }
            return "<span style='background: rgba(16,185,129,0.1); color: var(--success-color); padding: 5px 10px; border-radius: 5px;'>Activer</span>";
        }

        protected string GetConfirmMessage(object isActive)
        {
            if (isActive != null && isActive != DBNull.Value)
            {
                bool active = Convert.ToBoolean(isActive);
                string action = active ? "désactiver" : "activer";
                return "return confirm('Êtes-vous sûr de vouloir " + action + " cet utilisateur ?');";
            }
            return "return confirm('Êtes-vous sûr de vouloir activer cet utilisateur ?');";
        }
    }
}


