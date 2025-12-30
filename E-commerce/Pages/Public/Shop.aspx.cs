using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Ecommerce.Data;
using Ecommerce.Utils;

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

                string query = @"SELECT p.*, cat.Name as CategoryName,
                                        CASE WHEN w.Id IS NOT NULL THEN 1 ELSE 0 END as IsInWishlist
                                 FROM Products p
                                 LEFT JOIN Categories cat ON p.CategoryId = cat.Id
                                 LEFT JOIN Wishlist w ON p.Id = w.ProductId AND w.UserId = @UserId
                                 WHERE p.IsActive = 1";
                
                List<SqlParameter> parameters = new List<SqlParameter>();
                
                // Add UserId parameter for wishlist check
                int? userId = Session["UserId"] != null ? (int?)Convert.ToInt32(Session["UserId"]) : null;
                parameters.Add(new SqlParameter("@UserId", userId ?? (object)DBNull.Value));

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

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string commandName = e.CommandName;
            int productId = Convert.ToInt32(e.CommandArgument);

            if (commandName == "AddToCart")
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("/Pages/Public/Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                    return;
                }

                try
                {
                    // Check if product is in stock
                    DbContext db = new DbContext();
                    string stockQuery = "SELECT StockQuantity FROM Products WHERE Id = @ProductId";
                    SqlParameter[] stockParams = { new SqlParameter("@ProductId", productId) };
                    object stockResult = db.ExecuteScalar(stockQuery, stockParams);
                    
                    int stockQuantity = stockResult != DBNull.Value && stockResult != null ? Convert.ToInt32(stockResult) : 0;
                    
                    if (stockQuantity <= 0)
                    {
                        ShowNotification("Ce produit n'est plus en stock.", "error");
                        return;
                    }
                    
                    CartHelper.AddToCart(productId, 1);
                    Session["CartCount"] = CartHelper.GetCartItemCount();
                    ShowNotification("Produit ajouté au panier avec succès !", "success");
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    ShowNotification("Erreur lors de l'ajout au panier: " + Server.HtmlEncode(ex.Message), "error");
                }
            }
            else if (commandName == "AddToWishlist")
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                    return;
                }

                try
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    DbContext db = new DbContext();
                    
                    // Check if already in wishlist
                    string checkQuery = "SELECT COUNT(*) FROM Wishlist WHERE UserId = @UserId AND ProductId = @ProductId";
                    SqlParameter[] checkParams = {
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@ProductId", productId)
                    };
                    
                    int count = (int)db.ExecuteScalar(checkQuery, checkParams);
                    
                    if (count > 0)
                    {
                        // Remove from wishlist
                        string deleteQuery = "DELETE FROM Wishlist WHERE UserId = @UserId AND ProductId = @ProductId";
                        SqlParameter[] deleteParams = {
                            new SqlParameter("@UserId", userId),
                            new SqlParameter("@ProductId", productId)
                        };
                        db.ExecuteNonQuery(deleteQuery, deleteParams);
                        ShowNotification("Retiré de la liste de souhaits", "success");
                    }
                    else
                    {
                        // Add to wishlist
                        string insertQuery = "INSERT INTO Wishlist (UserId, ProductId) VALUES (@UserId, @ProductId)";
                        SqlParameter[] insertParams = {
                            new SqlParameter("@UserId", userId),
                            new SqlParameter("@ProductId", productId)
                        };
                        db.ExecuteNonQuery(insertQuery, insertParams);
                        ShowNotification("Ajouté à la liste de souhaits", "success");
                    }
                    
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    ShowNotification("Erreur lors de l'ajout aux favoris: " + Server.HtmlEncode(ex.Message), "error");
                }
            }
        }

        protected bool IsInStock(object stockQuantity)
        {
            if (stockQuantity != null && stockQuantity != DBNull.Value)
            {
                try
                {
                    int stock = Convert.ToInt32(stockQuantity);
                    return stock > 0;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        protected void ShowNotification(string message, string type)
        {
            string script = $"window.addEventListener('load', function() {{ showNotification('{message.Replace("'", "\\'")}', '{type}'); }});";
            ClientScript.RegisterStartupScript(this.GetType(), "Notification_" + Guid.NewGuid().ToString("N"), script, true);
        }

        protected string IsInWishlist(object isInWishlist)
        {
            if (isInWishlist != null && isInWishlist != DBNull.Value)
            {
                int value = Convert.ToInt32(isInWishlist);
                return value == 1 ? "active" : "";
            }
            return "";
        }

        protected string GetWishlistIcon(object isInWishlist)
        {
            if (isInWishlist != null && isInWishlist != DBNull.Value)
            {
                int value = Convert.ToInt32(isInWishlist);
                if (value == 1)
                {
                    return "<i class='fas fa-heart'></i>";
                }
            }
            return "<i class='far fa-heart'></i>";
        }

        protected string GetImageUrl(object imageUrl)
        {
            string url = imageUrl?.ToString()?.Trim() ?? "";

            if (string.IsNullOrEmpty(url))
            {
                return ResolveUrl("~/Assets/Images/placeholder.svg");
            }

            // Absolute URL (https, http)
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return url;
            }

            // If only filename stored, prepend product images folder
            if (!url.Contains("/") && !url.Contains("\\"))
            {
                url = "~/Assets/Images/Products/" + url;
            }
            else if (!url.StartsWith("~/"))
            {
                // Normalize other relative/virtual paths to application root
                url = "~/" + url.TrimStart('/');
            }

            return ResolveUrl(url);
        }
    }
}
