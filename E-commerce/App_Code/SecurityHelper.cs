using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

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
    }
}
