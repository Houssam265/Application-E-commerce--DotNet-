using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecommerce.Data;

namespace Ecommerce.Pages.Admin
{
    public partial class Dashboard : Page
    {
        // Controls
        protected global::System.Web.UI.WebControls.Label lblTotalOrders;
        protected global::System.Web.UI.WebControls.Label lblRevenue;
        protected global::System.Web.UI.WebControls.Label lblTotalProducts;
        protected global::System.Web.UI.WebControls.Label lblTotalUsers;
        protected global::System.Web.UI.WebControls.Button btnSeed;
        protected global::System.Web.UI.WebControls.Label lblMsg;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStats();
            }
        }

        private void LoadStats()
        {
            try
            {
                DbContext db = new DbContext();
                object orderCount = db.ExecuteScalar("SELECT COUNT(*) FROM Orders");
                lblTotalOrders.Text = orderCount.ToString();
                
                object rev = db.ExecuteScalar("SELECT SUM(TotalAmount) FROM Orders");
                lblRevenue.Text = (rev != DBNull.Value ? Convert.ToDecimal(rev) : 0).ToString("C");
                
                object prodCount = db.ExecuteScalar("SELECT COUNT(*) FROM Products WHERE IsActive = 1");
                lblTotalProducts.Text = prodCount.ToString();
                
                object userCount = db.ExecuteScalar("SELECT COUNT(*) FROM Users");
                lblTotalUsers.Text = userCount.ToString();
            }
            catch { }
        }

        protected void btnSeed_Click(object sender, EventArgs e)
        {
            try
            {
                DbContext db = new DbContext();
                
                int catCount = (int)db.ExecuteScalar("SELECT COUNT(*) FROM Categories");
                if (catCount == 0)
                {
                    db.ExecuteNonQuery("INSERT INTO Categories (Name) VALUES ('Montres'); INSERT INTO Categories (Name) VALUES ('Sacs');");
                }

                int prodCount = (int)db.ExecuteScalar("SELECT COUNT(*) FROM Products");
                if (prodCount == 0)
                {
                    int catId = (int)db.ExecuteScalar("SELECT TOP 1 Id FROM Categories");

                    db.ExecuteNonQuery($@"
                        INSERT INTO Products (CategoryId, Name, Description, Price, StockQuantity, ImageUrl) 
                        VALUES ({catId}, 'Montre Luxe Gold', 'Une montre en or très élégante.', 499.00, 10, 'watch_gold.jpg');
                        
                        INSERT INTO Products (CategoryId, Name, Description, Price, StockQuantity, ImageUrl) 
                        VALUES ({catId}, 'Sac Cuir Premium', 'Cuir authentique italien.', 299.00, 5, 'bag_leather.jpg');
                    ");
                }

                lblMsg.Text = "Données initialisées avec succès !";
                lblMsg.ForeColor = System.Drawing.Color.Green;
                lblMsg.Visible = true;
                LoadStats();
            }
            catch (Exception ex)
            {
                 lblMsg.Text = "Erreur: " + ex.Message;
                 lblMsg.ForeColor = System.Drawing.Color.Red;
                 lblMsg.Visible = true;
            }
        }
    }
}
