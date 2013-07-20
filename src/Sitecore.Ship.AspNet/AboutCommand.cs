using System;
using System.Text;
using System.Web;

namespace Sitecore.Ship.AspNet
{
    public class AboutCommand : CommandHandler
    {
        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                var version = GetType().Assembly.GetName().Version;

                var builder = new StringBuilder();
                builder.AppendFormat("Sitecore.Ship - version {0}", version);

                context.Response.Write(builder.ToString());
            }
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }

        private static bool CanHandle(HttpContextBase context)
        {
            return context.Request.Url != null &&  context.Request.Url.PathAndQuery.EndsWith("/services/about", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}