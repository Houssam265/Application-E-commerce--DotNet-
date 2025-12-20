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

        // Méthode helper pour obtenir la classe CSS complète du bouton
        protected string GetFullButtonClass(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "btn btn-secondary" : "btn btn-primary";
        }

        // Méthode helper pour obtenir l'icône du bouton
        protected string GetButtonIcon(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "ban" : "check";
        }

        // Méthode helper pour obtenir la classe CSS du statut
        protected string GetStatusClass(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "active" : "inactive";
        }

        // Méthode helper pour obtenir le texte du statut
        protected string GetStatusText(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "Actif" : "Inactif";
        }

        // Méthode helper pour obtenir le texte du bouton toggle
        protected string GetToggleText(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "Désactiver" : "Activer";
        }
    }
}


