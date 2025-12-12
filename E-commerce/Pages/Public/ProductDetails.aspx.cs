using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Ecommerce.Data;

namespace Ecommerce.Pages.Public
{
    public partial class ProductDetails : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.Image imgMain;
        protected global::System.Web.UI.WebControls.Label lblName;
        protected global::System.Web.UI.WebControls.Label lblPrice;
        protected global::System.Web.UI.WebControls.Label lblDescription;
        protected global::System.Web.UI.WebControls.Panel pnlVariants;
        protected global::System.Web.UI.WebControls.DropDownList ddlVariants;
        protected global::System.Web.UI.WebControls.TextBox txtQuantity;
        protected global::System.Web.UI.WebControls.Button btnAddToCart;
        protected global::System.Web.UI.WebControls.Button btnWishlist;
        protected global::System.Web.UI.WebControls.Label lblMessage;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string productId = Request.QueryString["id"];
                if (string.IsNullOrEmpty(productId))
                {
                    Response.Redirect("Shop.aspx");
                    return;
                }
                LoadProduct(productId);
            }
        }

        private void LoadProduct(string id)
        {
            try
            {
                DbContext db = new DbContext();
                string query = "SELECT * FROM Products WHERE Id = @Id";
                SqlParameter[] p = { new SqlParameter("@Id", id) };
                
                DataTable dt = db.ExecuteQuery(query, p);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    lblName.Text = row["Name"].ToString();
                    lblDescription.Text = row["Description"].ToString();
                    decimal price = Convert.ToDecimal(row["Price"]);
                    lblPrice.Text = price.ToString("C");
                    
                    // Use ImageUrl directly (supports both local and external URLs)
                    string imageUrl = row["ImageUrl"].ToString();
                    imgMain.ImageUrl = imageUrl;
                    // Fallback handled by onerror in HTML if needed

                    LoadVariants(id);
                }
                else
                {
                    Response.Redirect("Shop.aspx");
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Erreur: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        private void LoadVariants(string productId)
        {
            DbContext db = new DbContext();
            string query = "SELECT Id, Size, Color, PriceAdjustment FROM ProductVariants WHERE ProductId = @Id";
            SqlParameter[] p = { new SqlParameter("@Id", productId) };
            DataTable dt = db.ExecuteQuery(query, p);

            if (dt.Rows.Count > 0)
            {
                pnlVariants.Visible = true;
                ddlVariants.DataSource = dt;
                ddlVariants.DataTextField = "Size";
                ddlVariants.DataValueField = "Id";
                ddlVariants.DataBind();
                ddlVariants.Items.Insert(0, new ListItem("Choisir une option", ""));
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            string productId = Request.QueryString["id"];
            int quantity = int.Parse(txtQuantity.Text);
            string variantId = ddlVariants.SelectedValue;

            if (pnlVariants.Visible && string.IsNullOrEmpty(variantId))
            {
                 lblMessage.Text = "Veuillez choisir une option.";
                 lblMessage.ForeColor = System.Drawing.Color.Red;
                 lblMessage.Visible = true;
                 return;
            }

            AddToCart(productId, quantity, variantId);

            lblMessage.Text = "Produit ajouté au panier !";
            lblMessage.ForeColor = System.Drawing.Color.FromArgb(16, 185, 129);
            lblMessage.Visible = true;
        }

        private void AddToCart(string productId, int quantity, string variantId)
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            var existingItem = cart.Find(i => i.ProductId == productId && i.VariantId == variantId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                DbContext db = new DbContext();
                string query = "SELECT Name, Price, ImageUrl FROM Products WHERE Id = @Id";
                DataTable dt = db.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@Id", productId) });
                
                if (dt.Rows.Count > 0)
                {
                    cart.Add(new CartItem 
                    {
                        ProductId = productId,
                        VariantId = variantId,
                        Quantity = quantity,
                        Name = dt.Rows[0]["Name"].ToString(),
                        Price = Convert.ToDecimal(dt.Rows[0]["Price"]),
                        ImageUrl = dt.Rows[0]["ImageUrl"].ToString()
                    });
                }
            }

            Session["Cart"] = cart;
        }
    }

    [Serializable]
    public class CartItem
    {
        public string ProductId { get; set; }
        public string VariantId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public decimal Total => Price * Quantity;
    }
}
