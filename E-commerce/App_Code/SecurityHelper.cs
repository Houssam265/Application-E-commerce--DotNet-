using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Ecommerce.Utils
{
    public static class SecurityHelper
    {
        // Hash password using SHA256
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty");

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Verify password
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            string hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hashedPassword) == 0;
        }

        // Sanitize input to prevent XSS
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Remove script tags
            input = Regex.Replace(input, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", "", RegexOptions.IgnoreCase);
            
            // Remove event handlers
            input = Regex.Replace(input, @"on\w+\s*=\s*[""'][^""']*[""']", "", RegexOptions.IgnoreCase);
            
            // HTML encode
            input = HttpUtility.HtmlEncode(input);
            
            return input;
        }

        // Validate email format
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Generate secure token
        public static string GenerateSecureToken(int length = 32)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[length];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData).Replace("+", "-").Replace("/", "_").Replace("=", "");
            }
        }

        // Validate phone number (Moroccan format)
        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return false;

            // Remove spaces and dashes
            phone = phone.Replace(" ", "").Replace("-", "");
            
            // Check Moroccan format: 06XXXXXXXX or +2126XXXXXXXX
            return Regex.IsMatch(phone, @"^(0|(\+212))[67]\d{8}$");
        }

        // Generate numeric verification code (e.g., 6 digits)
        public static string GenerateNumericCode(int digits = 6)
        {
            if (digits <= 0 || digits > 12) digits = 6;
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[8];
                rng.GetBytes(data);
                ulong value = BitConverter.ToUInt64(data, 0);
                var code = (value % (ulong)Math.Pow(10, digits)).ToString().PadLeft(digits, '0');
                return code;
            }
        }

        // Send email via SMTP using appSettings
        public static bool SendEmail(string to, string subject, string htmlBody)
        {
            try
            {
                string fromAddress = ConfigurationManager.AppSettings["MAIL_FROM_ADDRESS"];
                string fromName = ConfigurationManager.AppSettings["MAIL_FROM_NAME"] ?? "Service";
                string host = ConfigurationManager.AppSettings["MAIL_HOST"];
                int port = int.TryParse(ConfigurationManager.AppSettings["MAIL_PORT"], out var p) ? p : 587;
                string username = ConfigurationManager.AppSettings["MAIL_USERNAME"];
                string password = ConfigurationManager.AppSettings["MAIL_PASSWORD"];
                string encryption = ConfigurationManager.AppSettings["MAIL_ENCRYPTION"];

                var mail = new MailMessage();
                mail.From = new MailAddress(fromAddress, fromName, Encoding.UTF8);
                mail.To.Add(new MailAddress(to));
                mail.Subject = subject;
                mail.Body = htmlBody;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;

                using (var client = new SmtpClient(host, port))
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(username, password);
                    client.EnableSsl = string.Equals(encryption, "tls", StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(encryption, "ssl", StringComparison.OrdinalIgnoreCase);

                    client.Send(mail);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
