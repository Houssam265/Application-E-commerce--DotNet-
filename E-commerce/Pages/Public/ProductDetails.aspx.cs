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
    public partial class ProductDetails : Page
    {
        protected global::System.Web.UI.WebControls.Image imgMain;
        protected global::System.Web.UI.WebControls.Label lblName;
        protected global::System.Web.UI.WebControls.Label lblPrice;
        protected global::System.Web.UI.WebControls.Label lblComparePrice;
        protected global::System.Web.UI.WebControls.Label lblDescription;
        protected global::System.Web.UI.WebControls.Label lblCooperative;
        protected global::System.Web.UI.WebControls.Label lblStock;
        protected global::System.Web.UI.WebControls.Label lblCategory;
        protected global::System.Web.UI.WebControls.Label lblSKU;
        protected global::System.Web.UI.WebControls.Panel pnlVariants;
        protected global::System.Web.UI.WebControls.DropDownList ddlVariants;
        protected global::System.Web.UI.WebControls.TextBox txtQuantity;
        protected global::System.Web.UI.WebControls.Button btnAddToCart;
        protected global::System.Web.UI.WebControls.LinkButton btnWishlist;
        protected global::System.Web.UI.WebControls.Panel pnlProductNotFound;
        protected global::System.Web.UI.WebControls.Panel pnlProductDetails;
        protected global::System.Web.UI.WebControls.Literal litStockStatus;
        protected global::System.Web.UI.WebControls.Repeater rptReviews;
        protected global::System.Web.UI.WebControls.Label lblNoReviews;

        private int productId = 0;

        protected global::System.Web.UI.WebControls.Panel pnlAddReview;
        protected global::System.Web.UI.WebControls.Panel pnlReviewError;
        protected global::System.Web.UI.WebControls.Literal litReviewError;
        protected global::System.Web.UI.WebControls.DropDownList ddlRating;
        protected global::System.Web.UI.WebControls.TextBox txtReviewComment;
        protected global::System.Web.UI.WebControls.Button btnSubmitReview;

        protected void Page_Load(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out productId))
            {
                pnlProductNotFound.Visible = true;
                pnlProductDetails.Visible = false;
                return;
            }

            if (!IsPostBack)
            {
                LoadProduct(productId);
                LoadReviews(productId);
                CheckCanAddReview(productId);
                CheckWishlistStatus(productId);
            }
            else
            {
                // Reload productId on postback (id is already declared above)
                if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int parsedId))
                {
                    productId = parsedId;
                }
                // Check wishlist status after postback
                if (productId > 0)
                {
                    CheckWishlistStatus(productId);
                }
            }
        }

        private void CheckCanAddReview(int productId)
        {
            if (Session["UserId"] == null)
            {
                pnlAddReview.Visible = false;
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            DbContext db = new DbContext();
            
            // Check if user already reviewed this product
            string checkQuery = "SELECT COUNT(*) FROM Reviews WHERE ProductId = @ProductId AND UserId = @UserId";
            SqlParameter[] checkParams = {
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@UserId", userId)
            };
            
            int count = (int)db.ExecuteScalar(checkQuery, checkParams);
            pnlAddReview.Visible = (count == 0);
        }

        private void LoadProduct(int id)
        {
            try
            {
                DbContext db = new DbContext();
                string query = @"SELECT p.*, c.Name as CooperativeName, cat.Name as CategoryName
                                 FROM Products p
                                 LEFT JOIN Cooperatives c ON p.CooperativeId = c.Id
                                 LEFT JOIN Categories cat ON p.CategoryId = cat.Id
                                 WHERE p.Id = @Id AND p.IsActive = 1";
                SqlParameter[] parameters = { new SqlParameter("@Id", id) };
                
                DataTable dt = db.ExecuteQuery(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    lblName.Text = Server.HtmlEncode(row["Name"].ToString());
                    lblDescription.Text = Server.HtmlEncode(row["Description"]?.ToString() ?? "");
                    decimal price = Convert.ToDecimal(row["Price"]);
                    lblPrice.Text = price.ToString("F2");
                    
                    if (row["CompareAtPrice"] != DBNull.Value)
                    {
                        decimal comparePrice = Convert.ToDecimal(row["CompareAtPrice"]);
                        lblComparePrice.Text = comparePrice.ToString("F2") + " MAD";
                        lblComparePrice.Visible = true;
                    }
                    
                    lblCooperative.Text = row["CooperativeName"]?.ToString() ?? "N/A";
                    lblStock.Text = row["StockQuantity"]?.ToString() ?? "0";
                    lblCategory.Text = row["CategoryName"]?.ToString() ?? "N/A";
                    
                    // Set SKU
                    if (row["SKU"] != DBNull.Value && !string.IsNullOrEmpty(row["SKU"].ToString()))
                    {
                        lblSKU.Text = "Réf: " + Server.HtmlEncode(row["SKU"].ToString());
                        lblSKU.Visible = true;
                    }
                    else
                    {
                        lblSKU.Visible = false;
                    }
                    
                    int stockQty = Convert.ToInt32(row["StockQuantity"]);
                    if (stockQty > 0)
                    {
                        litStockStatus.Text = "<span class=\"stock-status stock-in\"><i class=\"fas fa-check-circle\"></i> En stock</span>";
                        btnAddToCart.Enabled = true;
                    }
                    else
                    {
                        litStockStatus.Text = "<span class=\"stock-status stock-out\"><i class=\"fas fa-times-circle\"></i> Rupture de stock</span>";
                        btnAddToCart.Enabled = false;
                    }
                    
                    string imageUrl = row["ImageUrl"]?.ToString() ?? "";
                    imgMain.ImageUrl = GetImageUrl(imageUrl);
                    
                    // Update view count
                    db.ExecuteNonQuery("UPDATE Products SET ViewCount = ViewCount + 1 WHERE Id = @Id", parameters);
                    
                    LoadVariants(id);
                }
                else
                {
                    pnlProductNotFound.Visible = true;
                    pnlProductDetails.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowNotification("Erreur lors du chargement du produit: " + Server.HtmlEncode(ex.Message), "error");
            }
        }

        private void LoadVariants(int productId)
        {
            try
            {
                DbContext db = new DbContext();
                string query = @"SELECT Id, VariantType, VariantValue, PriceAdjustment, StockQuantity 
                                 FROM ProductVariants 
                                 WHERE ProductId = @Id AND StockQuantity > 0
                                 ORDER BY VariantType, VariantValue";
                SqlParameter[] parameters = { new SqlParameter("@Id", productId) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    pnlVariants.Visible = true;
                    ddlVariants.DataSource = dt;
                    ddlVariants.DataTextField = "VariantValue";
                    ddlVariants.DataValueField = "Id";
                    ddlVariants.DataBind();
                    ddlVariants.Items.Insert(0, new ListItem("Sélectionner une option", "0"));
                }
            }
            catch { }
        }

        private void LoadReviews(int productId)
        {
            try
            {
                DbContext db = new DbContext();
                string query = @"SELECT r.*, u.FullName 
                                 FROM Reviews r
                                 INNER JOIN Users u ON r.UserId = u.Id
                                 WHERE r.ProductId = @ProductId AND r.IsApproved = 1
                                 ORDER BY r.ReviewDate DESC";
                SqlParameter[] parameters = { new SqlParameter("@ProductId", productId) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    rptReviews.DataSource = dt;
                    rptReviews.DataBind();
                    lblNoReviews.Visible = false;
                }
                else
                {
                    lblNoReviews.Visible = true;
                }
            }
            catch { }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            // Reload productId on postback
            string id = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int parsedId))
            {
                productId = parsedId;
            }

            if (productId == 0)
            {
                ShowNotification("Produit invalide.", "error");
                return;
            }

            int quantity;
            if (!int.TryParse(txtQuantity.Text, out quantity) || quantity < 1)
            {
                ShowNotification("Quantité invalide.", "error");
                return;
            }

            int? variantId = null;
            if (pnlVariants.Visible && ddlVariants.SelectedValue != "0")
            {
                int temp;
                if (int.TryParse(ddlVariants.SelectedValue, out temp))
                    variantId = temp;
            }

            try
            {
                CartHelper.AddToCart(productId, quantity, variantId);
                ShowNotification("Produit ajouté au panier avec succès !", "success");
                
                // Update cart count in session
                Session["CartCount"] = CartHelper.GetCartItemCount();
            }
            catch (Exception ex)
            {
                ShowNotification("Erreur lors de l'ajout au panier: " + Server.HtmlEncode(ex.Message), "error");
            }
        }

        private void CheckWishlistStatus(int productId)
        {
            if (Session["UserId"] == null)
            {
                // Not logged in - show empty heart
                btnWishlist.CssClass = "btn btn-outline";
                btnWishlist.Controls.Clear();
                btnWishlist.Controls.Add(new LiteralControl("<i class='far fa-heart'></i>"));
                return;
            }

            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                
                string checkQuery = "SELECT COUNT(*) FROM Wishlist WHERE UserId = @UserId AND ProductId = @ProductId";
                SqlParameter[] checkParams = {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@ProductId", productId)
                };
                
                int count = (int)db.ExecuteScalar(checkQuery, checkParams);
                
                btnWishlist.CssClass = count > 0 ? "btn btn-outline active" : "btn btn-outline";
                btnWishlist.Controls.Clear();
                if (count > 0)
                {
                    btnWishlist.Controls.Add(new LiteralControl("<i class='fas fa-heart'></i>"));
                }
                else
                {
                    btnWishlist.Controls.Add(new LiteralControl("<i class='far fa-heart'></i>"));
                }
            }
            catch
            {
                // On error, show empty heart
                btnWishlist.CssClass = "btn btn-outline";
                btnWishlist.Controls.Clear();
                btnWishlist.Controls.Add(new LiteralControl("<i class='far fa-heart'></i>"));
            }
        }

        protected void btnWishlist_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            // Reload productId on postback
            string id = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int parsedId))
            {
                productId = parsedId;
            }

            if (productId == 0) return;

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
                    // Remove from wishlist - create new parameters
                    string deleteQuery = "DELETE FROM Wishlist WHERE UserId = @UserId AND ProductId = @ProductId";
                    SqlParameter[] deleteParams = new SqlParameter[] {
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@ProductId", productId)
                    };
                    db.ExecuteNonQuery(deleteQuery, deleteParams);
                    ShowNotification("Retiré de la liste de souhaits", "success");
                }
                else
                {
                    // Add to wishlist - create new parameters
                    string insertQuery = "INSERT INTO Wishlist (UserId, ProductId) VALUES (@UserId, @ProductId)";
                    SqlParameter[] insertParams = new SqlParameter[] {
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@ProductId", productId)
                    };
                    db.ExecuteNonQuery(insertQuery, insertParams);
                    ShowNotification("Ajouté à la liste de souhaits", "success");
                }
                
                // Update wishlist icon
                CheckWishlistStatus(productId);
            }
            catch (Exception ex)
            {
                ShowNotification("Erreur: " + Server.HtmlEncode(ex.Message), "error");
            }
        }

        protected void ddlVariants_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update price if variant selected
            if (ddlVariants.SelectedValue != "0")
            {
                try
                {
                    DbContext db = new DbContext();
                    string query = "SELECT PriceAdjustment FROM ProductVariants WHERE Id = @Id";
                    SqlParameter[] parameters = { new SqlParameter("@Id", ddlVariants.SelectedValue) };
                    DataTable dt = db.ExecuteQuery(query, parameters);
                    
                    if (dt.Rows.Count > 0)
                    {
                        decimal basePrice = Convert.ToDecimal(lblPrice.Text);
                        decimal adjustment = Convert.ToDecimal(dt.Rows[0]["PriceAdjustment"]);
                        decimal newPrice = basePrice + adjustment;
                        lblPrice.Text = newPrice.ToString("F2");
                    }
                }
                catch { }
            }
        }

        protected string GetStars(object rating)
        {
            int ratingValue = 0;
            if (rating != null && rating != DBNull.Value)
            {
                ratingValue = Convert.ToInt32(rating);
            }
            
            string stars = "";
            for (int i = 1; i <= 5; i++)
            {
                if (i <= ratingValue)
                    stars += "<i class='fas fa-star'></i>";
                else
                    stars += "<i class='far fa-star'></i>";
            }
            return stars;
        }

        protected string GetReviewDate(object reviewDate)
        {
            if (reviewDate != null && reviewDate != DBNull.Value)
            {
                return Convert.ToDateTime(reviewDate).ToString("dd/MM/yyyy");
            }
            return "";
        }

        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            pnlReviewError.Visible = false;

            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }

            if (productId == 0)
            {
                ShowReviewError("Produit invalide.");
                return;
            }

            int rating;
            if (!int.TryParse(ddlRating.SelectedValue, out rating) || rating < 1 || rating > 5)
            {
                ShowReviewError("Veuillez sélectionner une note valide.");
                return;
            }

            string comment = txtReviewComment.Text.Trim();
            if (string.IsNullOrWhiteSpace(comment))
            {
                ShowReviewError("Veuillez saisir un commentaire.");
                return;
            }

            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DbContext db = new DbContext();
                string insertQuery = @"INSERT INTO Reviews (ProductId, UserId, Rating, Comment, ReviewDate, IsApproved)
                                       VALUES (@ProductId, @UserId, @Rating, @Comment, GETDATE(), 1)";
                SqlParameter[] parameters = {
                    new SqlParameter("@ProductId", productId),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Rating", rating),
                    new SqlParameter("@Comment", comment)
                };

                db.ExecuteNonQuery(insertQuery, parameters);

                pnlAddReview.Visible = false;
                ShowNotification("Merci pour votre avis !", "success");

                LoadReviews(productId);
            }
            catch (Exception ex)
            {
                ShowReviewError("Impossible d'enregistrer votre avis : " + Server.HtmlEncode(ex.Message));
            }
        }

        private void ShowReviewError(string message)
        {
            pnlReviewError.Visible = true;
            litReviewError.Text = message;
        }

        protected void ShowNotification(string message, string type)
        {
            string script = $"showNotification('{message.Replace("'", "\\'")}', '{type}');";
            ClientScript.RegisterStartupScript(this.GetType(), "Notification_" + Guid.NewGuid().ToString("N"), script, true);
        }

        protected string GetImageUrl(object imageUrlObj)
        {
            if (imageUrlObj == null || imageUrlObj == DBNull.Value)
            {
                return ResolveUrl("~/Assets/Images/Products/placeholder.jpg");
            }

            string imageUrl = imageUrlObj.ToString().Trim();
            
            if (string.IsNullOrEmpty(imageUrl))
            {
                return ResolveUrl("~/Assets/Images/Products/placeholder.jpg");
            }

            // Si c'est une URL absolue (http/https), la retourner telle quelle
            if (imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return imageUrl;
            }

            // Si c'est un simple nom de fichier (sans slash), ajouter le chemin de base
            if (!imageUrl.Contains("/") && !imageUrl.Contains("\\"))
            {
                return ResolveUrl("~/Assets/Images/Products/" + imageUrl);
            }

            // Si c'est un chemin relatif, le résoudre
            if (imageUrl.StartsWith("~/") || imageUrl.StartsWith("../") || imageUrl.StartsWith("./"))
            {
                return ResolveUrl(imageUrl);
            }

            // Si le chemin commence par Assets/, le résoudre avec ~/
            if (imageUrl.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                return ResolveUrl("~/" + imageUrl);
            }

            // Sinon, essayer de résoudre tel quel
            return ResolveUrl("~/Assets/Images/Products/" + imageUrl);
        }
    }
}
