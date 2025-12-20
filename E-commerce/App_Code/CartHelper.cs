using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Ecommerce.Data;

namespace Ecommerce.Utils
{
    public static class CartHelper
    {
        public static void AddToCart(int productId, int quantity = 1, int? variantId = null)
        {
            string sessionId = GetSessionId();
            int? userId = GetUserId();

            DbContext db = new DbContext();
            
            // Check if item already exists in cart
            string checkQuery = @"SELECT Id, Quantity FROM ShoppingCart 
                                  WHERE (UserId = @UserId OR SessionId = @SessionId) 
                                  AND ProductId = @ProductId 
                                  AND (@VariantId IS NULL OR VariantId = @VariantId)";
            
            SqlParameter[] checkParams = {
                new SqlParameter("@UserId", userId ?? (object)DBNull.Value),
                new SqlParameter("@SessionId", sessionId),
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@VariantId", variantId ?? (object)DBNull.Value)
            };

            DataTable dt = db.ExecuteQuery(checkQuery, checkParams);

            if (dt.Rows.Count > 0)
            {
                // Update quantity
                int cartId = Convert.ToInt32(dt.Rows[0]["Id"]);
                int currentQty = Convert.ToInt32(dt.Rows[0]["Quantity"]);
                int newQty = currentQty + quantity;

                string updateQuery = "UPDATE ShoppingCart SET Quantity = @Quantity, UpdatedAt = GETDATE() WHERE Id = @Id";
                SqlParameter[] updateParams = {
                    new SqlParameter("@Quantity", newQty),
                    new SqlParameter("@Id", cartId)
                };
                db.ExecuteNonQuery(updateQuery, updateParams);
            }
            else
            {
                // Insert new item
                string insertQuery = @"INSERT INTO ShoppingCart (UserId, SessionId, ProductId, VariantId, Quantity) 
                                       VALUES (@UserId, @SessionId, @ProductId, @VariantId, @Quantity)";
                SqlParameter[] insertParams = {
                    new SqlParameter("@UserId", userId ?? (object)DBNull.Value),
                    new SqlParameter("@SessionId", sessionId),
                    new SqlParameter("@ProductId", productId),
                    new SqlParameter("@VariantId", variantId ?? (object)DBNull.Value),
                    new SqlParameter("@Quantity", quantity)
                };
                db.ExecuteNonQuery(insertQuery, insertParams);
            }

            UpdateCartCount();
        }

        public static void RemoveFromCart(int cartItemId)
        {
            DbContext db = new DbContext();
            string query = "DELETE FROM ShoppingCart WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", cartItemId) };
            db.ExecuteNonQuery(query, parameters);
            UpdateCartCount();
        }

        public static void UpdateCartQuantity(int cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                RemoveFromCart(cartItemId);
                return;
            }

            DbContext db = new DbContext();
            string query = "UPDATE ShoppingCart SET Quantity = @Quantity, UpdatedAt = GETDATE() WHERE Id = @Id";
            SqlParameter[] parameters = {
                new SqlParameter("@Quantity", quantity),
                new SqlParameter("@Id", cartItemId)
            };
            db.ExecuteNonQuery(query, parameters);
            UpdateCartCount();
        }

        public static DataTable GetCartItems()
        {
            string sessionId = GetSessionId();
            int? userId = GetUserId();

            DbContext db = new DbContext();
            string query = @"SELECT c.Id, c.ProductId, c.VariantId, c.Quantity, 
                                     p.Name, p.Price, p.ImageUrl, p.StockQuantity,
                                     pv.PriceAdjustment, pv.VariantType, pv.VariantValue
                              FROM ShoppingCart c
                              INNER JOIN Products p ON c.ProductId = p.Id
                              LEFT JOIN ProductVariants pv ON c.VariantId = pv.Id
                              WHERE (c.UserId = @UserId OR c.SessionId = @SessionId)
                              ORDER BY c.CreatedAt DESC";
            
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", userId ?? (object)DBNull.Value),
                new SqlParameter("@SessionId", sessionId)
            };

            return db.ExecuteQuery(query, parameters);
        }

        public static decimal GetCartTotal()
        {
            DataTable cartItems = GetCartItems();
            decimal total = 0;

            foreach (DataRow row in cartItems.Rows)
            {
                decimal price = Convert.ToDecimal(row["Price"]);
                decimal adjustment = row["PriceAdjustment"] != DBNull.Value ? Convert.ToDecimal(row["PriceAdjustment"]) : 0;
                int quantity = Convert.ToInt32(row["Quantity"]);
                total += (price + adjustment) * quantity;
            }

            return total;
        }

        public static int GetCartItemCount()
        {
            string sessionId = GetSessionId();
            int? userId = GetUserId();

            DbContext db = new DbContext();
            string query = "SELECT SUM(Quantity) FROM ShoppingCart WHERE (UserId = @UserId OR SessionId = @SessionId)";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", userId ?? (object)DBNull.Value),
                new SqlParameter("@SessionId", sessionId)
            };

            object result = db.ExecuteScalar(query, parameters);
            return result != DBNull.Value && result != null ? Convert.ToInt32(result) : 0;
        }

        public static void ClearCart()
        {
            string sessionId = GetSessionId();
            int? userId = GetUserId();

            DbContext db = new DbContext();
            string query = "DELETE FROM ShoppingCart WHERE (UserId = @UserId OR SessionId = @SessionId)";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", userId ?? (object)DBNull.Value),
                new SqlParameter("@SessionId", sessionId)
            };
            db.ExecuteNonQuery(query, parameters);
            UpdateCartCount();
        }

        public static void MergeCartOnLogin(int userId)
        {
            string sessionId = GetSessionId();
            
            DbContext db = new DbContext();
            
            // Update session cart items to user cart
            string updateQuery = @"UPDATE ShoppingCart 
                                    SET UserId = @UserId, SessionId = NULL 
                                    WHERE SessionId = @SessionId AND UserId IS NULL";
            SqlParameter[] updateParams = {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@SessionId", sessionId)
            };
            db.ExecuteNonQuery(updateQuery, updateParams);
            
            UpdateCartCount();
        }

        private static string GetSessionId()
        {
            HttpContext context = HttpContext.Current;
            if (context.Session["SessionId"] == null)
            {
                context.Session["SessionId"] = Guid.NewGuid().ToString();
            }
            return context.Session["SessionId"].ToString();
        }

        private static int? GetUserId()
        {
            HttpContext context = HttpContext.Current;
            if (context.Session["UserId"] != null)
            {
                return Convert.ToInt32(context.Session["UserId"]);
            }
            return null;
        }

        private static void UpdateCartCount()
        {
            HttpContext context = HttpContext.Current;
            context.Session["CartCount"] = GetCartItemCount();
        }
    }
}

