using System;
using System.Configuration;
using System.Text;

namespace Ecommerce.Utils
{
    /// <summary>
    /// Helper class for generating beautiful HTML email templates
    /// </summary>
    public static class EmailTemplates
    {
        private static string GetLogoUrl()
        {
            // Utiliser un logo en SVG encodé en base64 pour éviter les problèmes d'affichage
            return "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjYwIiB2aWV3Qm94PSIwIDAgMjAwIDYwIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxyZWN0IHdpZHRoPSIyMDAiIGhlaWdodD0iNjAiIGZpbGw9IiMxNmE" +
"zNGEiIHJ4PSI1Ii8+PHRleHQgeD0iMTAwIiB5PSIzNSIgZm9udC1mYW1pbHk9IkFyaWFsLCBzYW5zLXNlcmlmIiBmb250LXNpemU9IjI0IiBmb250LXdlaWdodD0iYm9sZCIgZmlsbD0id2hpdGUiIHRleHQtYW5jaG9yPSJtaWRkbGUiPkUtY29tbWVyY2U8L3RleHQ+PC9zdmc+";
        }

        private static string GetEmailHeader()
        {
            return $@"
<!DOCTYPE html>
<html lang='fr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ margin: 0; padding: 0; font-family: 'Inter', 'Segoe UI', Arial, sans-serif; background-color: #f3f4f6; }}
        .email-container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #22c55e 0%, #16a34a 100%); padding: 30px 20px; text-align: center; }}
        .header img {{ height: 50px; }}
        .content {{ padding: 40px 30px; color: #374151; }}
        .content h2 {{ color: #111827; margin: 0 0 20px 0; font-size: 24px; }}
        .content p {{ line-height: 1.6; margin: 0 0 15px 0; font-size: 15px; }}
        .status-badge {{ display: inline-block; padding: 8px 16px; border-radius: 20px; font-weight: 600; font-size: 14px; margin: 10px 0; }}
        .status-pending {{ background-color: #fef3c7; color: #92400e; }}
        .status-processing {{ background-color: #dbeafe; color: #1e40af; }}
        .status-shipped {{ background-color: #e0e7ff; color: #4338ca; }}
        .status-delivered {{ background-color: #d1fae5; color: #065f46; }}
        .status-cancelled {{ background-color: #fee2e2; color: #991b1b; }}
        .order-box {{ background-color: #f9fafb; border: 1px solid #e5e7eb; border-radius: 8px; padding: 20px; margin: 20px 0; }}
        .order-box h3 {{ margin: 0 0 15px 0; font-size: 18px; color: #111827; }}
        .product-table {{ width: 100%; border-collapse: collapse; margin: 15px 0; }}
        .product-table th {{ background-color: #f3f4f6; padding: 10px; text-align: left; border-bottom: 2px solid #e5e7eb; font-size: 13px; }}
        .product-table td {{ padding: 12px 10px; border-bottom: 1px solid #e5e7eb; font-size: 14px; }}
        .product-img {{ width: 60px; height: 60px; object-fit: cover; border-radius: 6px; }}
        .total-row {{ font-weight: bold; background-color: #f9fafb; }}
        .button {{ display: inline-block; padding: 12px 28px; background: linear-gradient(135deg, #22c55e 0%, #16a34a 100%); color: #ffffff; text-decoration: none; border-radius: 6px; font-weight: 600; margin: 20px 0; }}
        .footer {{ background-color: #f9fafb; padding: 30px 20px; text-align: center; color: #6b7280; font-size: 13px; border-top: 1px solid #e5e7eb; }}
        .footer a {{ color: #22c55e; text-decoration: none; }}
        .divider {{ height: 1px; background-color: #e5e7eb; margin: 25px 0; }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <img src='{GetLogoUrl()}' alt='E-commerce Platform' />
        </div>
        <div class='content'>";
        }

        private static string GetEmailFooter()
        {
            return @"
        </div>
        <div class='footer'>
            <p><strong>Besoin d'aide ?</strong></p>
            <p>Contactez-nous à <a href='mailto:contact@ecommerce.ma'>contact@ecommerce.ma</a> ou au +212 6XX XXX XXX</p>
            <p style='margin-top: 20px; color: #9ca3af;'>
                © 2025 E-commerce Platform. Tous droits réservés.<br/>
                Cet email a été envoyé automatiquement, merci de ne pas y répondre.
            </p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Get status badge HTML
        /// </summary>
        private static string GetStatusBadge(string status)
        {
            string className = "status-badge ";
            string label = status;

            switch (status)
            {
                case "Pending":
                    className += "status-pending";
                    label = "En attente";
                    break;
                case "Processing":
                    className += "status-processing";
                    label = "En préparation";
                    break;
                case "Shipped":
                    className += "status-shipped";
                    label = "Expédié";
                    break;
                case "Delivered":
                    className += "status-delivered";
                    label = "Livré";
                    break;
                case "Cancelled":
                    className += "status-cancelled";
                    label = "Annulé";
                    break;
            }

            return $"<span class='{className}'>{label}</span>";
        }

        /// <summary>
        /// Generate order status update email with product details
        /// </summary>
        public static string GetOrderStatusEmailTemplate(
            string fullName, 
            string orderNumber, 
            string status, 
            string productsHtml, 
            string cancelReason = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetEmailHeader());

            sb.Append($"<h2>Bonjour {System.Web.HttpUtility.HtmlEncode(fullName)},</h2>");
            sb.Append($"<p>Le statut de votre commande <strong>{System.Web.HttpUtility.HtmlEncode(orderNumber)}</strong> a été mis à jour.</p>");
            
            sb.Append($"<p>Nouveau statut : {GetStatusBadge(status)}</p>");

            if (!string.IsNullOrEmpty(cancelReason))
            {
                sb.Append($@"
                <div class='order-box' style='background-color: #fef2f2; border-color: #fca5a5;'>
                    <h3 style='color: #991b1b;'>Raison de l'annulation</h3>
                    <p>{System.Web.HttpUtility.HtmlEncode(cancelReason)}</p>
                </div>");
            }

            if (!string.IsNullOrEmpty(productsHtml))
            {
                sb.Append("<div class='order-box'>");
                sb.Append("<h3>🛒 Articles commandés</h3>");
                sb.Append(productsHtml);
                sb.Append("</div>");
            }

            // Status-specific messages
            switch (status)
            {
                case "Processing":
                    sb.Append("<p>✨ Bonne nouvelle ! Votre commande est en cours de préparation dans nos entrepôts.</p>");
                    break;
                case "Shipped":
                    sb.Append("<p>📦 Votre commande a été expédiée ! Vous pouvez suivre votre colis depuis votre profil.</p>");
                    break;
                case "Delivered":
                    sb.Append("<p>🎉 Votre commande a été livrée ! Nous espérons que vous êtes satisfait de vos achats.</p>");
                    sb.Append("<p>N'hésitez pas à laisser un avis sur vos produits pour aider d'autres clients.</p>");
                    break;
                case "Cancelled":
                    sb.Append("<p>Votre commande a été annulée. Si vous avez des questions, n'hésitez pas à nous contacter.</p>");
                    break;
            }

            sb.Append("<a href='#' class='button'>Voir ma commande</a>");

            sb.Append(GetEmailFooter());
            return sb.ToString();
        }

        /// <summary>
        /// Generate product table HTML for emails
        /// </summary>
        public static string GenerateProductTableHtml(System.Data.DataTable products, bool includeTotal = false)
        {
            if (products == null || products.Rows.Count == 0)
                return "<p>Aucun produit</p>";

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
            <table class='product-table'>
                <thead>
                    <tr>
                        <th>Produit</th>
                        <th style='text-align: center;'>Quantité</th>
                        <th style='text-align: right;'>Prix unitaire</th>
                        <th style='text-align: right;'>Total</th>
                    </tr>
                </thead>
                <tbody>");

            decimal grandTotal = 0;

            foreach (System.Data.DataRow row in products.Rows)
            {
                string productName = row["ProductName"].ToString();
                int quantity = Convert.ToInt32(row["Quantity"]);
                decimal unitPrice = Convert.ToDecimal(row["UnitPrice"]);
                decimal totalPrice = Convert.ToDecimal(row["TotalPrice"]);
                grandTotal += totalPrice;

                sb.Append($@"
                    <tr>
                        <td><strong>{System.Web.HttpUtility.HtmlEncode(productName)}</strong></td>
                        <td style='text-align: center;'>{quantity}</td>
                        <td style='text-align: right;'>{unitPrice:F2} MAD</td>
                        <td style='text-align: right;'><strong>{totalPrice:F2} MAD</strong></td>
                    </tr>");
            }

            if (includeTotal)
            {
                sb.Append($@"
                    <tr class='total-row'>
                        <td colspan='3' style='text-align: right; padding-top: 15px;'>Total</td>
                        <td style='text-align: right; padding-top: 15px;'>{grandTotal:F2} MAD</td>
                    </tr>");
            }

            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        /// <summary>
        /// Generate complaint response email
        /// </summary>
        public static string GetComplaintResponseEmailTemplate(
            string userName, 
            string complaintSubject, 
            string adminResponse)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetEmailHeader());

            sb.Append($"<h2>Bonjour {System.Web.HttpUtility.HtmlEncode(userName)},</h2>");
            sb.Append("<p>Nous avons répondu à votre réclamation.</p>");

            sb.Append("<div class='order-box'>");
            sb.Append($"<h3>📋 Sujet de votre réclamation</h3>");
            sb.Append($"<p>{System.Web.HttpUtility.HtmlEncode(complaintSubject)}</p>");
            sb.Append("</div>");

            sb.Append("<div class='order-box' style='background-color: #f0fdf4; border-color: #86efac;'>");
            sb.Append("<h3 style='color: #166534;'>💬 Réponse de notre équipe</h3>");
            sb.Append($"<p>{System.Web.HttpUtility.HtmlEncode(adminResponse)}</p>");
            sb.Append("</div>");

            sb.Append("<p>Si vous avez d'autres questions, n'hésitez pas à nous contacter.</p>");
            sb.Append("<a href='#' class='button'>Voir mes réclamations</a>");

            sb.Append(GetEmailFooter());
            return sb.ToString();
        }

        /// <summary>
        /// Generate account status change email (activation/deactivation)
        /// </summary>
        public static string GetAccountStatusEmailTemplate(
            string userName,
            bool isActivated)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetEmailHeader());

            if (isActivated)
            {
                sb.Append($"<h2>Bonjour {System.Web.HttpUtility.HtmlEncode(userName)},</h2>");
                sb.Append("<div class='order-box' style='background-color: #f0fdf4; border-color: #86efac;'>");
                sb.Append("<h3 style='color: #166534;'>✅ Votre compte a été réactivé</h3>");
                sb.Append("<p>Nous sommes heureux de vous informer que votre compte a été réactivé avec succès.</p>");
                sb.Append("<p>Vous pouvez maintenant vous connecter et accéder à tous les services de notre plateforme.</p>");
                sb.Append("</div>");
                sb.Append("<p>Si vous avez des questions ou besoin d'assistance, n'hésitez pas à nous contacter.</p>");
                sb.Append("<a href='#' class='button'>Se connecter</a>");
            }
            else
            {
                sb.Append($"<h2>Bonjour {System.Web.HttpUtility.HtmlEncode(userName)},</h2>");
                sb.Append("<div class='order-box' style='background-color: #fef2f2; border-color: #fecaca;'>");
                sb.Append("<h3 style='color: #991b1b;'>⚠️ Votre compte a été désactivé</h3>");
                sb.Append("<p>Nous vous informons que votre compte a été désactivé par l'administrateur.</p>");
                sb.Append("<p><strong>Vous ne pouvez plus vous connecter à votre compte jusqu'à ce qu'il soit réactivé.</strong></p>");
                sb.Append("</div>");
                sb.Append("<p>Si vous pensez qu'il s'agit d'une erreur ou si vous avez des questions concernant cette décision, veuillez contacter notre service client.</p>");
                sb.Append("<p><strong>Contact :</strong></p>");
                sb.Append("<ul style='margin: 15px 0; padding-left: 20px;'>");
                sb.Append("<li>Email : <a href='mailto:contact@ecommerce.ma'>contact@ecommerce.ma</a></li>");
                sb.Append("<li>Téléphone : +212 6XX XXX XXX</li>");
                sb.Append("</ul>");
            }

            sb.Append(GetEmailFooter());
            return sb.ToString();
        }

        /// <summary>
        /// Generate order confirmation email
        /// </summary>
        public static string GetOrderConfirmationEmailTemplate(
            string fullName, 
            string orderNumber, 
            string productsHtml, 
            decimal total)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetEmailHeader());

            sb.Append($"<h2>Merci pour votre commande, {System.Web.HttpUtility.HtmlEncode(fullName)} ! 🎉</h2>");
            sb.Append($"<p>Votre commande <strong>{System.Web.HttpUtility.HtmlEncode(orderNumber)}</strong> a été confirmée avec succès.</p>");
            
            sb.Append($"<p>Statut actuel : {GetStatusBadge("Pending")}</p>");

            if (!string.IsNullOrEmpty(productsHtml))
            {
                sb.Append("<div class='order-box'>");
                sb.Append("<h3>🛒 Récapitulatif de votre commande</h3>");
                sb.Append(productsHtml);
                sb.Append($"<div class='divider'></div>");
                sb.Append($"<p style='text-align: right; font-size: 18px;'><strong>Total : {total:F2} MAD</strong></p>");
                sb.Append("</div>");
            }

            sb.Append("<p>Nous préparons votre commande et vous tiendrons informé(e) de son évolution.</p>");
            sb.Append("<a href='#' class='button'>Suivre ma commande</a>");

            sb.Append(GetEmailFooter());
            return sb.ToString();
        }

        /// <summary>
        /// Generate contact message notification for admins
        /// </summary>
        public static string GetContactMessageEmailTemplate(
            string senderName, 
            string senderEmail, 
            string subject, 
            string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetEmailHeader());

            sb.Append("<h2>📧 Nouveau message de contact</h2>");

            sb.Append("<div class='order-box'>");
            sb.Append($"<p><strong>De :</strong> {System.Web.HttpUtility.HtmlEncode(senderName)} ({System.Web.HttpUtility.HtmlEncode(senderEmail)})</p>");
            sb.Append($"<p><strong>Sujet :</strong> {System.Web.HttpUtility.HtmlEncode(subject)}</p>");
            sb.Append("</div>");

            sb.Append("<div class='order-box' style='background-color: #f0fdf4;'>");
            sb.Append("<h3>Message</h3>");
            sb.Append($"<p>{System.Web.HttpUtility.HtmlEncode(message)}</p>");
            sb.Append("</div>");

            sb.Append(GetEmailFooter());
            return sb.ToString();
        }
    }
}
