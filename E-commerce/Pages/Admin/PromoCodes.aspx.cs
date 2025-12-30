using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;

namespace Ecommerce.Pages.Admin
{
    public partial class PromoCodes : Page
    {
        protected global::System.Web.UI.WebControls.LinkButton btnAddNew;
        protected global::System.Web.UI.WebControls.Panel pnlList;
        protected global::System.Web.UI.WebControls.GridView gvPromoCodes;
        protected global::System.Web.UI.WebControls.Panel pnlEdit;
        protected global::System.Web.UI.WebControls.Label lblTitle;
        protected global::System.Web.UI.WebControls.HiddenField hfCouponId;
        protected global::System.Web.UI.WebControls.TextBox txtCode;
        protected global::System.Web.UI.WebControls.DropDownList ddlDiscountType;
        protected global::System.Web.UI.WebControls.TextBox txtDiscountValue;
        protected global::System.Web.UI.WebControls.TextBox txtMinimumAmount;
        protected global::System.Web.UI.WebControls.TextBox txtMaximumDiscount;
        protected global::System.Web.UI.WebControls.TextBox txtStartDate;
        protected global::System.Web.UI.WebControls.TextBox txtEndDate;
        protected global::System.Web.UI.WebControls.TextBox txtUsageLimit;
        protected global::System.Web.UI.WebControls.DropDownList ddlIsActive;
        protected global::System.Web.UI.WebControls.LinkButton btnSave;
        protected global::System.Web.UI.WebControls.LinkButton btnCancel;
        protected global::System.Web.UI.WebControls.Label lblError;
        protected global::System.Web.UI.WebControls.Label lblSuccess;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvCode;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvDiscountValue;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvStartDate;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvEndDate;
        protected global::System.Web.UI.WebControls.Label lblDiscountHint;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl maxDiscountLabel;
        protected global::System.Web.UI.WebControls.TextBox txtSearch;
        protected global::System.Web.UI.WebControls.LinkButton btnSearch;
        protected global::System.Web.UI.WebControls.LinkButton btnClear;
        protected global::System.Web.UI.WebControls.DropDownList ddlStatusFilter;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPromoCodes("", "");
                SetDefaultDates();
            }
        }

        private void SetDefaultDates()
        {
            // Set default start date to now
            txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            // Set default end date to 30 days from now
            txtEndDate.Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-ddTHH:mm");
        }

        private void LoadPromoCodes(string searchTerm = "", string status = "")
        {
            try
            {
                DbContext db = new DbContext();
                string query = "SELECT * FROM Coupons WHERE 1=1";
                System.Collections.Generic.List<SqlParameter> parameters = new System.Collections.Generic.List<SqlParameter>();
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND Code LIKE @Search";
                    parameters.Add(new SqlParameter("@Search", "%" + searchTerm + "%"));
                }
                
                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND IsActive = @Status";
                    parameters.Add(new SqlParameter("@Status", status == "1"));
                }
                
                query += " ORDER BY IsActive DESC, CreatedAt DESC";
                
                DataTable dt;
                if (parameters.Count > 0)
                {
                    dt = db.ExecuteQuery(query, parameters.ToArray());
                }
                else
                {
                    dt = db.ExecuteQuery(query);
                }
                
                gvPromoCodes.DataSource = dt;
                gvPromoCodes.DataBind();
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur lors du chargement: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch != null ? txtSearch.Text.Trim() : "";
                string status = ddlStatusFilter != null && ddlStatusFilter.SelectedValue != null ? ddlStatusFilter.SelectedValue : "";
                LoadPromoCodes(searchTerm, status);
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur lors de la recherche: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            if (ddlStatusFilter.Items.Count > 0)
                ddlStatusFilter.SelectedIndex = 0;
            LoadPromoCodes("", "");
        }

        protected void gvPromoCodes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPromoCodes.PageIndex = e.NewPageIndex;
            string searchTerm = txtSearch != null ? txtSearch.Text.Trim() : "";
            string status = ddlStatusFilter != null && ddlStatusFilter.SelectedValue != null ? ddlStatusFilter.SelectedValue : "";
            LoadPromoCodes(searchTerm, status);
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            pnlList.Visible = false;
            pnlEdit.Visible = true;
            btnAddNew.Visible = false;

            hfCouponId.Value = "";
            txtCode.Text = "";
            txtDiscountValue.Text = "";
            txtMinimumAmount.Text = "0";
            txtMaximumDiscount.Text = "0";
            txtUsageLimit.Text = "0";
            ddlDiscountType.SelectedValue = "Percentage";
            ddlIsActive.SelectedValue = "1";
            SetDefaultDates();
            UpdateDiscountHint();
            lblTitle.Text = "<i class=\"fas fa-plus\"></i> Ajouter un Code Promo";
            lblError.Visible = false;
            lblSuccess.Visible = false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlEdit.Visible = false;
            pnlList.Visible = true;
            btnAddNew.Visible = true;
            lblError.Visible = false;
            lblSuccess.Visible = false;
        }

        protected void ddlDiscountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDiscountHint();
        }

        private void UpdateDiscountHint()
        {
            if (ddlDiscountType.SelectedValue == "Percentage")
            {
                lblDiscountHint.Text = "Entrez un pourcentage (ex: 20 pour 20% de réduction)";
                maxDiscountLabel.InnerText = "(Pourcentage uniquement)";
                txtMaximumDiscount.Enabled = true;
            }
            else
            {
                lblDiscountHint.Text = "Entrez un montant fixe en MAD (ex: 50 pour 50 MAD de réduction)";
                maxDiscountLabel.InnerText = "(Non applicable)";
                txtMaximumDiscount.Text = "0";
                txtMaximumDiscount.Enabled = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                string code = txtCode.Text.Trim().ToUpper();
                string discountType = ddlDiscountType.SelectedValue;
                decimal discountValue = Convert.ToDecimal(txtDiscountValue.Text);
                decimal minimumAmount = string.IsNullOrEmpty(txtMinimumAmount.Text) ? 0 : Convert.ToDecimal(txtMinimumAmount.Text);
                decimal maximumDiscount = string.IsNullOrEmpty(txtMaximumDiscount.Text) || !txtMaximumDiscount.Enabled ? 0 : Convert.ToDecimal(txtMaximumDiscount.Text);
                DateTime startDate = Convert.ToDateTime(txtStartDate.Text);
                DateTime endDate = Convert.ToDateTime(txtEndDate.Text);
                int usageLimit = string.IsNullOrEmpty(txtUsageLimit.Text) ? 0 : Convert.ToInt32(txtUsageLimit.Text);
                bool isActive = ddlIsActive.SelectedValue == "1";

                // Validate dates
                if (endDate <= startDate)
                {
                    lblError.Text = "La date de fin doit être supérieure à la date de début.";
                    lblError.Visible = true;
                    return;
                }

                // Validate discount value
                if (discountValue <= 0)
                {
                    lblError.Text = "La valeur de réduction doit être supérieure à 0.";
                    lblError.Visible = true;
                    return;
                }

                if (discountType == "Percentage" && discountValue > 100)
                {
                    lblError.Text = "Le pourcentage ne peut pas dépasser 100%.";
                    lblError.Visible = true;
                    return;
                }

                DbContext db = new DbContext();

                if (string.IsNullOrEmpty(hfCouponId.Value))
                {
                    // Check if code already exists
                    object existingCode = db.ExecuteScalar("SELECT Id FROM Coupons WHERE Code = @Code",
                        new SqlParameter[] { new SqlParameter("@Code", code) });

                    if (existingCode != null && existingCode != DBNull.Value)
                    {
                        lblError.Text = "Ce code promo existe déjà. Veuillez choisir un autre code.";
                        lblError.Visible = true;
                        return;
                    }

                    // Insertion
                    string query = @"INSERT INTO Coupons (Code, DiscountType, DiscountValue, MinimumAmount, MaximumDiscount, 
                                     StartDate, EndDate, UsageLimit, UsedCount, IsActive, CreatedAt) 
                                     VALUES (@Code, @DiscountType, @DiscountValue, @MinimumAmount, @MaximumDiscount, 
                                     @StartDate, @EndDate, @UsageLimit, 0, @IsActive, GETDATE())";

                    SqlParameter[] parameters = {
                        new SqlParameter("@Code", code),
                        new SqlParameter("@DiscountType", discountType),
                        new SqlParameter("@DiscountValue", discountValue),
                        new SqlParameter("@MinimumAmount", minimumAmount > 0 ? (object)minimumAmount : DBNull.Value),
                        new SqlParameter("@MaximumDiscount", maximumDiscount > 0 ? (object)maximumDiscount : DBNull.Value),
                        new SqlParameter("@StartDate", startDate),
                        new SqlParameter("@EndDate", endDate),
                        new SqlParameter("@UsageLimit", usageLimit > 0 ? (object)usageLimit : DBNull.Value),
                        new SqlParameter("@IsActive", isActive)
                    };

                    db.ExecuteNonQuery(query, parameters);
                    lblSuccess.Text = "Code promo ajouté avec succès!";
                    lblSuccess.Visible = true;
                }
                else
                {
                    // Mise à jour
                    int couponId = Convert.ToInt32(hfCouponId.Value);

                    // Check if code already exists (excluding current coupon)
                    object existingCode = db.ExecuteScalar("SELECT Id FROM Coupons WHERE Code = @Code AND Id != @Id",
                        new SqlParameter[] {
                            new SqlParameter("@Code", code),
                            new SqlParameter("@Id", couponId)
                        });

                    if (existingCode != null && existingCode != DBNull.Value)
                    {
                        lblError.Text = "Ce code promo existe déjà. Veuillez choisir un autre code.";
                        lblError.Visible = true;
                        return;
                    }

                    string query = @"UPDATE Coupons SET Code=@Code, DiscountType=@DiscountType, DiscountValue=@DiscountValue, 
                                     MinimumAmount=@MinimumAmount, MaximumDiscount=@MaximumDiscount, 
                                     StartDate=@StartDate, EndDate=@EndDate, UsageLimit=@UsageLimit, IsActive=@IsActive 
                                     WHERE Id=@Id";

                    SqlParameter[] parameters = {
                        new SqlParameter("@Code", code),
                        new SqlParameter("@DiscountType", discountType),
                        new SqlParameter("@DiscountValue", discountValue),
                        new SqlParameter("@MinimumAmount", minimumAmount > 0 ? (object)minimumAmount : DBNull.Value),
                        new SqlParameter("@MaximumDiscount", maximumDiscount > 0 ? (object)maximumDiscount : DBNull.Value),
                        new SqlParameter("@StartDate", startDate),
                        new SqlParameter("@EndDate", endDate),
                        new SqlParameter("@UsageLimit", usageLimit > 0 ? (object)usageLimit : DBNull.Value),
                        new SqlParameter("@IsActive", isActive),
                        new SqlParameter("@Id", couponId)
                    };

                    db.ExecuteNonQuery(query, parameters);
                    lblSuccess.Text = "Code promo mis à jour avec succès!";
                    lblSuccess.Visible = true;
                }

                // Recharger la liste après un court délai
                System.Threading.Thread.Sleep(500);
                string searchTerm = txtSearch != null ? txtSearch.Text.Trim() : "";
                string status = ddlStatusFilter != null ? ddlStatusFilter.SelectedValue : "";
                LoadPromoCodes(searchTerm, status);

                pnlEdit.Visible = false;
                pnlList.Visible = true;
                btnAddNew.Visible = true;
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void gvPromoCodes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditCoupon")
            {
                string id = e.CommandArgument.ToString();
                LoadCouponForEdit(id);
            }
            else if (e.CommandName == "ToggleCoupon")
            {
                string id = e.CommandArgument.ToString();
                ToggleCouponStatus(id);
            }
            else if (e.CommandName == "DeleteCoupon")
            {
                string id = e.CommandArgument.ToString();
                DeleteCoupon(id);
            }
        }

        protected void gvPromoCodes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                bool isActive = Convert.ToBoolean(rowView["IsActive"]);
                
                if (!isActive)
                {
                    e.Row.CssClass += " inactive-row";
                }
            }
        }

        private void LoadCouponForEdit(string id)
        {
            try
            {
                DbContext db = new DbContext();
                DataTable dt = db.ExecuteQuery("SELECT * FROM Coupons WHERE Id = @Id",
                    new SqlParameter[] { new SqlParameter("@Id", id) });

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    hfCouponId.Value = row["Id"].ToString();
                    txtCode.Text = row["Code"].ToString();
                    ddlDiscountType.SelectedValue = row["DiscountType"].ToString();
                    txtDiscountValue.Text = row["DiscountValue"].ToString();
                    txtMinimumAmount.Text = row["MinimumAmount"] != DBNull.Value ? row["MinimumAmount"].ToString() : "0";
                    txtMaximumDiscount.Text = row["MaximumDiscount"] != DBNull.Value ? row["MaximumDiscount"].ToString() : "0";
                    
                    DateTime startDate = Convert.ToDateTime(row["StartDate"]);
                    DateTime endDate = Convert.ToDateTime(row["EndDate"]);
                    txtStartDate.Text = startDate.ToString("yyyy-MM-ddTHH:mm");
                    txtEndDate.Text = endDate.ToString("yyyy-MM-ddTHH:mm");
                    
                    txtUsageLimit.Text = row["UsageLimit"] != DBNull.Value ? row["UsageLimit"].ToString() : "0";
                    ddlIsActive.SelectedValue = Convert.ToBoolean(row["IsActive"]) ? "1" : "0";
                    
                    UpdateDiscountHint();
                    lblTitle.Text = "<i class=\"fas fa-edit\"></i> Modifier le Code Promo";
                    pnlList.Visible = false;
                    btnAddNew.Visible = false;
                    pnlEdit.Visible = true;
                    lblError.Visible = false;
                    lblSuccess.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur lors du chargement: " + ex.Message;
                lblError.Visible = true;
            }
        }

        private void ToggleCouponStatus(string id)
        {
            try
            {
                DbContext db = new DbContext();

                // Get current status
                object currentStatus = db.ExecuteScalar("SELECT IsActive FROM Coupons WHERE Id = @Id",
                    new SqlParameter[] { new SqlParameter("@Id", id) });

                if (currentStatus != null && currentStatus != DBNull.Value)
                {
                    bool newStatus = !Convert.ToBoolean(currentStatus);

                    // Update status
                    db.ExecuteNonQuery("UPDATE Coupons SET IsActive = @IsActive WHERE Id = @Id",
                        new SqlParameter[] {
                            new SqlParameter("@IsActive", newStatus),
                            new SqlParameter("@Id", id)
                        });

                    lblSuccess.Text = $"Code promo {(newStatus ? "activé" : "désactivé")} avec succès!";
                    lblSuccess.Visible = true;
                    string searchTerm = txtSearch != null ? txtSearch.Text.Trim() : "";
                    string status = ddlStatusFilter != null ? ddlStatusFilter.SelectedValue : "";
                    LoadPromoCodes(searchTerm, status);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur lors de la modification: " + ex.Message;
                lblError.Visible = true;
            }
        }

        private void DeleteCoupon(string id)
        {
            try
            {
                DbContext db = new DbContext();

                // Check if coupon has been used
                object usageCount = db.ExecuteScalar("SELECT COUNT(*) FROM CouponUsage WHERE CouponId = @Id",
                    new SqlParameter[] { new SqlParameter("@Id", id) });

                if (Convert.ToInt32(usageCount) > 0)
                {
                    lblError.Text = "Impossible de supprimer ce code promo car il a déjà été utilisé.";
                    lblError.Visible = true;
                    return;
                }

                // Delete coupon
                db.ExecuteNonQuery("DELETE FROM Coupons WHERE Id = @Id",
                    new SqlParameter[] { new SqlParameter("@Id", id) });

                lblSuccess.Text = "Code promo supprimé avec succès!";
                lblSuccess.Visible = true;
                string searchTerm = txtSearch != null ? txtSearch.Text.Trim() : "";
                string status = ddlStatusFilter != null ? ddlStatusFilter.SelectedValue : "";
                LoadPromoCodes(searchTerm, status);
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur lors de la suppression: " + ex.Message;
                lblError.Visible = true;
            }
        }

        // Helper methods for GridView display
        protected string GetDiscountText(object discountType, object discountValue)
        {
            if (discountType == null || discountValue == null)
                return "-";

            string type = discountType.ToString();
            decimal value = Convert.ToDecimal(discountValue);

            if (type == "Percentage")
                return $"{value:F0}%";
            else
                return $"{value:F2} MAD";
        }

        protected string GetMinimumAmountText(object minimumAmount)
        {
            if (minimumAmount == null || minimumAmount == DBNull.Value)
                return "Aucun";
            
            decimal amount = Convert.ToDecimal(minimumAmount);
            return amount > 0 ? $"{amount:F2} MAD" : "Aucun";
        }

        protected string GetValidityText(object startDate, object endDate)
        {
            if (startDate == null || endDate == null)
                return "-";

            DateTime start = Convert.ToDateTime(startDate);
            DateTime end = Convert.ToDateTime(endDate);

            return $"{start:dd/MM/yyyy} - {end:dd/MM/yyyy}";
        }

        protected string GetUsageText(object usedCount, object usageLimit)
        {
            int used = usedCount != null && usedCount != DBNull.Value ? Convert.ToInt32(usedCount) : 0;
            int? limit = usageLimit != null && usageLimit != DBNull.Value ? (int?)Convert.ToInt32(usageLimit) : null;

            if (limit.HasValue && limit.Value > 0)
                return $"{used} / {limit.Value}";
            else
                return $"{used} utilisations";
        }

        protected string GetStatusClass(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "active" : "inactive";
        }

        protected string GetStatusText(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "Actif" : "Inactif";
        }

        protected string GetRowClass(object isActive)
        {
            bool active = false;
            if (isActive != null && isActive != DBNull.Value)
            {
                bool.TryParse(isActive.ToString(), out active);
            }
            return active ? "" : "inactive-row";
        }
    }
}

