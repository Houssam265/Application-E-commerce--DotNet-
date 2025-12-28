using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Ecommerce.Data;

namespace Ecommerce.Utils
{
    /// <summary>
    /// Helper class for generating PDF invoices
    /// </summary>
    public static class InvoiceHelper
    {
        /// <summary>
        /// Generate invoice HTML for an order
        /// </summary>
        public static string GenerateInvoiceHtml(int orderId)
        {
            DbContext db = new DbContext();
            
            // Get order details (try Orders first, then OrderHistory if archived)
            string orderQuery = @"
                SELECT O.*, U.FullName, U.Email, A.Street, A.City, A.ZipCode, A.Country
                FROM Orders O
                INNER JOIN Users U ON O.UserId = U.Id
                LEFT JOIN Addresses A ON O.ShippingAddressId = A.Id
                WHERE O.Id = @OrderId";
            
            DataTable orderDt = db.ExecuteQuery(orderQuery, new SqlParameter[] { new SqlParameter("@OrderId", orderId) });
            
            // If not found in Orders, try OrderHistory
            if (orderDt.Rows.Count == 0)
            {
                orderQuery = @"
                    SELECT 
                        OH.OrderId as Id, OH.UserId, OH.OrderNumber, OH.TotalAmount, OH.Status,
                        OH.OrderDate, NULL as ShippingAddressId, NULL as SubTotal, NULL as ShippingCost,
                        NULL as PaymentMethod, NULL as PaymentStatus, NULL as CreatedAt, NULL as UpdatedAt,
                        NULL as IsArchived, NULL as ArchivedAt, NULL as StatusLockedAt, OH.Notes,
                        U2.FullName, U2.Email, NULL as Street, NULL as City, NULL as ZipCode, NULL as Country
                    FROM OrderHistory OH
                    INNER JOIN Users U2 ON OH.UserId = U2.Id
                    WHERE OH.OrderId = @OrderId";
                orderDt = db.ExecuteQuery(orderQuery, new SqlParameter[] { new SqlParameter("@OrderId", orderId) });
            }
            
            if (orderDt.Rows.Count == 0)
                return null;
                
            DataRow order = orderDt.Rows[0];
            
            // Get order items
            string itemsQuery = "SELECT * FROM OrderItems WHERE OrderId = @OrderId";
            DataTable items = db.ExecuteQuery(itemsQuery, new SqlParameter[] { new SqlParameter("@OrderId", orderId) });
            
            StringBuilder html = new StringBuilder();
            
            html.Append(@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; color: #333; }
        .header { text-align: center; margin-bottom: 30px; border-bottom: 3px solid #16a34a; padding-bottom: 20px; }
        .header h1 { color: #16a34a; margin: 0; font-size: 32px; }
        .header p { margin: 5px 0; color: #666; }
        .info-section { display: flex; justify-content: space-between; margin-bottom: 30px; }
        .info-box { width: 48%; }
        .info-box h3 { color: #16a34a; border-bottom: 2px solid #e5e7eb; padding-bottom: 5px; margin-bottom: 10px; }
        .info-box p { margin: 5px 0; line-height: 1.6; }
        table { width: 100%; border-collapse: collapse; margin: 20px 0; }
        thead { background-color: #f3f4f6; }
        th { background-color: #16a34a; color: white; padding: 12px; text-align: left; font-weight: bold; }
        td { padding: 10px; border-bottom: 1px solid #e5e7eb; }
        tr:hover { background-color: #f9fafb; }
        .total-section { text-align: right; margin-top: 20px; }
        .total-section table { width: 300px; float: right; }
        .total-section td:first-child { font-weight: bold; }
        .grand-total { font-size: 18px; font-weight: bold; color: #16a34a; background-color: #f0fdf4; }
        .footer { margin-top: 50px; padding-top: 20px; border-top: 2px solid #e5e7eb; text-align: center; color: #666; font-size: 12px; }
        @media print {
            body { margin: 0; }
            .no-print { display: none; }
        }
    </style>
</head>
<body>
    <div class='header'>
        <h1>FACTURE</h1>
        <p>E-commerce Platform - Produits du terroir marocain</p>
        <p>Email: contact@ecommerce.ma | Tél: +212 6XX XXX XXX</p>
    </div>
    
    <div class='info-section'>
        <div class='info-box'>
            <h3>Informations de facturation</h3>
            <p><strong>N° Facture:</strong> " + HttpUtility.HtmlEncode(order["OrderNumber"].ToString()) + @"</p>
            <p><strong>Date:</strong> " + Convert.ToDateTime(order["OrderDate"]).ToString("dd/MM/yyyy") + @"</p>
            <p><strong>Statut:</strong> " + GetStatusLabel(order["Status"].ToString()) + @"</p>
        </div>
        
        <div class='info-box'>
            <h3>Client</h3>
            <p><strong>Nom:</strong> " + HttpUtility.HtmlEncode(order["FullName"].ToString()) + @"</p>
            <p><strong>Email:</strong> " + HttpUtility.HtmlEncode(order["Email"].ToString()) + @"</p>");
            
            if (order["Street"] != DBNull.Value)
            {
                html.Append(@"
            <p><strong>Adresse:</strong><br/>
            " + HttpUtility.HtmlEncode(order["Street"].ToString()) + @"<br/>
            " + HttpUtility.HtmlEncode(order["ZipCode"].ToString()) + " " + HttpUtility.HtmlEncode(order["City"].ToString()) + @"<br/>
            " + HttpUtility.HtmlEncode(order["Country"].ToString()) + @"</p>");
            }
            
            html.Append(@"
        </div>
    </div>
    
    <h3>Articles commandés</h3>
    <table>
        <thead>
            <tr>
                <th>Produit</th>
                <th style='text-align: center;'>Quantité</th>
                <th style='text-align: right;'>Prix unitaire</th>
                <th style='text-align: right;'>Total</th>
            </tr>
        </thead>
        <tbody>");
        
            foreach (DataRow item in items.Rows)
            {
                html.Append(@"
            <tr>
                <td>" + HttpUtility.HtmlEncode(item["ProductName"].ToString()) + @"</td>
                <td style='text-align: center;'>" + item["Quantity"] + @"</td>
                <td style='text-align: right;'>" + Convert.ToDecimal(item["UnitPrice"]).ToString("F2") + @" MAD</td>
                <td style='text-align: right;'>" + Convert.ToDecimal(item["TotalPrice"]).ToString("F2") + @" MAD</td>
            </tr>");
            }
            
            decimal subtotal = Convert.ToDecimal(order["SubTotal"]);
            decimal shipping = Convert.ToDecimal(order["ShippingCost"]);
            decimal total = Convert.ToDecimal(order["TotalAmount"]);
            
            html.Append(@"
        </tbody>
    </table>
    
    <div class='total-section'>
        <table>
            <tr>
                <td>Sous-total:</td>
                <td style='text-align: right;'>" + subtotal.ToString("F2") + @" MAD</td>
            </tr>
            <tr>
                <td>Frais de livraison:</td>
                <td style='text-align: right;'>" + shipping.ToString("F2") + @" MAD</td>
            </tr>
            <tr class='grand-total'>
                <td>TOTAL:</td>
                <td style='text-align: right;'>" + total.ToString("F2") + @" MAD</td>
            </tr>
        </table>
    </div>
    <div style='clear: both;'></div>
    
    <div class='footer'>
        <p>Merci pour votre confiance !</p>
        <p>© 2025 E-commerce Platform - Tous droits réservés</p>
        <p>Cette facture a été générée électroniquement et est valable sans signature.</p>
    </div>
</body>
</html>");
            
            return html.ToString();
        }
        
        /// <summary>
        /// Generate invoice PDF for an order using iTextSharp
        /// </summary>
        public static byte[] GenerateInvoicePdf(int orderId)
        {
            DbContext db = new DbContext();
            
            // Get order details (try Orders first, then OrderHistory if archived)
            string orderQuery = @"
                SELECT O.*, U.FullName, U.Email, A.Street, A.City, A.ZipCode, A.Country
                FROM Orders O
                INNER JOIN Users U ON O.UserId = U.Id
                LEFT JOIN Addresses A ON O.ShippingAddressId = A.Id
                WHERE O.Id = @OrderId";
            
            DataTable orderDt = db.ExecuteQuery(orderQuery, new SqlParameter[] { new SqlParameter("@OrderId", orderId) });
            
            // If not found in Orders, try OrderHistory
            if (orderDt.Rows.Count == 0)
            {
                orderQuery = @"
                    SELECT 
                        OH.OrderId as Id, OH.UserId, OH.OrderNumber, OH.TotalAmount, OH.Status,
                        OH.OrderDate, NULL as ShippingAddressId, NULL as SubTotal, NULL as ShippingCost,
                        NULL as PaymentMethod, NULL as PaymentStatus, NULL as CreatedAt, NULL as UpdatedAt,
                        NULL as IsArchived, NULL as ArchivedAt, NULL as StatusLockedAt, OH.Notes,
                        U2.FullName, U2.Email, NULL as Street, NULL as City, NULL as ZipCode, NULL as Country
                    FROM OrderHistory OH
                    INNER JOIN Users U2 ON OH.UserId = U2.Id
                    WHERE OH.OrderId = @OrderId";
                orderDt = db.ExecuteQuery(orderQuery, new SqlParameter[] { new SqlParameter("@OrderId", orderId) });
            }
            
            if (orderDt.Rows.Count == 0)
                return null;
                
            DataRow order = orderDt.Rows[0];
            
            // Get order items
            string itemsQuery = "SELECT * FROM OrderItems WHERE OrderId = @OrderId";
            DataTable items = db.ExecuteQuery(itemsQuery, new SqlParameter[] { new SqlParameter("@OrderId", orderId) });
            
            // Create PDF document
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                
                document.Open();
                
                // Set fonts
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font titleFont = new Font(baseFont, 24, Font.BOLD, new BaseColor(22, 163, 74));
                Font headerFont = new Font(baseFont, 14, Font.BOLD, new BaseColor(22, 163, 74));
                Font normalFont = new Font(baseFont, 10, Font.NORMAL);
                Font boldFont = new Font(baseFont, 10, Font.BOLD);
                
                // Title
                Paragraph title = new Paragraph("FACTURE", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 10f;
                document.Add(title);
                
                // Company info
                Paragraph companyInfo = new Paragraph("E-commerce Platform - Produits du terroir marocain\nEmail: contact@ecommerce.ma | Tél: +212 6XX XXX XXX", normalFont);
                companyInfo.Alignment = Element.ALIGN_CENTER;
                companyInfo.SpacingAfter = 20f;
                document.Add(companyInfo);
                
                // Line separator
                document.Add(new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator())));
                document.Add(new Paragraph(" "));
                
                // Invoice and client info in two columns
                PdfPTable infoTable = new PdfPTable(2);
                infoTable.WidthPercentage = 100;
                infoTable.SetWidths(new float[] { 1, 1 });
                
                // Invoice info
                PdfPCell invoiceCell = new PdfPCell();
                invoiceCell.Border = PdfPCell.NO_BORDER;
                invoiceCell.AddElement(new Paragraph("Informations de facturation", headerFont));
                invoiceCell.AddElement(new Paragraph("N° Facture: " + HttpUtility.HtmlEncode(order["OrderNumber"].ToString()), normalFont));
                invoiceCell.AddElement(new Paragraph("Date: " + Convert.ToDateTime(order["OrderDate"]).ToString("dd/MM/yyyy"), normalFont));
                invoiceCell.AddElement(new Paragraph("Statut: " + GetStatusLabel(order["Status"].ToString()), normalFont));
                infoTable.AddCell(invoiceCell);
                
                // Client info
                PdfPCell clientCell = new PdfPCell();
                clientCell.Border = PdfPCell.NO_BORDER;
                clientCell.AddElement(new Paragraph("Client", headerFont));
                clientCell.AddElement(new Paragraph("Nom: " + HttpUtility.HtmlEncode(order["FullName"].ToString()), normalFont));
                clientCell.AddElement(new Paragraph("Email: " + HttpUtility.HtmlEncode(order["Email"].ToString()), normalFont));
                
                if (order["Street"] != DBNull.Value && order["Street"] != null)
                {
                    clientCell.AddElement(new Paragraph("Adresse:", normalFont));
                    clientCell.AddElement(new Paragraph(HttpUtility.HtmlEncode(order["Street"].ToString()), normalFont));
                    clientCell.AddElement(new Paragraph(HttpUtility.HtmlEncode(order["ZipCode"].ToString()) + " " + HttpUtility.HtmlEncode(order["City"].ToString()), normalFont));
                    clientCell.AddElement(new Paragraph(HttpUtility.HtmlEncode(order["Country"].ToString()), normalFont));
                }
                
                infoTable.AddCell(clientCell);
                document.Add(infoTable);
                
                document.Add(new Paragraph(" "));
                
                // Items table
                Paragraph itemsTitle = new Paragraph("Articles commandés", headerFont);
                itemsTitle.SpacingAfter = 10f;
                document.Add(itemsTitle);
                
                PdfPTable itemsTable = new PdfPTable(4);
                itemsTable.WidthPercentage = 100;
                itemsTable.SetWidths(new float[] { 3, 1, 1.5f, 1.5f });
                
                // Table headers
                PdfPCell headerCell = new PdfPCell(new Phrase("Produit", boldFont));
                headerCell.BackgroundColor = new BaseColor(22, 163, 74);
                headerCell.HorizontalAlignment = Element.ALIGN_LEFT;
                headerCell.Padding = 8f;
                itemsTable.AddCell(headerCell);
                
                headerCell = new PdfPCell(new Phrase("Quantité", boldFont));
                headerCell.BackgroundColor = new BaseColor(22, 163, 74);
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell.Padding = 8f;
                itemsTable.AddCell(headerCell);
                
                headerCell = new PdfPCell(new Phrase("Prix unitaire", boldFont));
                headerCell.BackgroundColor = new BaseColor(22, 163, 74);
                headerCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                headerCell.Padding = 8f;
                itemsTable.AddCell(headerCell);
                
                headerCell = new PdfPCell(new Phrase("Total", boldFont));
                headerCell.BackgroundColor = new BaseColor(22, 163, 74);
                headerCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                headerCell.Padding = 8f;
                itemsTable.AddCell(headerCell);
                
                // Table rows
                foreach (DataRow item in items.Rows)
                {
                    itemsTable.AddCell(new PdfPCell(new Phrase(HttpUtility.HtmlEncode(item["ProductName"].ToString()), normalFont)));
                    itemsTable.AddCell(new PdfPCell(new Phrase(item["Quantity"].ToString(), normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    itemsTable.AddCell(new PdfPCell(new Phrase(Convert.ToDecimal(item["UnitPrice"]).ToString("F2") + " MAD", normalFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    itemsTable.AddCell(new PdfPCell(new Phrase(Convert.ToDecimal(item["TotalPrice"]).ToString("F2") + " MAD", normalFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                }
                
                document.Add(itemsTable);
                
                document.Add(new Paragraph(" "));
                
                // Totals
                PdfPTable totalsTable = new PdfPTable(2);
                totalsTable.WidthPercentage = 40;
                totalsTable.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalsTable.SetWidths(new float[] { 1, 1 });
                
                decimal subtotal = Convert.ToDecimal(order["SubTotal"] ?? 0);
                decimal shipping = Convert.ToDecimal(order["ShippingCost"] ?? 0);
                decimal total = Convert.ToDecimal(order["TotalAmount"]);
                
                totalsTable.AddCell(new PdfPCell(new Phrase("Sous-total:", normalFont)) { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });
                totalsTable.AddCell(new PdfPCell(new Phrase(subtotal.ToString("F2") + " MAD", normalFont)) { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT });
                
                totalsTable.AddCell(new PdfPCell(new Phrase("Frais de livraison:", normalFont)) { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });
                totalsTable.AddCell(new PdfPCell(new Phrase(shipping.ToString("F2") + " MAD", normalFont)) { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT });
                
                PdfPCell totalLabelCell = new PdfPCell(new Phrase("TOTAL:", boldFont));
                totalLabelCell.Border = PdfPCell.NO_BORDER;
                totalLabelCell.HorizontalAlignment = Element.ALIGN_LEFT;
                totalLabelCell.BackgroundColor = new BaseColor(240, 253, 244);
                totalsTable.AddCell(totalLabelCell);
                
                PdfPCell totalValueCell = new PdfPCell(new Phrase(total.ToString("F2") + " MAD", boldFont));
                totalValueCell.Border = PdfPCell.NO_BORDER;
                totalValueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalValueCell.BackgroundColor = new BaseColor(240, 253, 244);
                totalsTable.AddCell(totalValueCell);
                
                document.Add(totalsTable);
                
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph(" "));
                
                // Footer
                Paragraph footer = new Paragraph("Merci pour votre confiance !\n© 2025 E-commerce Platform - Tous droits réservés\nCette facture a été générée électroniquement et est valable sans signature.", normalFont);
                footer.Alignment = Element.ALIGN_CENTER;
                footer.SpacingBefore = 20f;
                document.Add(footer);
                
                document.Close();
                
                return ms.ToArray();
            }
        }
        
        private static string GetStatusLabel(string status)
        {
            switch (status)
            {
                case "Pending": return "En attente";
                case "Processing": return "En préparation";
                case "Shipped": return "Expédié";
                case "Delivered": return "Livré";
                case "Cancelled": return "Annulé";
                default: return status;
            }
        }
    }
}
