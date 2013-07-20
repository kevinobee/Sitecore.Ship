using System.Net;
using System.Web;
using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Infrastructure.Configuration;
using Sitecore.Ship.Infrastructure.Web;

namespace Sitecore.Ship.AspNet
{
    public abstract class BaseHttpHandler : IHttpHandler
    {
        private readonly IAuthoriser _authoriser;

        protected BaseHttpHandler(IAuthoriser authoriser)
        {
            _authoriser = authoriser;
        }

        protected BaseHttpHandler() : this(new HttpRequestAuthoriser(new HttpRequestChecker(), new PackageInstallationConfigurationProvider()))
        {
            
        }

        public virtual bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            // TODO KO use abstractions ILog
            //            Sitecore.Diagnostics.Log.Audit(this, "Started at: {0}", new[] { DateTime.Now.ToLongTimeString() });

            if (!_authoriser.IsAllowed())
            {
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            }

            ProcessRequest(new HttpContextWrapper(context));

            //            Sitecore.Diagnostics.Log.Audit(this, "Ended at: {0}", new[] { DateTime.Now.ToLongTimeString() });
        }

        public abstract void ProcessRequest(HttpContextBase context);
    }
}