using System;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Ecommerce.Chatbot;

namespace Ecommerce.Services
{
    [WebService(Namespace = "http://ecommerce.ma/chatbot")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class ChatbotService : WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ChatbotResponse SendMessage(string message)
        {
            var logic = new ChatbotLogic();
            return logic.ProcessMessage(message, HttpContext.Current);
        }
    }
}
