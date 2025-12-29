using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;
using Ecommerce.Utils;

namespace Ecommerce.Pages.Admin
{
    public partial class ComplaintsManagement : Page
    {
        // Controls (defined in ASPX)
        protected global::System.Web.UI.WebControls.DropDownList ddlStatusFilter;
        protected global::System.Web.UI.WebControls.DropDownList ddlPriorityFilter;
        protected global::System.Web.UI.WebControls.TextBox txtSearch;
        protected global::System.Web.UI.WebControls.LinkButton btnSearch;
        protected global::System.Web.UI.WebControls.Panel pnlList;
        protected global::System.Web.UI.WebControls.Repeater rptComplaints;
        protected global::System.Web.UI.WebControls.Panel pnlNoComplaints;
        protected global::System.Web.UI.WebControls.Panel pnlDetails;
        protected global::System.Web.UI.WebControls.LinkButton btnBack;
        protected global::System.Web.UI.WebControls.Label lblUserName;
        protected global::System.Web.UI.WebControls.Label lblUserEmail;
        protected global::System.Web.UI.WebControls.Label lblCreatedDate;
        protected global::System.Web.UI.WebControls.Label lblOrderNumber;
        protected global::System.Web.UI.WebControls.Label lblPriority;
        protected global::System.Web.UI.WebControls.Label lblCategory;
        protected global::System.Web.UI.WebControls.Label lblSubject;
        protected global::System.Web.UI.WebControls.Label lblDescription;
        protected global::System.Web.UI.WebControls.Panel pnlExistingResponse;
        protected global::System.Web.UI.WebControls.Label lblResponseDate;
        protected global::System.Web.UI.WebControls.Label lblExistingResponse;
        protected global::System.Web.UI.WebControls.DropDownList ddlStatus;
        protected global::System.Web.UI.WebControls.TextBox txtResponse;
        protected global::System.Web.UI.WebControls.CheckBox chkSendEmail;
        protected global::System.Web.UI.WebControls.Button btnSubmitResponse;
        protected global::System.Web.UI.WebControls.Panel pnlResponseSuccess;
        protected global::System.Web.UI.WebControls.Literal litSuccess;
        protected global::System.Web.UI.WebControls.Panel pnlResponseError;
        protected global::System.Web.UI.WebControls.Literal litError;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComplaints();
            }
        }

        private void LoadComplaints()
        {
            DbContext db = new DbContext();
            
            string query = @"
                SELECT C.Id, C.Subject, C.Description, C.Status, C.Priority, C.Category, C.CreatedAt,
                       U.FullName, U.Email, O.OrderNumber
                FROM Complaints C
                INNER JOIN Users U ON C.UserId = U.Id
                LEFT JOIN Orders O ON C.OrderId = O.Id
                WHERE 1=1";
            
            System.Collections.Generic.List<SqlParameter> parameters = new System.Collections.Generic.List<SqlParameter>();
            
            // Apply filters
            if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
            {
                query += " AND C.Status = @Status";
                parameters.Add(new SqlParameter("@Status", ddlStatusFilter.SelectedValue));
            }

if (!string.IsNullOrEmpty(ddlPriorityFilter.SelectedValue))
            {
                query += " AND C.Priority = @Priority";
                parameters.Add(new SqlParameter("@Priority", ddlPriorityFilter.SelectedValue));
            }
            
            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                query += " AND (C.Subject LIKE @Search OR C.Description LIKE @Search OR U.FullName LIKE @Search OR U.Email LIKE @Search OR O.OrderNumber LIKE @Search)";
                parameters.Add(new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%"));
            }
            
            query += " ORDER BY C.CreatedAt DESC";
            
            DataTable dt = db.ExecuteQuery(query, parameters.ToArray());
            
            if (dt.Rows.Count > 0)
            {
                rptComplaints.DataSource = dt;
                rptComplaints.DataBind();
                pnlList.Visible = true;
                pnlNoComplaints.Visible = false;
            }
            else
            {
                rptComplaints.DataSource = null;
                rptComplaints.DataBind();
                pnlList.Visible = false;
                pnlNoComplaints.Visible = true;
            }
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadComplaints();
        }

        protected void ddlPriorityFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadComplaints();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadComplaints();
        }

        protected void rptComplaints_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewComplaint")
            {
                string complaintId = e.CommandArgument.ToString();
                LoadComplaintDetails(complaintId);
            }
        }

        private void LoadComplaintDetails(string complaintId)
        {
            DbContext db = new DbContext();
            
            string query = @"
                SELECT C.*, U.FullName, U.Email, O.OrderNumber
                FROM Complaints C
                INNER JOIN Users U ON C.UserId = U.Id
                LEFT JOIN Orders O ON C.OrderId = O.Id
                WHERE C.Id = @Id";
            
            DataTable dt = db.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@Id", complaintId) });
            
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                
                // Store complaint ID in ViewState
                ViewState["ComplaintId"] = complaintId;
                
                // Fill fields
                lblUserName.Text = Server.HtmlEncode(row["FullName"].ToString());
                lblUserEmail.Text = Server.HtmlEncode(row["Email"].ToString());
                lblCreatedDate.Text = Convert.ToDateTime(row["CreatedAt"]).ToString("dd/MM/yyyy à HH:mm");
                lblOrderNumber.Text = row["OrderNumber"] != DBNull.Value ? Server.HtmlEncode(row["OrderNumber"].ToString()) : "Aucune";
                lblPriority.Text = GetPriorityLabel(row["Priority"].ToString());
                lblCategory.Text = row["Category"] != DBNull.Value ? Server.HtmlEncode(row["Category"].ToString()) : "Non spécifié";
                lblSubject.Text = Server.HtmlEncode(row["Subject"].ToString());
                lblDescription.Text = Server.HtmlEncode(row["Description"].ToString()).Replace("\n", "<br/>");
                
                string currentStatus = row["Status"].ToString();
                ddlStatus.SelectedValue = currentStatus;
                
                // Lock form if complaint is Closed or Resolved
                bool isLocked = (currentStatus == "Closed" || currentStatus == "Resolved");
                ddlStatus.Enabled = !isLocked;
                txtResponse.Enabled = !isLocked;
                btnSubmitResponse.Enabled = !isLocked;
                chkSendEmail.Enabled = !isLocked;
                
                if (isLocked)
                {
                    litError.Text = "Cette réclamation est fermée et ne peut plus être modifiée.";
                    pnlResponseError.Visible = true;
                }
                
                // Show existing response if any
                if (row["AdminResponse"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["AdminResponse"].ToString()))
                {
                    pnlExistingResponse.Visible = true;
                    lblExistingResponse.Text = Server.HtmlEncode(row["AdminResponse"].ToString()).Replace("\n", "<br/>");
                    if (row["UpdatedAt"] != DBNull.Value)
                    {
                        lblResponseDate.Text = Convert.ToDateTime(row["UpdatedAt"]).ToString("dd/MM/yyyy à HH:mm");
                    }
                }
                else
                {
                    pnlExistingResponse.Visible = false;
                }
                
                pnlList.Visible = false;
                pnlDetails.Visible = true;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            pnlList.Visible = true;
            LoadComplaints();
        }

        protected void btnSubmitResponse_Click(object sender, EventArgs e)
        {
            try
            {
                string complaintId = ViewState["ComplaintId"]?.ToString();
                if (string.IsNullOrEmpty(complaintId))
                {
                    litError.Text = "Erreur: ID de réclamation introuvable.";
                    pnlResponseError.Visible = true;
                    return;
                }

                string response = txtResponse.Text.Trim();
                string status = ddlStatus.SelectedValue;
                int adminId = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : 0;
                
                DbContext db = new DbContext();
                
                // Check if complaint is already closed
                string checkQuery = "SELECT Status FROM Complaints WHERE Id = @Id";
                DataTable checkDt = db.ExecuteQuery(checkQuery, new SqlParameter[] { new SqlParameter("@Id", complaintId) });
                if (checkDt.Rows.Count > 0 && (checkDt.Rows[0]["Status"].ToString() == "Closed" || checkDt.Rows[0]["Status"].ToString() == "Resolved"))
                {
                    litError.Text = "Cette réclamation est fermée et ne peut plus être modifiée.";
                    pnlResponseError.Visible = true;
                    return;
                }
                
                // If status is Resolved, automatically close it
                string finalStatus = status;
                if (status == "Resolved")
                {
                    finalStatus = "Closed";
                }
                
                // Get user ID, email and subject for notification and email
                string getUserQuery = @"
                    SELECT C.UserId, C.Subject, U.FullName, U.Email
                    FROM Complaints C
                    INNER JOIN Users U ON C.UserId = U.Id
                    WHERE C.Id = @Id";
                DataTable userDt = db.ExecuteQuery(getUserQuery, new SqlParameter[] { new SqlParameter("@Id", complaintId) });
                int userId = 0;
                string complaintSubject = "";
                string userEmail = "";
                string userName = "";
                if (userDt.Rows.Count > 0)
                {
                    userId = Convert.ToInt32(userDt.Rows[0]["UserId"]);
                    complaintSubject = userDt.Rows[0]["Subject"].ToString();
                    userEmail = userDt.Rows[0]["Email"].ToString();
                    userName = userDt.Rows[0]["FullName"].ToString();
                }
                
                // Update complaint
                string updateQuery = @"
                    UPDATE Complaints 
                    SET Status = @Status, 
                        AdminResponse = @Response, 
                        AdminId = @AdminId, 
                        UpdatedAt = GETDATE(),
                        ResolvedAt = CASE WHEN @Status = 'Closed' THEN GETDATE() ELSE ResolvedAt END
                    WHERE Id = @Id";
                
                SqlParameter[] updateParams = {
                    new SqlParameter("@Status", finalStatus),
                    new SqlParameter("@Response", string.IsNullOrWhiteSpace(response) ? (object)DBNull.Value : response),
                    new SqlParameter("@AdminId", adminId),
                    new SqlParameter("@Id", complaintId)
                };
                
                db.ExecuteNonQuery(updateQuery, updateParams);
                
                // Create notification for user
                if (userId > 0 && !string.IsNullOrWhiteSpace(response))
                {
                    string notificationTitle = "Réponse à votre réclamation: " + complaintSubject;
                    string notificationMessage = "ComplaintId=" + complaintId + ";Response=" + Server.UrlEncode(response);
                    db.ExecuteNonQuery(@"INSERT INTO Notifications (UserId, Title, Message, Type) 
                                         VALUES (@UserId, @Title, @Message, 'Complaint')",
                                         new SqlParameter[] {
                                             new SqlParameter("@UserId", userId),
                                             new SqlParameter("@Title", notificationTitle),
                                             new SqlParameter("@Message", notificationMessage)
                                         });
                }
                
                // Send email if requested and response provided
                if (chkSendEmail.Checked && !string.IsNullOrWhiteSpace(response) && !string.IsNullOrEmpty(userEmail))
                {
                    string emailSubject = "Réponse à votre réclamation: " + complaintSubject;
                    string emailBody = EmailTemplates.GetComplaintResponseEmailTemplate(userName, complaintSubject, response);
                    
                    SecurityHelper.SendEmail(userEmail, emailSubject, emailBody);
                }
                
                litSuccess.Text = "Réponse enregistrée avec succès" + (chkSendEmail.Checked && !string.IsNullOrWhiteSpace(response) ? " et email envoyé." : ".");
                pnlResponseSuccess.Visible = true;
                pnlResponseError.Visible = false;
                
                // Reload complaint details
                System.Threading.Thread.Sleep(1000);
                LoadComplaintDetails(complaintId);
            }
            catch (Exception ex)
            {
                litError.Text = "Erreur: " + Server.HtmlEncode(ex.Message);
                pnlResponseError.Visible = true;
                pnlResponseSuccess.Visible = false;
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

