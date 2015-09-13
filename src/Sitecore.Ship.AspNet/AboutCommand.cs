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
                var assemblyName = GetType().Assembly.GetName();
                var applicationName = assemblyName.Name;
                var version = assemblyName.Version;

                var builder = new StringBuilder();
                builder.AppendFormat("{0} - version {1}", applicationName, version);

                context.Response.ContentType = "text/plain";
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