using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;
using Ecommerce.Utils;

namespace Ecommerce.Pages.Public
{
    public partial class Wishlist : Page
    {
        protected global::System.Web.UI.WebControls.Panel pnlNotLoggedIn;
        protected global::System.Web.UI.WebControls.Panel pnlWishlist;
        protected global::System.Web.UI.WebControls.Repeater rptWishlist;
        protected global::System.Web.UI.WebControls.Panel pnlEmptyWishlist;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                pnlNotLoggedIn.Visible = true;
                pnlWishlist.Visible = false;
                return;
            }

            pnlNotLoggedIn.Visible = false;
            pnlWishlist.Visible = true;

            if (!IsPostBack)
            {
                LoadWishlist();
            }
        }

        private void LoadWishlist()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                
                string query = @"SELECT w.Id, w.ProductId, w.CreatedAt,
                                        p.Name, p.Description, p.ShortDescription, p.Price, p.ImageUrl, p.StockQuantity
                                 FROM Wishlist w
                                 INNER JOIN Products p ON w.ProductId = p.Id
                                 WHERE w.UserId = @UserId AND p.IsActive = 1
                                 ORDER BY w.CreatedAt DESC";
                
                SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    rptWishlist.DataSource = dt;
                    rptWishlist.DataBind();
                    pnlEmptyWishlist.Visible = false;
                }
                else
                {
                    rptWishlist.DataSource = null;
                    rptWishlist.DataBind();
                    pnlEmptyWishlist.Visible = true;
                }
            }
            catch
            {
                pnlEmptyWishlist.Visible = true;
            }
        }

        protected void rptWishlist_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string commandName = e.CommandName;

            if (commandName == "AddToCart")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    CartHelper.AddToCart(productId, 1);
                    Session["CartCount"] = CartHelper.GetCartItemCount();
                    ShowNotification("Produit ajouté au panier avec succès !", "success");
                    LoadWishlist();
                }
                catch (Exception ex)
                {
                    ShowNotification("Erreur lors de l'ajout au panier: " + Server.HtmlEncode(ex.Message), "error");
                }
            }
            else if (commandName == "RemoveFromWishlist")
            {
                int wishlistId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    DbContext db = new DbContext();
                    string deleteQuery = "DELETE FROM Wishlist WHERE Id = @Id AND UserId = @UserId";
                    SqlParameter[] parameters = {
                        new SqlParameter("@Id", wishlistId),
                        new SqlParameter("@UserId", userId)
                    };
                    db.ExecuteNonQuery(deleteQuery, parameters);
                    ShowNotification("Produit retiré de vos favoris", "success");
                    LoadWishlist();
                }
                catch (Exception ex)
                {
                    ShowNotification("Erreur lors de la suppression: " + Server.HtmlEncode(ex.Message), "error");
                }
            }
        }

        protected string GetImageUrl(object imageUrl)
        {
            if (imageUrl == null || imageUrl == DBNull.Value)
            {
                return ResolveUrl("~/Assets/Images/placeholder.svg");
            }

            string imageUrlStr = imageUrl.ToString().Trim();
            
            if (string.IsNullOrEmpty(imageUrlStr))
            {
                return ResolveUrl("~/Assets/Images/placeholder.svg");
            }

            // Si c'est une URL absolue (http/https), la retourner telle quelle
            if (imageUrlStr.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                imageUrlStr.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return imageUrlStr;
            }

            // Si c'est un simple nom de fichier (sans slash), ajouter le chemin de base
            if (!imageUrlStr.Contains("/") && !imageUrlStr.Contains("\\"))
            {
                return ResolveUrl("~/Assets/Images/Products/" + imageUrlStr);
            }

            // Si c'est un chemin relatif, le résoudre
            if (imageUrlStr.StartsWith("~/") || imageUrlStr.StartsWith("../") || imageUrlStr.StartsWith("./"))
            {
                return ResolveUrl(imageUrlStr);
            }

            // Si le chemin commence par Assets/, le résoudre avec ~/
            if (imageUrlStr.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                return ResolveUrl("~/" + imageUrlStr);
            }

            // Sinon, essayer de résoudre tel quel
            return ResolveUrl("~/Assets/Images/Products/" + imageUrlStr);
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
            string script = $"showNotification('{message.Replace("'", "\\'")}', '{type}');";
            ClientScript.RegisterStartupScript(this.GetType(), "Notification_" + Guid.NewGuid().ToString("N"), script, true);
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

