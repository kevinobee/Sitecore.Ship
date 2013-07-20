using System;
using System.Text;
using System.Web;

namespace Sitecore.Ship.AspNet.Package
{
    public class InstallUploadPackageCommand : CommandHandler
    {
        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                var builder = new StringBuilder();
                builder.AppendFormat("TODO implement InstallUploadPackage command");

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
                   context.Request.Url.PathAndQuery.EndsWith("/services/package/install/fileupload", StringComparison.InvariantCultureIgnoreCase)
                //           TODO        && context.Request.HttpMethod == "POST"
                ;
        }
    }
}