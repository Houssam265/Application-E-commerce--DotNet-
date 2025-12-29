using System;
using System.Data;
using System.Web.UI;
using Ecommerce.Data;

namespace Ecommerce
{
    public partial class Default : Page
    {
        protected global::System.Web.UI.WebControls.Repeater rptCategories;
        protected global::System.Web.UI.WebControls.Repeater rptFeaturedProducts;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();
                LoadFeaturedProducts();
            }
        }

        private void LoadCategories()
        {
            try
            {
                DbContext db = new DbContext();
                string query = "SELECT TOP 6 * FROM Categories WHERE IsActive = 1 ORDER BY DisplayOrder, Name";
                DataTable dt = db.ExecuteQuery(query);
                
                rptCategories.DataSource = dt;
                rptCategories.DataBind();
            }
            catch
            {
                // Silent fail
            }
        }

        private void LoadFeaturedProducts()
        {
            try
            {
                DbContext db = new DbContext();
                string query = @"SELECT TOP 8 p.*
                                 FROM Products p
                                 WHERE p.IsActive = 1 AND p.IsFeatured = 1
                                 ORDER BY p.CreatedAt DESC";
                DataTable dt = db.ExecuteQuery(query);
                
                rptFeaturedProducts.DataSource = dt;
                rptFeaturedProducts.DataBind();
            }
            catch
            {
                // Silent fail
            }
        }

        protected string GetFeaturedBadge(object isFeatured)
        {
            if (isFeatured != null && isFeatured != DBNull.Value)
            {
                bool featured = Convert.ToBoolean(isFeatured);
                if (featured)
                {
                    return "<span class='product-badge'>Nouveau</span>";
                }
            }
            return "";
        }

        protected string GetCategoryImage(object imageUrl, object categoryName)
        {
            string imageUrlStr = imageUrl?.ToString()?.Trim() ?? "";
            string categoryNameStr = categoryName?.ToString() ?? "Category";
            
            if (!string.IsNullOrEmpty(imageUrlStr))
            {
                // Si c'est une URL absolue (http/https), la retourner telle quelle
                if (imageUrlStr.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    imageUrlStr.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return $"<img src='{imageUrlStr}' alt='{categoryNameStr}' onerror=\"this.style.display='none'; this.nextElementSibling.style.display='block';\">" +
                           "<i class='fas fa-seedling category-icon' style='color: var(--primary-color); display: none;'></i>";
                }
                
                // Si c'est un simple nom de fichier ou chemin relatif
                string url = imageUrlStr;
                if (!url.StartsWith("~/") && !url.StartsWith("/"))
                {
                     // Supposer que c'est juste le nom du fichier si pas de slash
                    if (!url.Contains("/") && !url.Contains("\\"))
                    {
                        url = "~/Assets/Images/Categories/" + url;
                    } 
                    else 
                    {
                        // Si contient slash mais pas de tilde/slash début, ajouter ~/
                         url = "~/" + url;
                    }
                }
                else if (url.StartsWith("/"))
                {
                     // Convertir /Assets... en ~/Assets... pour ResolveUrl
                     url = "~" + url;
                }

                string resolvedUrl = ResolveUrl(url);
                return $"<img src='{resolvedUrl}' alt='{categoryNameStr}' onerror=\"this.style.display='none'; this.nextElementSibling.style.display='block';\">" +
                       "<i class='fas fa-seedling category-icon' style='color: var(--primary-color); display: none;'></i>";
            }
            
            // Pas d'image, afficher l'icône par défaut
            return "<i class='fas fa-seedling category-icon' style='color: var(--primary-color);'></i>";
        }
    }
}
