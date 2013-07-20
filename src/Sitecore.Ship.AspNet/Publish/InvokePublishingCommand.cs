using System;
using System.Text;
using System.Web;

namespace Sitecore.Ship.AspNet.Publish
{
    public class InvokePublishingCommand : CommandHandler
    {
        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                var builder = new StringBuilder();
                builder.AppendFormat("TODO implement InvokePublishingCommand command");

                context.Response.Write(builder.ToString());
            }
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }

        private static bool CanHandle(HttpContextBase context)
        {
            return context.Request.Url != null &&
                   context.Request.Url.PathAndQuery.EndsWith("/services/publish/{mode}", StringComparison.InvariantCultureIgnoreCase)  // TODO {mode} == ....
                //           TODO        && context.Request.HttpMethod == "POST"
                ;
        }
    }
}