using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Ecommerce.Data;

namespace Ecommerce.Chatbot
{
    public class ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    [Serializable]
    public class ChatbotResponse
    {
        public string Reply { get; set; }
        public string[] Suggestions { get; set; }
        public string Error { get; set; }
    }

    internal class ChatCompletionMessage
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    internal class ChatCompletionChoice
    {
        public int index { get; set; }
        public ChatCompletionMessage message { get; set; }
        public string finish_reason { get; set; }
    }

    internal class ChatCompletionResponse
    {
        public List<ChatCompletionChoice> choices { get; set; }
    }

    // Gemini API Response Classes
    internal class GeminiContent
    {
        public string role { get; set; }
        public GeminiPart[] parts { get; set; }
    }

    internal class GeminiPart
    {
        public string text { get; set; }
    }

    internal class GeminiCandidate
    {
        public GeminiContent content { get; set; }
        public string finishReason { get; set; }
    }

    internal class GeminiResponse
    {
        public GeminiCandidate[] candidates { get; set; }
    }

    public class ChatbotLogic
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private readonly string _provider;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _endpoint;
        private readonly string _deploymentId;
        private readonly string _apiVersion;

        public ChatbotLogic()
        {
            _provider = ConfigurationManager.AppSettings["AI:Provider"]
                ?? ConfigurationManager.AppSettings["OpenAI:Provider"] ?? "OpenAI"; // or AzureOpenAI
            _apiKey = ConfigurationManager.AppSettings["AI:ApiKey"]
                ?? ConfigurationManager.AppSettings["OpenAI:ApiKey"] ?? string.Empty;
            _model = ConfigurationManager.AppSettings["AI:Model"]
                ?? ConfigurationManager.AppSettings["OpenAI:Model"] ?? "gpt-3.5-turbo";
            _endpoint = ConfigurationManager.AppSettings["AI:Endpoint"]
                ?? ConfigurationManager.AppSettings["OpenAI:Endpoint"] ?? string.Empty;
            _deploymentId = ConfigurationManager.AppSettings["OpenAI:DeploymentId"] ?? string.Empty;
            _apiVersion = ConfigurationManager.AppSettings["OpenAI:ApiVersion"] ?? "2023-05-15";
        }

        public ChatbotResponse ProcessMessage(string userMessage, HttpContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userMessage))
                {
                    return new ChatbotResponse { Reply = "Veuillez saisir un message.", Suggestions = new[] { "Lister les produits", "Suivre une commande", "Aide" } };
                }

                // Get user authentication status
                bool isAuthenticated = context.Session["UserId"] != null;
                int? userId = isAuthenticated ? (int?)context.Session["UserId"] : null;
                string userEmail = context.Session["UserEmail"]?.ToString();
                string userName = context.Session["FullName"]?.ToString();
                string userRole = context.Session["Role"]?.ToString();

                var history = GetHistory(context);
                history.Add(new ChatMessage { Role = "user", Content = userMessage });

                // Try quick intent resolution first (before LLM)
                var quickIntent = TryResolveIntentFromUserMessage(userMessage, isAuthenticated, userId);
                if (!string.IsNullOrEmpty(quickIntent))
                {
                    var quickReply = ExecuteIntent(quickIntent, context);
                    AppendAssistant(history, quickReply);
                    SaveHistory(context, history);
                    return new ChatbotResponse { Reply = quickReply, Suggestions = DefaultSuggestions() };
                }

                // Build context-aware system prompt
                var systemPrompt = BuildSystemPrompt(isAuthenticated, userId, userEmail, userName, userRole);
                var lastTurns = history.Skip(Math.Max(0, history.Count - 10)).ToList();

                var messages = new List<ChatCompletionMessage>
                {
                    new ChatCompletionMessage { role = "system", content = systemPrompt }
                };
                messages.AddRange(lastTurns.Select(m => new ChatCompletionMessage { role = m.Role, content = m.Content }));

                string assistant = CallLLM(messages);

                if (!string.IsNullOrEmpty(assistant))
                {
                    // Try to extract an intent JSON
                    var intentJson = ExtractIntentJson(assistant);
                    if (!string.IsNullOrEmpty(intentJson))
                    {
                        var reply = ExecuteIntent(intentJson, context);
                        AppendAssistant(history, reply);
                        SaveHistory(context, history);
                        return new ChatbotResponse { Reply = reply, Suggestions = DefaultSuggestions() };
                    }

                    // Fallback: echo assistant text
                    AppendAssistant(history, assistant);
                    SaveHistory(context, history);
                    return new ChatbotResponse { Reply = assistant, Suggestions = DefaultSuggestions() };
                }

                AppendAssistant(history, "Je n'ai pas pu générer de réponse.");
                SaveHistory(context, history);
                return new ChatbotResponse { Reply = "Je n'ai pas pu générer de réponse.", Suggestions = DefaultSuggestions() };
            }
            catch (Exception ex)
            {
                return new ChatbotResponse { Reply = "Désolé, une erreur s'est produite.", Error = ex.Message, Suggestions = DefaultSuggestions() };
            }
        }

        private void AppendAssistant(List<ChatMessage> history, string content)
        {
            history.Add(new ChatMessage { Role = "assistant", Content = content });
        }

        private List<ChatMessage> GetHistory(HttpContext context)
        {
            var history = context.Session["ChatHistory"] as List<ChatMessage>;
            if (history == null)
            {
                history = new List<ChatMessage>();
                context.Session["ChatHistory"] = history;
            }
            return history;
        }

        private void SaveHistory(HttpContext context, List<ChatMessage> history)
        {
            context.Session["ChatHistory"] = history;
        }

        private string[] DefaultSuggestions()
        {
            return new[] {
                "Rechercher un produit",
                "Mes commandes",
                "Produits populaires",
                "Catégories"
            };
        }

        private string BuildSystemPrompt(bool isAuthenticated, int? userId, string email, string name, string role)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Tu es un assistant e-commerce intelligent et serviable pour une boutique en ligne spécialisée en produits bio marocains (huiles d'argan, huile d'olive, miel, produits naturels, etc.).");
            sb.AppendLine("Réponds TOUJOURS en français, de manière polie, amicale et naturelle. Sois proactif et aide l'utilisateur au maximum.");
            
            // Add user context
            if (isAuthenticated)
            {
                sb.AppendLine("\n=== CONTEXTE UTILISATEUR ===");
                sb.AppendLine("L'utilisateur est CONNECTÉ et authentifié.");
                if (userId.HasValue)
                    sb.AppendLine($"UserId: {userId.Value}");
                if (!string.IsNullOrEmpty(email))
                    sb.AppendLine($"Email: {email}");
                if (!string.IsNullOrEmpty(name))
                    sb.AppendLine($"Nom: {name}");
                sb.AppendLine("Tu as accès COMPLET à toutes ses données: commandes, panier, wishlist, notifications, compte.");
                sb.AppendLine("Pour ses commandes, utilise automatiquement son userId - ne demande JAMAIS l'email ou le numéro.");
            }
            else
            {
                sb.AppendLine("\n=== CONTEXTE UTILISATEUR ===");
                sb.AppendLine("L'utilisateur est un INVITÉ (non connecté).");
                sb.AppendLine("Pour les commandes, tu dois demander le numéro de commande (ORD-xxxxx) OU l'email.");
            }
            
            sb.AppendLine("\n=== INSTRUCTIONS IMPORTANTES ===");
            sb.AppendLine("1. Quand la question nécessite des données SQL, produis UNIQUEMENT un JSON valide, sans texte autour:");
            sb.AppendLine("   {\"intent\": \"<intention>\", \"parameters\": { ... }}");
            sb.AppendLine("2. Si la question est générale (salutations, aide, informations sur le site), réponds directement en français SANS JSON.");
            sb.AppendLine("3. Sois intelligent: comprends les variantes de questions (ex: 'mes commandes' = 'mon historique' = 'mes achats').");
            sb.AppendLine("4. Pour les utilisateurs connectés, sois proactif: propose des produits, rappelle leurs commandes récentes, etc.");
            
            if (isAuthenticated)
            {
                sb.AppendLine("\n=== INTENTIONS DISPONIBLES (Utilisateur Connecté) ===");
                sb.AppendLine("COMMANDES:");
                sb.AppendLine("  - \"get_my_orders\"(limit) - Toutes mes commandes");
                sb.AppendLine("  - \"get_order_status\"(orderNumber) - Statut d'une commande spécifique");
                sb.AppendLine("  - \"get_order_details\"(orderNumber) - Détails complets d'une commande (items, adresse, tracking)");
                sb.AppendLine("  - \"get_pending_orders\"() - Mes commandes en attente");
                sb.AppendLine("  - \"get_recent_orders\"(limit) - Mes commandes récentes");
                sb.AppendLine("");
                sb.AppendLine("PRODUITS:");
                sb.AppendLine("  - \"search_products\"(name, categoryId, minPrice, maxPrice, limit) - Recherche avancée");
                sb.AppendLine("  - \"get_product_details\"(id|name) - Détails complets d'un produit");
                sb.AppendLine("  - \"get_products_by_category\"(categoryId|categoryName, limit) - Produits par catégorie");
                sb.AppendLine("  - \"top_products\"(limit) - Produits populaires");
                sb.AppendLine("  - \"featured_products\"(limit) - Produits en vedette");
                sb.AppendLine("  - \"low_stock_products\"(limit) - Produits en stock limité");
                sb.AppendLine("  - \"get_product_reviews\"(productId, limit) - Avis sur un produit");
                sb.AppendLine("");
                sb.AppendLine("CATÉGORIES:");
                sb.AppendLine("  - \"get_categories\"() - Liste toutes les catégories");
                sb.AppendLine("  - \"get_category_products\"(categoryId|categoryName, limit) - Produits d'une catégorie");
                sb.AppendLine("");
                sb.AppendLine("PANIER & WISHLIST:");
                sb.AppendLine("  - \"get_cart\"() - Contenu de mon panier");
                sb.AppendLine("  - \"get_cart_count\"() - Nombre d'articles dans le panier");
                sb.AppendLine("  - \"get_wishlist\"(limit) - Ma liste de souhaits");
                sb.AppendLine("");
                sb.AppendLine("COMPTE & NOTIFICATIONS:");
                sb.AppendLine("  - \"get_account_info\"() - Mes informations de compte");
                sb.AppendLine("  - \"get_notifications\"(limit, unreadOnly) - Mes notifications");
                sb.AppendLine("  - \"get_unread_notifications_count\"() - Nombre de notifications non lues");
            }
            else
            {
                sb.AppendLine("\n=== INTENTIONS DISPONIBLES (Invité) ===");
                sb.AppendLine("COMMANDES:");
                sb.AppendLine("  - \"get_order_status\"(orderNumber) - Statut d'une commande (nécessite ORD-xxxxx)");
                sb.AppendLine("  - \"get_user_orders\"(email, limit) - Commandes par email");
                sb.AppendLine("  - \"get_order_details\"(orderNumber) - Détails d'une commande");
                sb.AppendLine("");
                sb.AppendLine("PRODUITS:");
                sb.AppendLine("  - \"search_products\"(name, categoryId, minPrice, maxPrice, limit) - Recherche avancée");
                sb.AppendLine("  - \"get_product_details\"(id|name) - Détails complets d'un produit");
                sb.AppendLine("  - \"get_products_by_category\"(categoryId|categoryName, limit) - Produits par catégorie");
                sb.AppendLine("  - \"top_products\"(limit) - Produits populaires");
                sb.AppendLine("  - \"featured_products\"(limit) - Produits en vedette");
                sb.AppendLine("  - \"get_product_reviews\"(productId, limit) - Avis sur un produit");
                sb.AppendLine("");
                sb.AppendLine("CATÉGORIES:");
                sb.AppendLine("  - \"get_categories\"() - Liste toutes les catégories");
                sb.AppendLine("  - \"get_category_products\"(categoryId|categoryName, limit) - Produits d'une catégorie");
            }
            
            sb.AppendLine("\n=== BASE DE DONNÉES ===");
            sb.AppendLine("Tables disponibles:");
            sb.AppendLine("  - Products(Id, Name, Description, Price, StockQuantity, IsActive, CategoryId, ViewCount, IsFeatured)");
            sb.AppendLine("  - Categories(Id, Name, Description, IsActive)");
            sb.AppendLine("  - Orders(Id, OrderNumber, UserId, OrderDate, TotalAmount, Status, TrackingNumber, ShippingMethod)");
            sb.AppendLine("  - OrderItems(OrderId, ProductId, ProductName, Quantity, UnitPrice, TotalPrice)");
            sb.AppendLine("  - Users(Id, Email, FullName, Phone, Role)");
            sb.AppendLine("  - ShoppingCart(UserId, ProductId, Quantity)");
            sb.AppendLine("  - Wishlist(UserId, ProductId)");
            sb.AppendLine("  - Reviews(ProductId, UserId, Rating, Comment, ReviewDate)");
            sb.AppendLine("  - Notifications(UserId, Title, Message, Type, IsRead, CreatedAt)");
            sb.AppendLine("  - Addresses(UserId, FullName, Street, City, ZipCode, Country, Phone)");
            
            sb.AppendLine("\n=== EXEMPLES DE RÉPONSES INTELLIGENTES ===");
            sb.AppendLine("- Si l'utilisateur demande 'mes commandes', utilise get_my_orders automatiquement.");
            sb.AppendLine("- Si l'utilisateur demande 'où est ma commande', utilise get_order_status ou get_order_details.");
            sb.AppendLine("- Si l'utilisateur cherche un produit, propose plusieurs options avec search_products.");
            sb.AppendLine("- Sois proactif: si l'utilisateur mentionne un produit, propose des détails ou des alternatives.");
            sb.AppendLine("- Pour les questions générales (horaires, livraison, paiement), réponds directement sans JSON.");
            
            return sb.ToString();
        }

        private string CallLLM(List<ChatCompletionMessage> messages)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                return FallbackAnswer(messages);
            }

            try
            {
                // Handle Gemini API
                if (string.Equals(_provider, "Gemini", StringComparison.OrdinalIgnoreCase))
                {
                    return CallGeminiAPI(messages);
                }

                // Existing OpenAI/Groq/Azure logic
                var payload = new
                {
                    model = _model,
                    messages = messages,
                    temperature = 0.1
                };

                string url;
                var request = (HttpWebRequest)null;

                if (string.Equals(_provider, "AzureOpenAI", StringComparison.OrdinalIgnoreCase))
                {
                    url = CombineUrl(_endpoint, $"/openai/deployments/{_deploymentId}/chat/completions?api-version={_apiVersion}");
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Headers["api-key"] = _apiKey;
                }
                else
                {
                    // Support Groq and OpenAI (OpenAI-compatible APIs)
                    if (string.Equals(_provider, "Groq", StringComparison.OrdinalIgnoreCase))
                    {
                        url = !string.IsNullOrEmpty(_endpoint) ? _endpoint : "https://api.groq.com/openai/v1/chat/completions";
                    }
                    else if (string.Equals(_provider, "OpenAI", StringComparison.OrdinalIgnoreCase))
                    {
                        url = !string.IsNullOrEmpty(_endpoint) ? _endpoint : "https://api.openai.com/v1/chat/completions";
                    }
                    else
                    {
                        url = !string.IsNullOrEmpty(_endpoint) ? _endpoint : "https://api.openai.com/v1/chat/completions";
                    }
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Headers["Authorization"] = "Bearer " + _apiKey;
                }

                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                var json = _serializer.Serialize(payload);
                var bytes = Encoding.UTF8.GetBytes(json);
                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var raw = reader.ReadToEnd();
                    var data = _serializer.Deserialize<ChatCompletionResponse>(raw);
                    var content = data?.choices != null && data.choices.Count > 0 ? data.choices[0].message.content : null;
                    return content ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CallLLM Error: " + ex.Message);
                return FallbackAnswer(messages);
            }
        }

        // New method for Gemini API
        private string CallGeminiAPI(List<ChatCompletionMessage> messages)
        {
            try
            {
                // Convert OpenAI format to Gemini format
                var geminiContents = new List<GeminiContent>();
                
                foreach (var msg in messages)
                {
                    // Skip system messages (Gemini doesn't use system role, we'll add it to first user message)
                    if (msg.role == "system")
                    {
                        continue;
                    }
                    
                    // Map roles: OpenAI "assistant" -> Gemini "model", OpenAI "user" -> Gemini "user"
                    string geminiRole = msg.role == "assistant" ? "model" : "user";
                    
                    geminiContents.Add(new GeminiContent
                    {
                        role = geminiRole,
                        parts = new[] { new GeminiPart { text = msg.content } }
                    });
                }

                // Add system prompt to first user message if exists
                var systemMsg = messages.FirstOrDefault(m => m.role == "system");
                if (systemMsg != null && geminiContents.Count > 0 && geminiContents[0].role == "user")
                {
                    geminiContents[0].parts[0].text = systemMsg.content + "\n\n" + geminiContents[0].parts[0].text;
                }

                var payload = new
                {
                    contents = geminiContents.ToArray(),
                    generationConfig = new
                    {
                        temperature = 0.1,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 1024
                    }
                };

                string modelName = string.IsNullOrEmpty(_model) ? "gemini-1.5-flash" : _model;
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={_apiKey}";

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";

                var json = _serializer.Serialize(payload);
                var bytes = Encoding.UTF8.GetBytes(json);
                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var raw = reader.ReadToEnd();
                    var data = _serializer.Deserialize<GeminiResponse>(raw);
                    
                    if (data?.candidates != null && data.candidates.Length > 0 && data.candidates[0].content != null)
                    {
                        var parts = data.candidates[0].content.parts;
                        if (parts != null && parts.Length > 0)
                        {
                            return parts[0].text ?? string.Empty;
                        }
                    }
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Gemini API Error: " + ex.Message);
                return FallbackAnswer(messages);
            }
        }

        private string CombineUrl(string baseUrl, string path)
        {
            if (string.IsNullOrEmpty(baseUrl)) return path ?? string.Empty;
            if (string.IsNullOrEmpty(path)) return baseUrl;
            return baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
        }

        private string FallbackAnswer(List<ChatCompletionMessage> messages)
        {
            var lastUser = messages.LastOrDefault(m => m.role == "user")?.content ?? string.Empty;
            // Try quick intent resolution
            var quickIntent = TryResolveIntentFromUserMessage(lastUser, false, null);
            if (!string.IsNullOrEmpty(quickIntent))
            {
                try { 
                    // Create a mock context for fallback
                    var mockContext = HttpContext.Current;
                    return ExecuteIntent(quickIntent, mockContext); 
                } catch { }
            }

            if (lastUser.IndexOf("commande", StringComparison.OrdinalIgnoreCase) >= 0 ||
                Regex.IsMatch(lastUser, @"\bORD-?\d+\b", RegexOptions.IgnoreCase))
            {
                return "Pour suivre une commande, indiquez votre numéro de commande (ex: ORD-12345).";
            }
            if (lastUser.IndexOf("produit", StringComparison.OrdinalIgnoreCase) >= 0 ||
                lastUser.IndexOf("huile", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Je peux rechercher des produits. Donnez-moi un nom (ex: huile d'argan).";
            }
            return "Bonjour, je suis votre assistant. Comment puis-je vous aider ?";
        }

        private string TryResolveIntentFromUserMessage(string userMessage, bool isAuthenticated, int? userId)
        {
            if (string.IsNullOrWhiteSpace(userMessage)) return null;
            var text = userMessage.Trim();

            // Order status via order number ORD-xxxxx
            var mOrder = Regex.Match(text, @"\b(ORD-?\d+)\b", RegexOptions.IgnoreCase);
            if (mOrder.Success)
            {
                var num = mOrder.Groups[1].Value;
                return _serializer.Serialize(new { intent = "get_order_status", parameters = new { orderNumber = num } });
            }

            // Product details by ID
            var mId = Regex.Match(text, @"\b(?:id\s*[:#]?|produit\s*#?)\s*(\d{1,9})\b", RegexOptions.IgnoreCase);
            if (mId.Success)
            {
                var id = mId.Groups[1].Value;
                return _serializer.Serialize(new { intent = "get_product_details", parameters = new { id = id } });
            }

            // My orders (if authenticated) - many variations
            if (isAuthenticated && (
                text.IndexOf("mes commandes", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("mes commande", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("mon historique", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("mes ordres", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("mes achats", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("historique", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("commandes", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                // Check if asking for pending/recent
                if (text.IndexOf("en attente", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    text.IndexOf("pending", StringComparison.OrdinalIgnoreCase) >= 0)
                    return _serializer.Serialize(new { intent = "get_pending_orders", parameters = new { } });
                if (text.IndexOf("récent", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    text.IndexOf("recent", StringComparison.OrdinalIgnoreCase) >= 0)
                    return _serializer.Serialize(new { intent = "get_recent_orders", parameters = new { limit = 5 } });
                return _serializer.Serialize(new { intent = "get_my_orders", parameters = new { limit = 5 } });
            }
            
            // Order details/tracking
            if (text.IndexOf("détails", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("details", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("suivi", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("tracking", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("où est", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("ou est", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var orderMatch = Regex.Match(text, @"\b(ORD-?\d+)\b", RegexOptions.IgnoreCase);
                if (orderMatch.Success)
                {
                    return _serializer.Serialize(new { intent = "get_order_details", parameters = new { orderNumber = orderMatch.Groups[1].Value } });
                }
            }
            
            // Cart (authenticated)
            if (isAuthenticated && (
                text.IndexOf("panier", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("cart", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                if (text.IndexOf("nombre", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    text.IndexOf("combien", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    text.IndexOf("count", StringComparison.OrdinalIgnoreCase) >= 0)
                    return _serializer.Serialize(new { intent = "get_cart_count", parameters = new { } });
                return _serializer.Serialize(new { intent = "get_cart", parameters = new { } });
            }
            
            // Wishlist (authenticated)
            if (isAuthenticated && (
                text.IndexOf("wishlist", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("liste de souhaits", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("favoris", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return _serializer.Serialize(new { intent = "get_wishlist", parameters = new { limit = 10 } });
            }
            
            // Categories
            if (text.IndexOf("catégorie", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("categorie", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("catégories", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // Extract category name if mentioned
                var catMatch = Regex.Match(text, @"(?:catégorie|categorie)\s+([a-zéèêëàâäôöùûüç\s]+)", RegexOptions.IgnoreCase);
                if (catMatch.Success)
                {
                    var catName = catMatch.Groups[1].Value.Trim();
                    return _serializer.Serialize(new { intent = "get_category_products", parameters = new { categoryName = catName, limit = 10 } });
                }
                return _serializer.Serialize(new { intent = "get_categories", parameters = new { } });
            }
            
            // Notifications (authenticated)
            if (isAuthenticated && (
                text.IndexOf("notification", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("alerte", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                if (text.IndexOf("non lue", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    text.IndexOf("unread", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    text.IndexOf("nombre", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    text.IndexOf("combien", StringComparison.OrdinalIgnoreCase) >= 0)
                    return _serializer.Serialize(new { intent = "get_unread_notifications_count", parameters = new { } });
                return _serializer.Serialize(new { intent = "get_notifications", parameters = new { limit = 10 } });
            }
            
            // Account info (authenticated)
            if (isAuthenticated && (
                text.IndexOf("mon compte", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("mes informations", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("mon profil", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return _serializer.Serialize(new { intent = "get_account_info", parameters = new { } });
            }
            
            // Featured products
            if (text.IndexOf("vedette", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("featured", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("recommandé", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return _serializer.Serialize(new { intent = "featured_products", parameters = new { limit = 5 } });
            }
            
            // Reviews
            if (text.IndexOf("avis", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("review", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("commentaire", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var prodIdMatch = Regex.Match(text, @"(?:produit|id)\s*[:#]?\s*(\d+)", RegexOptions.IgnoreCase);
                if (prodIdMatch.Success)
                {
                    return _serializer.Serialize(new { intent = "get_product_reviews", parameters = new { productId = prodIdMatch.Groups[1].Value, limit = 5 } });
                }
            }

            // User orders by email (for guests)
            var mEmail = Regex.Match(text, @"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}", RegexOptions.IgnoreCase);
            if (mEmail.Success && text.IndexOf("commande", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var email = mEmail.Value;
                return _serializer.Serialize(new { intent = "get_user_orders", parameters = new { email = email, limit = 5 } });
            }

            // Product search
            if (Regex.IsMatch(text, @"\b(huile|produit|chercher|recherche|bio|argan|coco|olive)\b", RegexOptions.IgnoreCase))
            {
                var tokens = Regex.Matches(text, @"[\p{L}\p{N}]{3,}", RegexOptions.IgnoreCase)
                                   .Cast<Match>()
                                   .Select(x => x.Value)
                                   .Where(w => w.Length >= 3)
                                   .Take(3)
                                   .ToArray();
                var name = string.Join(" ", tokens);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return _serializer.Serialize(new { intent = "search_products", parameters = new { name = name, limit = 5 } });
                }
            }

            // Top products
            if (text.IndexOf("top", StringComparison.OrdinalIgnoreCase) >= 0 ||
                text.IndexOf("populaire", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return _serializer.Serialize(new { intent = "top_products", parameters = new { limit = 5 } });
            }

            return null;
        }

        private string ExtractIntentJson(string assistant)
        {
            // try ```json ... ```
            var codeBlock = Regex.Match(assistant, @"```json\s*(\{[\s\S]*?\})\s*```", RegexOptions.IgnoreCase);
            if (codeBlock.Success) return codeBlock.Groups[1].Value;

            // try first JSON object containing "intent"
            var intentObj = Regex.Match(assistant, @"(\{[\s\S]*?\}\s*)", RegexOptions.Singleline);
            if (intentObj.Success && intentObj.Value.IndexOf("\"intent\"", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return intentObj.Value.Trim();
            }
            return null;
        }

        private string ExecuteIntent(string intentJson, HttpContext context)
        {
            try
            {
                var dict = _serializer.Deserialize<Dictionary<string, object>>(intentJson);
                var intent = (dict.ContainsKey("intent") ? Convert.ToString(dict["intent"]) : string.Empty) ?? string.Empty;
                var parameters = new Dictionary<string, object>();
                if (dict.ContainsKey("parameters") && dict["parameters"] != null)
                {
                    parameters = (Dictionary<string, object>)dict["parameters"];
                }

                // Get user info
                bool isAuthenticated = context.Session["UserId"] != null;
                int? userId = isAuthenticated ? (int?)context.Session["UserId"] : null;
                string userEmail = context.Session["UserEmail"]?.ToString();

                switch (intent)
                {
                    // Products
                    case "search_products":
                        return HandleSearchProducts(parameters);
                    case "get_product_details":
                        return HandleGetProductDetails(parameters);
                    case "get_products_by_category":
                        return HandleGetProductsByCategory(parameters);
                    case "top_products":
                        return HandleTopProducts(parameters);
                    case "featured_products":
                        return HandleFeaturedProducts(parameters);
                    case "low_stock_products":
                        return HandleLowStockProducts(parameters);
                    case "get_product_reviews":
                        return HandleGetProductReviews(parameters);
                    
                    // Categories
                    case "get_categories":
                        return HandleGetCategories(parameters);
                    case "get_category_products":
                        return HandleGetCategoryProducts(parameters);
                    
                    // Orders (Authenticated)
                    case "get_my_orders":
                        if (!isAuthenticated)
                            return "Vous devez être connecté pour voir vos commandes. Veuillez vous connecter.";
                        return HandleGetMyOrders(userId.Value, parameters);
                    case "get_pending_orders":
                        if (!isAuthenticated)
                            return "Vous devez être connecté pour voir vos commandes en attente.";
                        return HandleGetPendingOrders(userId.Value, parameters);
                    case "get_recent_orders":
                        if (!isAuthenticated)
                            return "Vous devez être connecté pour voir vos commandes récentes.";
                        return HandleGetRecentOrders(userId.Value, parameters);
                    case "get_order_status":
                        return HandleGetOrderStatus(parameters, context);
                    case "get_order_details":
                        return HandleGetOrderDetails(parameters, context);
                    case "get_user_orders": // For guests
                        if (isAuthenticated && !parameters.ContainsKey("email"))
                            return HandleGetMyOrders(userId.Value, parameters);
                        return HandleGetUserOrders(parameters);
                    
                    // Cart & Wishlist (Authenticated only)
                    case "get_cart":
                        if (!isAuthenticated)
                            return "Vous devez être connecté pour voir votre panier.";
                        return HandleGetCart(userId.Value, parameters);
                    case "get_cart_count":
                        if (!isAuthenticated)
                            return "0";
                        return HandleGetCartCount(userId.Value);
                    case "get_wishlist":
                        if (!isAuthenticated)
                            return "Vous devez être connecté pour voir votre liste de souhaits.";
                        return HandleGetWishlist(userId.Value, parameters);
                    
                    // Account & Notifications (Authenticated only)
                    case "get_account_info":
                        if (!isAuthenticated)
                            return "Vous devez être connecté pour voir vos informations de compte.";
                        return HandleGetAccountInfo(userId.Value);
                    case "get_notifications":
                        if (!isAuthenticated)
                            return "Vous devez être connecté pour voir vos notifications.";
                        return HandleGetNotifications(userId.Value, parameters);
                    case "get_unread_notifications_count":
                        if (!isAuthenticated)
                            return "0";
                        return HandleGetUnreadNotificationsCount(userId.Value);
                    
                    default:
                        return "Je n'ai pas compris l'action demandée. Pouvez-vous préciser votre question ?";
                }
            }
            catch
            {
                return "Je n'ai pas pu interpréter la demande. Réessayez en précisant votre question.";
            }
        }

        private string HandleSearchProducts(Dictionary<string, object> parameters)
        {
            string name = Param(parameters, "name");
            int limit = ParamInt(parameters, "limit", 10);
            var categoryIdStr = Param(parameters, "categoryId");
            decimal minPrice = ParamDecimal(parameters, "minPrice", 0);
            decimal maxPrice = ParamDecimal(parameters, "maxPrice", 0);
            
            var db = new DbContext();
            var sqlParams = new List<SqlParameter>();
            var sql = new StringBuilder(@"SELECT TOP(@limit) p.Id, p.Name, p.Price, p.StockQuantity, c.Name as CategoryName
                                         FROM Products p
                                         LEFT JOIN Categories c ON p.CategoryId = c.Id
                                         WHERE p.IsActive = 1");
            
            sqlParams.Add(new SqlParameter("@limit", limit));
            
            if (!string.IsNullOrWhiteSpace(name))
            {
                sql.Append(" AND p.Name LIKE @name");
                sqlParams.Add(new SqlParameter("@name", "%" + name + "%"));
            }
            
            if (!string.IsNullOrWhiteSpace(categoryIdStr) && int.TryParse(categoryIdStr, out var categoryId))
            {
                sql.Append(" AND p.CategoryId = @categoryId");
                sqlParams.Add(new SqlParameter("@categoryId", categoryId));
            }
            
            if (minPrice > 0)
            {
                sql.Append(" AND p.Price >= @minPrice");
                sqlParams.Add(new SqlParameter("@minPrice", minPrice));
            }
            
            if (maxPrice > 0)
            {
                sql.Append(" AND p.Price <= @maxPrice");
                sqlParams.Add(new SqlParameter("@maxPrice", maxPrice));
            }
            
            sql.Append(" ORDER BY p.ViewCount DESC");
            
            var dt = db.ExecuteQuery(sql.ToString(), sqlParams.ToArray());

            if (dt.Rows.Count == 0) 
            {
                if (!string.IsNullOrWhiteSpace(name))
                    return $"Aucun produit trouvé pour '{name}'.";
                return "Aucun produit trouvé avec ces critères.";
            }
            
            var lines = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                var catName = row["CategoryName"] != DBNull.Value ? $" [{row["CategoryName"]}]" : "";
                lines.Add($"#{row["Id"]} — {row["Name"]}{catName} — {Convert.ToDecimal(row["Price"]):0.00} MAD — Stock: {row["StockQuantity"]}");
            }
            return $"Produits trouvés ({dt.Rows.Count}):\n" + string.Join("\n", lines);
        }

        private string HandleGetProductDetails(Dictionary<string, object> parameters)
        {
            var idStr = Param(parameters, "id");
            var name = Param(parameters, "name");
            var db = new DbContext();
            DataTable dt;
            if (!string.IsNullOrWhiteSpace(idStr) && int.TryParse(idStr, out var id))
            {
                var sql = @"SELECT TOP(1) Id, Name, Description, Price, StockQuantity FROM Products WHERE Id=@id";
                dt = db.ExecuteQuery(sql, new[] { new SqlParameter("@id", id) });
            }
            else if (!string.IsNullOrWhiteSpace(name))
            {
                var sql = @"SELECT TOP(1) Id, Name, Description, Price, StockQuantity FROM Products WHERE IsActive=1 AND Name LIKE @name ORDER BY ViewCount DESC";
                dt = db.ExecuteQuery(sql, new[] { new SqlParameter("@name", "%" + name + "%") });
            }
            else
            {
                return "Veuillez fournir l'identifiant ou le nom du produit.";
            }

            if (dt.Rows.Count == 0) return "Produit introuvable.";
            var r = dt.Rows[0];
            return $"Détails produit:\nId: {r["Id"]}\nNom: {r["Name"]}\nPrix: {Convert.ToDecimal(r["Price"]):0.00} MAD\nStock: {r["StockQuantity"]}\nDescription: {r["Description"]}";
        }

        private string HandleGetOrderStatus(Dictionary<string, object> parameters, HttpContext context)
        {
            string orderNumber = Param(parameters, "orderNumber");
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                return "Veuillez fournir votre numéro de commande (ex: ORD-12345).";
            }

            var sql = @"SELECT TOP(1) OrderNumber, Status, TotalAmount, OrderDate, UserId 
                        FROM Orders WHERE OrderNumber=@num";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[] { new SqlParameter("@num", orderNumber) });
            
            if (dt.Rows.Count == 0) 
                return "Commande introuvable. Vérifiez le numéro.";
            
            // Security check: If user is authenticated, only show THEIR orders
            if (context.Session["UserId"] != null)
            {
                int currentUserId = (int)context.Session["UserId"];
                int orderUserId = Convert.ToInt32(dt.Rows[0]["UserId"]);
                if (orderUserId != currentUserId)
                {
                    return "Vous n'avez pas accès à cette commande. Cette commande ne vous appartient pas.";
                }
            }
            
            var r = dt.Rows[0];
            return $"Statut de la commande {r["OrderNumber"]}: {r["Status"]}. Montant: {Convert.ToDecimal(r["TotalAmount"]):0.00} MAD. Date: {Convert.ToDateTime(r["OrderDate"]).ToString("dd/MM/yyyy HH:mm")}.";
        }

        private string HandleGetUserOrders(Dictionary<string, object> parameters)
        {
            string email = Param(parameters, "email");
            int limit = ParamInt(parameters, "limit", 5);
            if (string.IsNullOrWhiteSpace(email))
            {
                return "Veuillez indiquer votre e-mail pour retrouver vos commandes.";
            }

            var sql = @"SELECT TOP(@limit) o.OrderNumber, o.Status, o.TotalAmount, o.OrderDate
                        FROM Orders o INNER JOIN Users u ON o.UserId = u.Id
                        WHERE u.Email = @mail
                        ORDER BY o.OrderDate DESC";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[]
            {
                new SqlParameter("@limit", limit),
                new SqlParameter("@mail", email)
            });
            if (dt.Rows.Count == 0) return "Aucune commande trouvée pour cet e-mail.";

            var lines = new List<string>();
            foreach (DataRow r in dt.Rows)
            {
                lines.Add($"{Convert.ToDateTime(r["OrderDate"]).ToString("dd/MM/yyyy")}: {r["OrderNumber"]} — {r["Status"]} — {Convert.ToDecimal(r["TotalAmount"]):0.00} MAD");
            }
            return "Vos dernières commandes:\n" + string.Join("\n", lines);
        }

        private string HandleGetMyOrders(int userId, Dictionary<string, object> parameters)
        {
            int limit = ParamInt(parameters, "limit", 5);
            
            var sql = @"SELECT TOP(@limit) o.OrderNumber, o.Status, o.TotalAmount, o.OrderDate
                        FROM Orders o
                        WHERE o.UserId = @userId
                        ORDER BY o.OrderDate DESC";
            
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[]
            {
                new SqlParameter("@limit", limit),
                new SqlParameter("@userId", userId)
            });
            
            if (dt.Rows.Count == 0) 
                return "Vous n'avez aucune commande pour le moment.";
            
            var lines = new List<string>();
            foreach (DataRow r in dt.Rows)
            {
                lines.Add($"{Convert.ToDateTime(r["OrderDate"]).ToString("dd/MM/yyyy")}: {r["OrderNumber"]} — {r["Status"]} — {Convert.ToDecimal(r["TotalAmount"]):0.00} MAD");
            }
            return "Vos dernières commandes:\n" + string.Join("\n", lines);
        }

        private string HandleTopProducts(Dictionary<string, object> parameters)
        {
            int limit = ParamInt(parameters, "limit", 5);
            var sql = @"SELECT TOP(@limit) Id, Name, Price, StockQuantity FROM Products WHERE IsActive=1 ORDER BY ViewCount DESC";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[] { new SqlParameter("@limit", limit) });
            if (dt.Rows.Count == 0) return "Aucun produit à afficher pour le moment.";
            var lines = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                lines.Add($"#{row["Id"]} — {row["Name"]} — {Convert.ToDecimal(row["Price"]):0.00} MAD");
            }
            return "Produits populaires:\n" + string.Join("\n", lines);
        }

        private string Param(Dictionary<string, object> p, string key)
        {
            if (p == null || !p.ContainsKey(key) || p[key] == null) return null;
            return Convert.ToString(p[key]);
        }

        private int ParamInt(Dictionary<string, object> p, string key, int def)
        {
            var v = Param(p, key);
            return int.TryParse(v, out var n) ? n : def;
        }

        private decimal ParamDecimal(Dictionary<string, object> p, string key, decimal def)
        {
            var v = Param(p, key);
            return decimal.TryParse(v, out var n) ? n : def;
        }

        private bool ParamBool(Dictionary<string, object> p, string key, bool def)
        {
            var v = Param(p, key);
            if (string.IsNullOrEmpty(v)) return def;
            return bool.TryParse(v, out var b) ? b : def;
        }

        // ========== NEW INTENT HANDLERS ==========

        private string HandleGetCategories(Dictionary<string, object> parameters)
        {
            var sql = @"SELECT Id, Name, Description FROM Categories WHERE IsActive = 1 ORDER BY DisplayOrder ASC, Name ASC";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql);
            
            if (dt.Rows.Count == 0) return "Aucune catégorie disponible pour le moment.";
            
            var lines = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                var desc = row["Description"] != DBNull.Value ? row["Description"].ToString() : "";
                lines.Add($"• {row["Name"]}" + (!string.IsNullOrEmpty(desc) ? $" - {desc}" : ""));
            }
            return "Catégories disponibles:\n" + string.Join("\n", lines);
        }

        private string HandleGetCategoryProducts(Dictionary<string, object> parameters)
        {
            var categoryIdStr = Param(parameters, "categoryId");
            var categoryName = Param(parameters, "categoryName");
            int limit = ParamInt(parameters, "limit", 10);
            
            var db = new DbContext();
            DataTable dt;
            
            if (!string.IsNullOrWhiteSpace(categoryIdStr) && int.TryParse(categoryIdStr, out var categoryId))
            {
                var sql = @"SELECT TOP(@limit) p.Id, p.Name, p.Price, p.StockQuantity, c.Name as CategoryName
                           FROM Products p
                           INNER JOIN Categories c ON p.CategoryId = c.Id
                           WHERE p.IsActive = 1 AND p.CategoryId = @catId
                           ORDER BY p.ViewCount DESC";
                dt = db.ExecuteQuery(sql, new[] { 
                    new SqlParameter("@limit", limit),
                    new SqlParameter("@catId", categoryId)
                });
            }
            else if (!string.IsNullOrWhiteSpace(categoryName))
            {
                var sql = @"SELECT TOP(@limit) p.Id, p.Name, p.Price, p.StockQuantity, c.Name as CategoryName
                           FROM Products p
                           INNER JOIN Categories c ON p.CategoryId = c.Id
                           WHERE p.IsActive = 1 AND c.Name LIKE @catName
                           ORDER BY p.ViewCount DESC";
                dt = db.ExecuteQuery(sql, new[] { 
                    new SqlParameter("@limit", limit),
                    new SqlParameter("@catName", "%" + categoryName + "%")
                });
            }
            else
            {
                return "Veuillez spécifier une catégorie (ID ou nom).";
            }
            
            if (dt.Rows.Count == 0) return "Aucun produit trouvé dans cette catégorie.";
            
            var lines = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                lines.Add($"#{row["Id"]} — {row["Name"]} — {Convert.ToDecimal(row["Price"]):0.00} MAD — Stock: {row["StockQuantity"]}");
            }
            return $"Produits de la catégorie '{dt.Rows[0]["CategoryName"]}':\n" + string.Join("\n", lines);
        }

        private string HandleGetProductsByCategory(Dictionary<string, object> parameters)
        {
            return HandleGetCategoryProducts(parameters);
        }

        private string HandleFeaturedProducts(Dictionary<string, object> parameters)
        {
            int limit = ParamInt(parameters, "limit", 5);
            var sql = @"SELECT TOP(@limit) Id, Name, Price, StockQuantity, Description
                       FROM Products 
                       WHERE IsActive = 1 AND IsFeatured = 1
                       ORDER BY ViewCount DESC";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[] { new SqlParameter("@limit", limit) });
            
            if (dt.Rows.Count == 0) return "Aucun produit en vedette pour le moment.";
            
            var lines = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                var desc = row["Description"] != DBNull.Value && !string.IsNullOrEmpty(row["Description"].ToString()) 
                    ? row["Description"].ToString().Substring(0, Math.Min(50, row["Description"].ToString().Length)) + "..." 
                    : "";
                lines.Add($"#{row["Id"]} — {row["Name"]} — {Convert.ToDecimal(row["Price"]):0.00} MAD" + 
                         (!string.IsNullOrEmpty(desc) ? $"\n  {desc}" : ""));
            }
            return "Produits en vedette:\n" + string.Join("\n\n", lines);
        }

        private string HandleLowStockProducts(Dictionary<string, object> parameters)
        {
            int limit = ParamInt(parameters, "limit", 5);
            var sql = @"SELECT TOP(@limit) Id, Name, Price, StockQuantity
                       FROM Products 
                       WHERE IsActive = 1 AND StockQuantity > 0 AND StockQuantity < 10
                       ORDER BY StockQuantity ASC";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[] { new SqlParameter("@limit", limit) });
            
            if (dt.Rows.Count == 0) return "Tous les produits sont bien en stock !";
            
            var lines = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                lines.Add($"#{row["Id"]} — {row["Name"]} — {Convert.ToDecimal(row["Price"]):0.00} MAD — Stock: {row["StockQuantity"]} (limité!)");
            }
            return "Produits en stock limité (moins de 10 unités):\n" + string.Join("\n", lines);
        }

        private string HandleGetProductReviews(Dictionary<string, object> parameters)
        {
            var productIdStr = Param(parameters, "productId");
            int limit = ParamInt(parameters, "limit", 5);
            
            if (string.IsNullOrWhiteSpace(productIdStr) || !int.TryParse(productIdStr, out var productId))
            {
                return "Veuillez spécifier l'ID du produit.";
            }
            
            var sql = @"SELECT TOP(@limit) r.Rating, r.Comment, r.ReviewDate, u.FullName, r.IsVerifiedPurchase
                       FROM Reviews r
                       INNER JOIN Users u ON r.UserId = u.Id
                       WHERE r.ProductId = @prodId AND r.IsApproved = 1
                       ORDER BY r.ReviewDate DESC";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[] { 
                new SqlParameter("@limit", limit),
                new SqlParameter("@prodId", productId)
            });
            
            if (dt.Rows.Count == 0) return "Aucun avis disponible pour ce produit.";
            
            var lines = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                var rating = Convert.ToInt32(row["Rating"]);
                var stars = new string('⭐', rating) + new string('☆', 5 - rating);
                var comment = row["Comment"] != DBNull.Value ? row["Comment"].ToString() : "";
                var verified = Convert.ToBoolean(row["IsVerifiedPurchase"]) ? " ✓ Achat vérifié" : "";
                var date = Convert.ToDateTime(row["ReviewDate"]).ToString("dd/MM/yyyy");
                var name = row["FullName"].ToString();
                
                lines.Add($"{stars} par {name} ({date}){verified}");
                if (!string.IsNullOrEmpty(comment))
                {
                    lines.Add($"  \"{comment}\"");
                }
            }
            return "Avis clients:\n" + string.Join("\n\n", lines);
        }

        private string HandleGetOrderDetails(Dictionary<string, object> parameters, HttpContext context)
        {
            string orderNumber = Param(parameters, "orderNumber");
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                return "Veuillez fournir le numéro de commande (ex: ORD-12345).";
            }
            
            var db = new DbContext();
            
            // Get order info
            var orderSql = @"SELECT o.Id, o.OrderNumber, o.Status, o.TotalAmount, o.OrderDate, o.TrackingNumber, 
                           o.ShippingMethod, o.Notes, o.UserId, o.SubTotal, o.ShippingCost, o.TaxAmount,
                           a.FullName, a.Street, a.City, a.ZipCode, a.Country, a.Phone
                           FROM Orders o
                           LEFT JOIN Addresses a ON o.ShippingAddressId = a.Id
                           WHERE o.OrderNumber = @num";
            var orderDt = db.ExecuteQuery(orderSql, new[] { new SqlParameter("@num", orderNumber) });
            
            if (orderDt.Rows.Count == 0) 
                return "Commande introuvable. Vérifiez le numéro.";
            
            var order = orderDt.Rows[0];
            
            // Security check
            if (context.Session["UserId"] != null)
            {
                int currentUserId = (int)context.Session["UserId"];
                int orderUserId = Convert.ToInt32(order["UserId"]);
                if (orderUserId != currentUserId)
                {
                    return "Vous n'avez pas accès à cette commande.";
                }
            }
            
            // Get order items
            var itemsSql = @"SELECT ProductName, Quantity, UnitPrice, TotalPrice
                            FROM OrderItems
                            WHERE OrderId = @orderId";
            var itemsDt = db.ExecuteQuery(itemsSql, new[] { new SqlParameter("@orderId", order["Id"]) });
            
            var result = new StringBuilder();
            result.AppendLine($"📦 Commande {order["OrderNumber"]}");
            result.AppendLine($"Statut: {order["Status"]}");
            result.AppendLine($"Date: {Convert.ToDateTime(order["OrderDate"]).ToString("dd/MM/yyyy HH:mm")}");
            result.AppendLine("");
            result.AppendLine("Articles:");
            foreach (DataRow item in itemsDt.Rows)
            {
                result.AppendLine($"  • {item["ProductName"]} x{item["Quantity"]} = {Convert.ToDecimal(item["TotalPrice"]):0.00} MAD");
            }
            result.AppendLine("");
            result.AppendLine($"Sous-total: {Convert.ToDecimal(order["SubTotal"]):0.00} MAD");
            if (Convert.ToDecimal(order["ShippingCost"]) > 0)
                result.AppendLine($"Livraison: {Convert.ToDecimal(order["ShippingCost"]):0.00} MAD");
            if (Convert.ToDecimal(order["TaxAmount"]) > 0)
                result.AppendLine($"Taxes: {Convert.ToDecimal(order["TaxAmount"]):0.00} MAD");
            result.AppendLine($"TOTAL: {Convert.ToDecimal(order["TotalAmount"]):0.00} MAD");
            
            if (order["TrackingNumber"] != DBNull.Value && !string.IsNullOrEmpty(order["TrackingNumber"].ToString()))
            {
                result.AppendLine($"");
                result.AppendLine($"📮 Numéro de suivi: {order["TrackingNumber"]}");
            }
            
            if (order["ShippingMethod"] != DBNull.Value && !string.IsNullOrEmpty(order["ShippingMethod"].ToString()))
            {
                result.AppendLine($"Méthode de livraison: {order["ShippingMethod"]}");
            }
            
            if (order["Street"] != DBNull.Value)
            {
                result.AppendLine("");
                result.AppendLine("Adresse de livraison:");
                result.AppendLine($"  {order["FullName"]}");
                result.AppendLine($"  {order["Street"]}");
                result.AppendLine($"  {order["City"]} {order["ZipCode"]}");
                result.AppendLine($"  {order["Country"]}");
                if (order["Phone"] != DBNull.Value)
                    result.AppendLine($"  Tél: {order["Phone"]}");
            }
            
            return result.ToString();
        }

        private string HandleGetPendingOrders(int userId, Dictionary<string, object> parameters)
        {
            var sql = @"SELECT TOP(10) OrderNumber, Status, TotalAmount, OrderDate
                       FROM Orders
                       WHERE UserId = @userId AND Status IN ('Pending', 'Processing', 'Shipped')
                       ORDER BY OrderDate DESC";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[] { new SqlParameter("@userId", userId) });
            
            if (dt.Rows.Count == 0) 
                return "Vous n'avez aucune commande en attente.";
            
            var lines = new List<string>();
            foreach (DataRow r in dt.Rows)
            {
                lines.Add($"{Convert.ToDateTime(r["OrderDate"]).ToString("dd/MM/yyyy")}: {r["OrderNumber"]} — {r["Status"]} — {Convert.ToDecimal(r["TotalAmount"]):0.00} MAD");
            }
            return "Vos commandes en attente:\n" + string.Join("\n", lines);
        }

        private string HandleGetRecentOrders(int userId, Dictionary<string, object> parameters)
        {
            int limit = ParamInt(parameters, "limit", 5);
            return HandleGetMyOrders(userId, new Dictionary<string, object> { { "limit", limit } });
        }

        private string HandleGetCart(int userId, Dictionary<string, object> parameters)
        {
            var sql = @"SELECT c.Quantity, p.Id, p.Name, p.Price, p.StockQuantity
                       FROM ShoppingCart c
                       INNER JOIN Products p ON c.ProductId = p.Id
                       WHERE c.UserId = @userId AND p.IsActive = 1
                       ORDER BY c.CreatedAt DESC";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[] { new SqlParameter("@userId", userId) });
            
            if (dt.Rows.Count == 0) 
                return "Votre panier est vide.";
            
            var lines = new List<string>();
            decimal total = 0;
            foreach (DataRow row in dt.Rows)
            {
                int qty = Convert.ToInt32(row["Quantity"]);
                decimal price = Convert.ToDecimal(row["Price"]);
                decimal itemTotal = qty * price;
                total += itemTotal;
                lines.Add($"• {row["Name"]} x{qty} = {itemTotal:0.00} MAD");
            }
            lines.Add($"");
            lines.Add($"TOTAL: {total:0.00} MAD");
            return "Contenu de votre panier:\n" + string.Join("\n", lines);
        }

        private string HandleGetCartCount(int userId)
        {
            var sql = @"SELECT ISNULL(SUM(Quantity), 0) as TotalItems
                       FROM ShoppingCart
                       WHERE UserId = @userId";
            var db = new DbContext();
            var result = db.ExecuteScalar(sql, new[] { new SqlParameter("@userId", userId) });
            int count = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            return count.ToString();
        }

        private string HandleGetWishlist(int userId, Dictionary<string, object> parameters)
        {
            int limit = ParamInt(parameters, "limit", 10);
            var sql = @"SELECT TOP(@limit) p.Id, p.Name, p.Price, p.StockQuantity
                       FROM Wishlist w
                       INNER JOIN Products p ON w.ProductId = p.Id
                       WHERE w.UserId = @userId AND p.IsActive = 1
                       ORDER BY w.CreatedAt DESC";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[] { 
                new SqlParameter("@limit", limit),
                new SqlParameter("@userId", userId)
            });
            
            if (dt.Rows.Count == 0) 
                return "Votre liste de souhaits est vide.";
            
            var lines = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                lines.Add($"#{row["Id"]} — {row["Name"]} — {Convert.ToDecimal(row["Price"]):0.00} MAD — Stock: {row["StockQuantity"]}");
            }
            return "Votre liste de souhaits:\n" + string.Join("\n", lines);
        }

        private string HandleGetAccountInfo(int userId)
        {
            var sql = @"SELECT Email, FullName, Phone, Role, CreatedAt
                       FROM Users
                       WHERE Id = @userId";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[] { new SqlParameter("@userId", userId) });
            
            if (dt.Rows.Count == 0) return "Informations de compte introuvables.";
            
            var row = dt.Rows[0];
            var result = new StringBuilder();
            result.AppendLine("📋 Vos informations de compte:");
            result.AppendLine($"Nom: {row["FullName"]}");
            result.AppendLine($"Email: {row["Email"]}");
            if (row["Phone"] != DBNull.Value && !string.IsNullOrEmpty(row["Phone"].ToString()))
                result.AppendLine($"Téléphone: {row["Phone"]}");
            result.AppendLine($"Type de compte: {row["Role"]}");
            result.AppendLine($"Membre depuis: {Convert.ToDateTime(row["CreatedAt"]).ToString("dd/MM/yyyy")}");
            return result.ToString();
        }

        private string HandleGetNotifications(int userId, Dictionary<string, object> parameters)
        {
            int limit = ParamInt(parameters, "limit", 10);
            bool unreadOnly = ParamBool(parameters, "unreadOnly", false);
            
            var sql = @"SELECT TOP(@limit) Title, Message, Type, IsRead, CreatedAt
                       FROM Notifications
                       WHERE UserId = @userId" + (unreadOnly ? " AND IsRead = 0" : "") + @"
                       ORDER BY CreatedAt DESC";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[] { 
                new SqlParameter("@limit", limit),
                new SqlParameter("@userId", userId)
            });
            
            if (dt.Rows.Count == 0) 
                return unreadOnly ? "Aucune notification non lue." : "Aucune notification.";
            
            var lines = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                var readStatus = Convert.ToBoolean(row["IsRead"]) ? "✓" : "●";
                var date = Convert.ToDateTime(row["CreatedAt"]).ToString("dd/MM/yyyy HH:mm");
                lines.Add($"{readStatus} [{row["Type"]}] {row["Title"]} ({date})");
                if (row["Message"] != DBNull.Value && !string.IsNullOrEmpty(row["Message"].ToString()))
                    lines.Add($"  {row["Message"]}");
            }
            return "Vos notifications:\n" + string.Join("\n\n", lines);
        }

        private string HandleGetUnreadNotificationsCount(int userId)
        {
            var sql = @"SELECT COUNT(*) as UnreadCount
                       FROM Notifications
                       WHERE UserId = @userId AND IsRead = 0";
            var db = new DbContext();
            var result = db.ExecuteScalar(sql, new[] { new SqlParameter("@userId", userId) });
            int count = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            return count.ToString();
        }
    }
}
