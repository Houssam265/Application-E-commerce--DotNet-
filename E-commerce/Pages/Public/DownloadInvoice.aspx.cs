using System;
using System.Web;
using System.Web.UI;

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
                    Response.End();
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
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                Response.Write("Error generating invoice: " + Server.HtmlEncode(ex.Message));
            }
        }
    }
}
