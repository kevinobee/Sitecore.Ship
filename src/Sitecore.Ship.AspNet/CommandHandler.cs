using System.Net;
using System.Web;

namespace Sitecore.Ship.AspNet
{
    public abstract class CommandHandler
    {
        protected CommandHandler Successor;

        public void SetSuccessor(CommandHandler successor)
        {
            Successor = successor;
        }

        public abstract void HandleRequest(HttpContextBase context);

        protected static void JsonResponse(string json, HttpStatusCode statusCode, HttpContextBase context)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.Clear();
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Write(json);
        }
    }
}