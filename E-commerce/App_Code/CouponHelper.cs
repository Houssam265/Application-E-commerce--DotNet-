using System;
using System.Data;
using System.Data.SqlClient;
using Ecommerce.Data;

namespace Ecommerce.Utils
{
    public class CouponValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; }
        public int CouponId { get; set; }
    }

    public static class CouponHelper
    {
        /// <summary>
        /// Validates a coupon code and calculates the discount amount
        /// </summary>
        public static CouponValidationResult ValidateCoupon(string couponCode, decimal cartSubTotal)
        {
            var result = new CouponValidationResult
            {
                IsValid = false,
                DiscountAmount = 0
            };

            if (string.IsNullOrWhiteSpace(couponCode))
            {
                result.ErrorMessage = "Le code promo est requis.";
                return result;
            }

            couponCode = couponCode.Trim().ToUpper();

            try
            {
                DbContext db = new DbContext();

                // Get coupon details
                string query = @"SELECT Id, Code, DiscountType, DiscountValue, MinimumAmount, 
                                MaximumDiscount, StartDate, EndDate, UsageLimit, UsedCount, IsActive 
                                FROM Coupons 
                                WHERE Code = @Code";

                SqlParameter[] parameters = { new SqlParameter("@Code", couponCode) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count == 0)
                {
                    result.ErrorMessage = "Code promo invalide.";
                    return result;
                }

                DataRow row = dt.Rows[0];

                // Check if coupon is active
                if (!Convert.ToBoolean(row["IsActive"]))
                {
                    result.ErrorMessage = "Ce code promo n'est plus actif.";
                    return result;
                }

                // Check date validity
                DateTime startDate = Convert.ToDateTime(row["StartDate"]);
                DateTime endDate = Convert.ToDateTime(row["EndDate"]);
                DateTime now = DateTime.Now;

                if (now < startDate)
                {
                    result.ErrorMessage = "Ce code promo n'est pas encore valide.";
                    return result;
                }

                if (now > endDate)
                {
                    result.ErrorMessage = "Ce code promo a expiré.";
                    return result;
                }

                // Check usage limit
                int? usageLimit = row["UsageLimit"] != DBNull.Value ? (int?)Convert.ToInt32(row["UsageLimit"]) : null;
                int usedCount = Convert.ToInt32(row["UsedCount"]);

                if (usageLimit.HasValue && usedCount >= usageLimit.Value)
                {
                    result.ErrorMessage = "Ce code promo a atteint sa limite d'utilisation.";
                    return result;
                }

                // Check minimum amount
                decimal? minimumAmount = row["MinimumAmount"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["MinimumAmount"]) : null;

                if (minimumAmount.HasValue && cartSubTotal < minimumAmount.Value)
                {
                    result.ErrorMessage = $"Le montant minimum de {minimumAmount.Value:F2} MAD n'est pas atteint.";
                    return result;
                }

                // Calculate discount
                string discountType = row["DiscountType"].ToString();
                decimal discountValue = Convert.ToDecimal(row["DiscountValue"]);
                decimal discountAmount = 0;

                if (discountType == "Percentage")
                {
                    discountAmount = (cartSubTotal * discountValue) / 100;

                    // Apply maximum discount if specified
                    decimal? maxDiscount = row["MaximumDiscount"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["MaximumDiscount"]) : null;
                    if (maxDiscount.HasValue && discountAmount > maxDiscount.Value)
                    {
                        discountAmount = maxDiscount.Value;
                    }
                }
                else if (discountType == "Fixed")
                {
                    discountAmount = discountValue;
                    // Fixed discount cannot exceed cart total
                    if (discountAmount > cartSubTotal)
                    {
                        discountAmount = cartSubTotal;
                    }
                }

                // Success
                result.IsValid = true;
                result.DiscountAmount = discountAmount;
                result.DiscountType = discountType;
                result.CouponId = Convert.ToInt32(row["Id"]);
                result.ErrorMessage = string.Empty;

                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "Erreur lors de la validation du code promo: " + ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Records coupon usage when an order is placed
        /// </summary>
        public static void RecordCouponUsage(int couponId, int userId, int orderId, decimal discountAmount)
        {
            try
            {
                DbContext db = new DbContext();

                // Insert into CouponUsage table
                string usageQuery = @"INSERT INTO CouponUsage (CouponId, UserId, OrderId, DiscountAmount, UsedAt) 
                                     VALUES (@CouponId, @UserId, @OrderId, @DiscountAmount, GETDATE())";
                SqlParameter[] usageParams = {
                    new SqlParameter("@CouponId", couponId),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@OrderId", orderId),
                    new SqlParameter("@DiscountAmount", discountAmount)
                };
                db.ExecuteNonQuery(usageQuery, usageParams);

                // Increment UsedCount in Coupons table
                string updateQuery = "UPDATE Coupons SET UsedCount = UsedCount + 1 WHERE Id = @CouponId";
                SqlParameter[] updateParams = { new SqlParameter("@CouponId", couponId) };
                db.ExecuteNonQuery(updateQuery, updateParams);
            }
            catch (Exception)
            {
                // Log error but don't fail the order process
                // The discount has already been applied, so we continue
            }
        }
    }
}

