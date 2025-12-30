using System;
using System.Web;
using System.Web.UI;
using Ecommerce.Data;

namespace Ecommerce.Pages.Public
{
    public partial class DownloadInvoice : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check user authentication
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Get order ID
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Response.Write("Invalid order ID");
                return;
            }

            int orderId;
            if (!int.TryParse(Request.QueryString["id"], out orderId))
            {
                Response.Write("Invalid order ID");
                return;
            }

            // Verify order belongs to user OR user is admin
            int userId = Convert.ToInt32(Session["UserId"]);
            string userRole = Session["Role"]?.ToString() ?? "";
            bool isAdmin = (userRole == "Admin");
            
            DbContext db = new DbContext();
            
            if (!isAdmin)
            {
                // For regular users, verify order belongs to them
                // Check in Orders table
                string verifyQuery1 = "SELECT COUNT(*) FROM Orders WHERE Id = @OrderId AND UserId = @UserId";
                object count1 = db.ExecuteScalar(verifyQuery1, new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@OrderId", orderId),
                    new System.Data.SqlClient.SqlParameter("@UserId", userId)
                });
                
                // Check in OrderHistory table
                string verifyQuery2 = "SELECT COUNT(*) FROM OrderHistory WHERE OrderId = @OrderId AND UserId = @UserId";
                object count2 = db.ExecuteScalar(verifyQuery2, new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@OrderId", orderId),
                    new System.Data.SqlClient.SqlParameter("@UserId", userId)
                });
                
                int orderCount = (count1 != null && count1 != DBNull.Value ? Convert.ToInt32(count1) : 0) +
                                (count2 != null && count2 != DBNull.Value ? Convert.ToInt32(count2) : 0);
                
                if (orderCount == 0)
                {
                    Response.Write("Order not found or access denied");
                    return;
                }
            }
            else
            {
                // For admins, just verify order exists
                string verifyQuery1 = "SELECT COUNT(*) FROM Orders WHERE Id = @OrderId";
                object count1 = db.ExecuteScalar(verifyQuery1, new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@OrderId", orderId)
                });
                
                string verifyQuery2 = "SELECT COUNT(*) FROM OrderHistory WHERE OrderId = @OrderId";
                object count2 = db.ExecuteScalar(verifyQuery2, new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@OrderId", orderId)
                });
                
                int orderCount = (count1 != null && count1 != DBNull.Value ? Convert.ToInt32(count1) : 0) +
                                (count2 != null && count2 != DBNull.Value ? Convert.ToInt32(count2) : 0);
                
                if (orderCount == 0)
                {
                    Response.Write("Order not found");
                    return;
                }
            }

            try
            {
                string format = Request.QueryString["format"]?.ToLower() ?? "html";
                
                if (format == "pdf")
                {
                    // Generate PDF invoice using iTextSharp
                    byte[] pdfBytes = Ecommerce.Utils.InvoiceHelper.GenerateInvoicePdf(orderId);
                    
                    if (pdfBytes == null)
                    {
                        Response.Write("Order not found");
                        return;
                    }

                    // Set response headers for PDF download
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", $"attachment; filename=Facture_{orderId}.pdf");
                    Response.BinaryWrite(pdfBytes);
                    Response.Flush();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    // Generate invoice HTML
                    string invoiceHtml = Ecommerce.Utils.InvoiceHelper.GenerateInvoiceHtml(orderId);
                    
                    if (invoiceHtml == null)
                    {
                        Response.Write("Order not found");
                        return;
                    }

                    // Set response headers for HTML download/print
                    Response.Clear();
                    Response.ContentType = "text/html";
                    Response.AddHeader("Content-Disposition", $"inline; filename=Facture_{orderId}.html");
                    Response.Write(invoiceHtml);
                    Response.Flush();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    return;
                }
            }
            catch (Exception ex)
            {
                Response.Write("Error generating invoice: " + Server.HtmlEncode(ex.Message));
            }
        }
    }
}
