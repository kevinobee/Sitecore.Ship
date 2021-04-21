using System;
using System.Net;
using System.Web;

namespace Sitecore.Ship.AspNet
{
    public class UnhandledCommand : CommandHandler
    {
        public override void HandleRequest(HttpContextBase context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
        }
    }
}