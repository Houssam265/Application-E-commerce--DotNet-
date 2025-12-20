using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Ecommerce.Data;

namespace Ecommerce.Pages.Public
{
    public partial class Shop : Page
    {
        protected global::System.Web.UI.WebControls.Repeater rptCategories;
        protected global::System.Web.UI.WebControls.Repeater rptProducts;
        protected global::System.Web.UI.WebControls.TextBox txtSearch;
        protected global::System.Web.UI.WebControls.Button btnSearch;
        protected global::System.Web.UI.WebControls.Label lblNoResults;
        protected global::System.Web.UI.WebControls.Label lblProductCount;
        protected global::System.Web.UI.WebControls.TextBox txtMinPrice;
        protected global::System.Web.UI.WebControls.TextBox txtMaxPrice;
        protected global::System.Web.UI.WebControls.Button btnApplyFilters;
        protected global::System.Web.UI.WebControls.CheckBox chkInStock;
        protected global::System.Web.UI.WebControls.DropDownList ddlSort;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();
                LoadProducts();
            }
        }

        private void LoadCategories()
        {
            try
            {
                DbContext db = new DbContext();
                DataTable dt = db.ExecuteQuery("SELECT * FROM Categories WHERE IsActive = 1 ORDER BY DisplayOrder, Name");
                rptCategories.DataSource = dt;
                rptCategories.DataBind();
            }
            catch { }
        }

        private void LoadProducts()
        {
            try
            {
                string catId = Request.QueryString["cat"];
                string search = txtSearch?.Text?.Trim() ?? "";
                string sort = ddlSort?.SelectedValue ?? "newest";
                decimal? minPrice = null;
                decimal? maxPrice = null;
                bool inStockOnly = chkInStock?.Checked ?? false;

                if (!string.IsNullOrEmpty(Request.Form[txtMinPrice.UniqueID]))
                {
                    decimal temp;
                    if (decimal.TryParse(Request.Form[txtMinPrice.UniqueID], out temp))
                        minPrice = temp;
                }

                if (!string.IsNullOrEmpty(Request.Form[txtMaxPrice.UniqueID]))
                {
                    decimal temp;
                    if (decimal.TryParse(Request.Form[txtMaxPrice.UniqueID], out temp))
                        maxPrice = temp;
                }

                string query = @"SELECT p.*, c.Name as CooperativeName, cat.Name as CategoryName
                                 FROM Products p
                                 LEFT JOIN Cooperatives c ON p.CooperativeId = c.Id
                                 LEFT JOIN Categories cat ON p.CategoryId = cat.Id
                                 WHERE p.IsActive = 1";
                
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(catId))
                {
                    query += " AND p.CategoryId = @CatId";
                    parameters.Add(new SqlParameter("@CatId", catId));
                }

                if (!string.IsNullOrEmpty(search))
                {
                    query += " AND (p.Name LIKE @Search OR p.Description LIKE @Search OR p.ShortDescription LIKE @Search)";
                    parameters.Add(new SqlParameter("@Search", "%" + search + "%"));
                }

                if (minPrice.HasValue)
                {
                    query += " AND p.Price >= @MinPrice";
                    parameters.Add(new SqlParameter("@MinPrice", minPrice.Value));
                }

                if (maxPrice.HasValue)
                {
                    query += " AND p.Price <= @MaxPrice";
                    parameters.Add(new SqlParameter("@MaxPrice", maxPrice.Value));
                }

                if (inStockOnly)
                {
                    query += " AND p.StockQuantity > 0";
                }

                // Sorting
                switch (sort)
                {
                    case "price_asc":
                        query += " ORDER BY p.Price ASC";
                        break;
                    case "price_desc":
                        query += " ORDER BY p.Price DESC";
                        break;
                    case "name":
                        query += " ORDER BY p.Name ASC";
                        break;
                    default:
                        query += " ORDER BY p.CreatedAt DESC";
                        break;
                }

                DbContext db = new DbContext();
                DataTable dt = db.ExecuteQuery(query, parameters.ToArray());

                if (dt.Rows.Count > 0)
                {
                    rptProducts.DataSource = dt;
                    rptProducts.DataBind();
                    lblNoResults.Visible = false;
                    lblProductCount.Text = $"{dt.Rows.Count} produit(s) trouvé(s)";
                }
                else
                {
                    rptProducts.DataSource = null;
                    rptProducts.DataBind();
                    lblNoResults.Visible = true;
                    lblProductCount.Text = "0 produit trouvé";
                }
            }
            catch (Exception ex)
            {
                lblNoResults.Text = "Erreur de chargement: " + ex.Message;
                lblNoResults.Visible = true;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        protected void btnApplyFilters_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        protected string IsActiveCategory(object id)
        {
            string currentCat = Request.QueryString["cat"];
            if (currentCat != null && id.ToString() == currentCat)
            {
                return "active";
            }
            return "";
        }

        protected string GetStockBadge(object stockQuantity)
        {
            if (stockQuantity != null && stockQuantity != DBNull.Value)
            {
                int stock = Convert.ToInt32(stockQuantity);
                if (stock <= 0)
                {
                    return "<span class='product-badge' style='background: var(--danger-color);'>Rupture</span>";
                }
            }
            return "";
        }
    }
}
