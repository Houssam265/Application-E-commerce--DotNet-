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

    public class ChatResponse
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
            _provider = ConfigurationManager.AppSettings["OpenAI:Provider"] ?? "OpenAI"; // or AzureOpenAI
            _apiKey = ConfigurationManager.AppSettings["OpenAI:ApiKey"] ?? string.Empty;
            _model = ConfigurationManager.AppSettings["OpenAI:Model"] ?? "gpt-3.5-turbo";
            _endpoint = ConfigurationManager.AppSettings["OpenAI:Endpoint"] ?? string.Empty;
            _deploymentId = ConfigurationManager.AppSettings["OpenAI:DeploymentId"] ?? string.Empty;
            _apiVersion = ConfigurationManager.AppSettings["OpenAI:ApiVersion"] ?? "2023-05-15";
        }

        public ChatResponse ProcessMessage(string userMessage, HttpContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userMessage))
                {
                    return new ChatResponse { Reply = "Veuillez saisir un message.", Suggestions = new[] { "Lister les produits", "Suivre une commande", "Aide" } };
                }

                var history = GetHistory(context);
                history.Add(new ChatMessage { Role = "user", Content = userMessage });

                var systemPrompt = BuildSystemPrompt();
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
                        var reply = ExecuteIntent(intentJson);
                        AppendAssistant(history, reply);
                        SaveHistory(context, history);
                        return new ChatResponse { Reply = reply, Suggestions = DefaultSuggestions() };
                    }

                    // Fallback: echo assistant text
                    AppendAssistant(history, assistant);
                    SaveHistory(context, history);
                    return new ChatResponse { Reply = assistant, Suggestions = DefaultSuggestions() };
                }

                AppendAssistant(history, "Je n'ai pas pu générer de réponse.");
                SaveHistory(context, history);
                return new ChatResponse { Reply = "Je n'ai pas pu générer de réponse.", Suggestions = DefaultSuggestions() };
            }
            catch (Exception ex)
            {
                return new ChatResponse { Reply = "Désolé, une erreur s'est produite.", Error = ex.Message, Suggestions = DefaultSuggestions() };
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
                "Suivre ma commande",
                "Voir les produits populaires",
                "Aide"
            };
        }

        private string BuildSystemPrompt()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Tu es un assistant e-commerce pour une boutique en ligne. Réponds toujours en français, de manière polie et concise.");
            sb.AppendLine("Quand la question nécessite des données de la base SQL, produis UNIQUEMENT un JSON valide respectant ce schéma, sans texte autour:");
            sb.AppendLine("{\"intent\": \"<l_intention>\", \"parameters\": { ... }}");
            sb.AppendLine("Intentions supportées : \"search_products\"(name, limit), \"get_product_details\"(id|name), \"get_order_status\"(orderNumber), \"get_user_orders\"(email, limit), \"top_products\"(limit).");
            sb.AppendLine("Si la question n'exige pas la base, réponds normalement en français sans JSON.");
            sb.AppendLine("Tables disponibles: Products(Id, Name, Description, Price, StockQuantity, IsActive), Orders(Id, OrderNumber, UserId, OrderDate, TotalAmount, Status), Users(Id, Email, FullName).");
            return sb.ToString();
        }

        private string CallLLM(List<ChatCompletionMessage> messages)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                // No API key, provide basic FAQ fallback
                return FallbackAnswer(messages);
            }

            try
            {
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
                    url = "https://api.openai.com/v1/chat/completions";
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
            catch
            {
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
            if (lastUser.IndexOf("commande", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Pour suivre une commande, indiquez votre numéro de commande (ex: ORD-12345).";
            }
            if (lastUser.IndexOf("produit", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Je peux rechercher des produits. Donnez-moi un nom ou une catégorie.";
            }
            return "Bonjour, je suis votre assistant. Comment puis-je vous aider ?";
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

        private string ExecuteIntent(string intentJson)
        {
            try
            {
                var dict = _serializer.Deserialize<Dictionary<string, object>>(intentJson);
                var intent = (dict.ContainsKey("intent") ? Convert.ToString(dict["intent"]) : string.Empty) ?? string.Empty;
                var parameters = new Dictionary<string, object>();
                if (dict.ContainsKey("parameters") && dict["parameters"] != null)
                {
                    parameters = (Dictionary<string, object>)dict["parameters"]; // JavaScriptSerializer uses Dictionary<string, object>
                }

                switch (intent)
                {
                    case "search_products":
                        return HandleSearchProducts(parameters);
                    case "get_product_details":
                        return HandleGetProductDetails(parameters);
                    case "get_order_status":
                        return HandleGetOrderStatus(parameters);
                    case "get_user_orders":
                        return HandleGetUserOrders(parameters);
                    case "top_products":
                        return HandleTopProducts(parameters);
                    default:
                        return "Je n'ai pas compris l'action demandée. Pouvez-vous préciser ?";
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
            int limit = ParamInt(parameters, "limit", 5);
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Veuillez indiquer le nom du produit à rechercher.";
            }

            var sql = @"SELECT TOP(@limit) Id, Name, Price, StockQuantity FROM Products WHERE IsActive = 1 AND Name LIKE @name ORDER BY ViewCount DESC";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[]
            {
                new SqlParameter("@limit", limit),
                new SqlParameter("@name", "%" + name + "%")
            });

            if (dt.Rows.Count == 0) return "Aucun produit trouvé.";
            var lines = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                lines.Add($"#{row["Id"]} — {row["Name"]} — {Convert.ToDecimal(row["Price"]):0.00} MAD — Stock: {row["StockQuantity"]}");
            }
            return "Produits trouvés:\n" + string.Join("\n", lines);
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

        private string HandleGetOrderStatus(Dictionary<string, object> parameters)
        {
            string orderNumber = Param(parameters, "orderNumber");
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                return "Veuillez fournir votre numéro de commande (ex: ORD-12345).";
            }

            var sql = @"SELECT TOP(1) OrderNumber, Status, TotalAmount, OrderDate FROM Orders WHERE OrderNumber=@num";
            var db = new DbContext();
            var dt = db.ExecuteQuery(sql, new[] { new SqlParameter("@num", orderNumber) });
            if (dt.Rows.Count == 0) return "Commande introuvable. Vérifiez le numéro.";
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
    }
}
