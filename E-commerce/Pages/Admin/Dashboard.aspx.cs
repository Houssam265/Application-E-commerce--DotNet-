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
        protected global::System.Web.UI.WebControls.Label lblLowStockProducts;
        protected global::System.Web.UI.WebControls.Label lblPendingComplaints;

        protected global::System.Web.UI.WebControls.HiddenField hfSalesData;
        protected global::System.Web.UI.WebControls.HiddenField hfOrdersByStatus;
        protected global::System.Web.UI.WebControls.HiddenField hfTopProducts;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStats();
                LoadChartData();
            }
        }

        private void LoadChartData()
        {
            hfSalesData.Value = GetSalesData();
            hfOrdersByStatus.Value = GetOrdersByStatus();
            hfTopProducts.Value = GetTopProducts();
        }

        private void LoadStats()
        {
            try
            {
                DbContext db = new DbContext();
                
                // Total orders (only active, non-archived)
                object orderCount = db.ExecuteScalar("SELECT COUNT(*) FROM Orders WHERE IsArchived = 0");
                lblTotalOrders.Text = orderCount?.ToString() ?? "0";
                
                // Revenue (excluding cancelled orders)
                object rev = db.ExecuteScalar("SELECT SUM(TotalAmount) FROM Orders WHERE Status != 'Cancelled'");
                decimal revenue = (rev != null && rev != DBNull.Value) ? Convert.ToDecimal(rev) : 0;
                lblRevenue.Text = revenue.ToString("F2") + " MAD";
                
                // Active products
                object prodCount = db.ExecuteScalar("SELECT COUNT(*) FROM Products WHERE IsActive = 1");
                lblTotalProducts.Text = prodCount?.ToString() ?? "0";
                
                // Total users (excluding admins)
                object userCount = db.ExecuteScalar("SELECT COUNT(*) FROM Users WHERE Role != 'Admin'");
                lblTotalUsers.Text = userCount?.ToString() ?? "0";
                
                // Low stock products (stock < 10)
                try
                {
                    object lowStockCount = db.ExecuteScalar("SELECT COUNT(*) FROM Products WHERE IsActive = 1 AND StockQuantity < 10");
                    if (lblLowStockProducts != null)
                        lblLowStockProducts.Text = lowStockCount?.ToString() ?? "0";
                }
                catch { }
                
                // Pending complaints
                try
                {
                    object complaintCount = db.ExecuteScalar("SELECT COUNT(*) FROM Complaints WHERE Status IN ('Pending', 'InProgress')");
                    if (lblPendingComplaints != null)
                        lblPendingComplaints.Text = complaintCount?.ToString() ?? "0";
                }
                catch { }
            }
            catch { }
        }

        // Méthode pour obtenir les données de ventes des 7 derniers jours
        public string GetSalesData()
        {
            try
            {
                DbContext db = new DbContext();
                string query = @"
                    SELECT 
                        CAST(OrderDate AS DATE) as SaleDate,
                        SUM(TotalAmount) as DailyRevenue
                    FROM Orders 
                    WHERE OrderDate >= DATEADD(day, -7, GETDATE()) 
                        AND Status != 'Cancelled'
                        AND (IsArchived IS NULL OR IsArchived = 0)
                    GROUP BY CAST(OrderDate AS DATE)
                    ORDER BY SaleDate ASC";
                
                DataTable dt = db.ExecuteQuery(query);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("[");
                
                // Créer un tableau pour les 7 derniers jours
                DateTime[] last7Days = new DateTime[7];
                for (int i = 6; i >= 0; i--)
                {
                    last7Days[6 - i] = DateTime.Now.AddDays(-i).Date;
                }
                
                // Remplir avec les données réelles ou 0
                bool isFirst = true;
                foreach (DateTime day in last7Days)
                {
                    decimal amount = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        DateTime saleDate = Convert.ToDateTime(row["SaleDate"]).Date;
                        if (saleDate == day)
                        {
                            amount = Convert.ToDecimal(row["DailyRevenue"]);
                            break;
                        }
                    }
                    if (!isFirst) sb.Append(",");
                    sb.Append(amount.ToString("F2"));
                    isFirst = false;
                }
                
                sb.Append("]");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                // Log l'erreur pour debug
                System.Diagnostics.Debug.WriteLine("GetSalesData Error: " + ex.Message);
                return "[0,0,0,0,0,0,0]";
            }
        }

        // Méthode pour obtenir les commandes par statut
        public string GetOrdersByStatus()
        {
            try
            {
                DbContext db = new DbContext();
                string query = @"
                    SELECT Status, COUNT(*) as Count
                    FROM Orders
                    GROUP BY Status";
                
                DataTable dt = db.ExecuteQuery(query);
                System.Collections.Generic.Dictionary<string, int> statusCounts = new System.Collections.Generic.Dictionary<string, int>
                {
                    { "Pending", 0 },
                    { "Processing", 0 },
                    { "Shipped", 0 },
                    { "Delivered", 0 },
                    { "Cancelled", 0 }
                };
                
                foreach (DataRow row in dt.Rows)
                {
                    string status = row["Status"].ToString();
                    int count = Convert.ToInt32(row["Count"]);
                    if (statusCounts.ContainsKey(status))
                    {
                        statusCounts[status] = count;
                    }
                }
                
                // Retourner dans l'ordre: En attente, Expédié, Livré, Annulé
                return $"[{statusCounts["Pending"]}, {statusCounts["Shipped"]}, {statusCounts["Delivered"]}, {statusCounts["Cancelled"]}]";
            }
            catch
            {
                return "[0, 0, 0, 0]";
            }
        }

        // Méthode pour obtenir le top 5 des produits
        public string GetTopProducts()
        {
            try
            {
                DbContext db = new DbContext();
                string query = @"
                    SELECT TOP 5 
                        p.Name,
                        ISNULL(SUM(oi.Quantity), 0) as TotalSold
                    FROM Products p
                    INNER JOIN OrderItems oi ON p.Id = oi.ProductId
                    INNER JOIN Orders o ON oi.OrderId = o.Id 
                    WHERE p.IsActive = 1
                        AND o.Status != 'Cancelled'
                        AND (o.IsArchived IS NULL OR o.IsArchived = 0)
                    GROUP BY p.Id, p.Name
                    ORDER BY TotalSold DESC, p.Name ASC";
                
                DataTable dt = db.ExecuteQuery(query);
                System.Collections.Generic.List<string> labels = new System.Collections.Generic.List<string>();
                System.Collections.Generic.List<int> data = new System.Collections.Generic.List<int>();
                
                foreach (DataRow row in dt.Rows)
                {
                    string name = row["Name"].ToString();
                    if (name.Length > 25) name = name.Substring(0, 25) + "...";
                    labels.Add(name.Replace("\"", "\\\"").Replace("'", "\\'"));
                    int totalSold = Convert.ToInt32(row["TotalSold"]);
                    data.Add(totalSold);
                }
                
                // Remplir jusqu'à 5 produits si nécessaire
                while (labels.Count < 5)
                {
                    labels.Add("-");
                    data.Add(0);
                }
                
                // Créer le JSON
                System.Text.StringBuilder json = new System.Text.StringBuilder();
                json.Append("{\"labels\":[");
                for (int i = 0; i < labels.Count; i++)
                {
                    if (i > 0) json.Append(",");
                    json.Append("\"").Append(labels[i]).Append("\"");
                }
                json.Append("],\"data\":[");
                for (int i = 0; i < data.Count; i++)
                {
                    if (i > 0) json.Append(",");
                    json.Append(data[i]);
                }
                json.Append("]}");
                
                return json.ToString();
            }
            catch (Exception ex)
            {
                // Log l'erreur pour debug
                System.Diagnostics.Debug.WriteLine("GetTopProducts Error: " + ex.Message);
                return "{\"labels\":[\"-\",\"-\",\"-\",\"-\",\"-\"],\"data\":[0,0,0,0,0]}";
            }
        }
    }
}
