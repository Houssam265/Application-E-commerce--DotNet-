using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;
using Ecommerce.Utils;

namespace Ecommerce.Pages.Public
{
    public partial class UserComplaints : Page
    {
        protected global::System.Web.UI.WebControls.Button btnNewComplaint;
        protected global::System.Web.UI.WebControls.Panel pnlNewComplaint;
        protected global::System.Web.UI.WebControls.DropDownList ddlOrder;
        protected global::System.Web.UI.WebControls.DropDownList ddlCategory;
        protected global::System.Web.UI.WebControls.TextBox txtSubject;
        protected global::System.Web.UI.WebControls.TextBox txtDescription;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.Button btnSubmit;
        protected global::System.Web.UI.WebControls.Button btnCancel;
        protected global::System.Web.UI.WebControls.Repeater rptComplaints;
        protected global::System.Web.UI.WebControls.Panel pnlNoComplaints;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            if (!IsPostBack)
            {
                LoadUserOrders();
                LoadComplaints();
            }
        }

        private void LoadUserOrders()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            DbContext db = new DbContext();
            
            string query = "SELECT Id, OrderNumber FROM Orders WHERE UserId = @UserId ORDER BY OrderDate DESC";
            DataTable dt = db.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@UserId", userId) });
            
            ddlOrder.DataSource = dt;
            ddlOrder.DataTextField = "OrderNumber";
            ddlOrder.DataValueField = "Id";
            ddlOrder.DataBind();
            ddlOrder.Items.Insert(0, new ListItem("-- Aucune commande spécifique --", ""));
        }

        private void LoadComplaints()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            DbContext db = new DbContext();
            
            string query = @"
                SELECT C.*, O.OrderNumber
                FROM Complaints C
                LEFT JOIN Orders O ON C.OrderId = O.Id
                WHERE C.UserId = @UserId
                ORDER BY C.CreatedAt DESC";
            
            DataTable dt = db.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@UserId", userId) });
            
            if (dt.Rows.Count > 0)
            {
                rptComplaints.DataSource = dt;
                rptComplaints.DataBind();
                pnlNoComplaints.Visible = false;
            }
            else
            {
                pnlNoComplaints.Visible = true;
            }
        }

        protected void btnNewComplaint_Click(object sender, EventArgs e)
        {
            pnlNewComplaint.Visible = true;
            txtSubject.Text = "";
            txtDescription.Text = "";
            pnlError.Visible = false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlNewComplaint.Visible = false;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string subject = txtSubject.Text.Trim();
                string description = txtDescription.Text.Trim();
                
                if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(description))
                {
                    litError.Text = "Veuillez remplir tous les champs obligatoires.";
                    pnlError.Visible = true;
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                int? orderId = null;
                if (!string.IsNullOrEmpty(ddlOrder.SelectedValue))
                {
                    orderId = Convert.ToInt32(ddlOrder.SelectedValue);
                }

                DbContext db = new DbContext();
                
                string insertQuery = @"
                    INSERT INTO Complaints (UserId, OrderId, Subject, Description, Category, Status, Priority)
                    VALUES (@UserId, @OrderId, @Subject, @Description, @Category, 'Pending', 'Medium')";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@OrderId", orderId ?? (object)DBNull.Value),
                    new SqlParameter("@Subject", Server.HtmlEncode(subject)),
                    new SqlParameter("@Description", Server.HtmlEncode(description)),
                    new SqlParameter("@Category", ddlCategory.SelectedValue)
                };
                
                db.ExecuteNonQuery(insertQuery, parameters);
                
                // Success
                pnlNewComplaint.Visible = false;
                LoadComplaints();
                
                // You could add a success message panel here
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur: " + Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
            }
        }

        protected string GetStatusLabel(string status)
        {
            switch (status)
            {
                case "Pending": return "En attente";
                case "InProgress": return "En cours";
                case "Resolved": return "Résolue";
                case "Closed": return "Fermée";
                default: return status;
            }
        }

        protected string GetPriorityLabel(string priority)
        {
            switch (priority)
            {
                case "High": return "Haute";
                case "Medium": return "Moyenne";
                case "Low": return "Basse";
                default: return priority;
            }
        }
    }
}
