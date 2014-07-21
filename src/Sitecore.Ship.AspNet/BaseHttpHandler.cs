using System;
using System.Globalization;
using System.Net;
using System.Web;
using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Infrastructure.Configuration;
using Sitecore.Ship.Infrastructure.Diagnostics;
using Sitecore.Ship.Infrastructure.Web;

namespace Sitecore.Ship.AspNet
{
    public abstract class BaseHttpHandler : IHttpHandler
    {
        private readonly IAuthoriser _authoriser;
        private const string StartTime = "start_time";

        protected BaseHttpHandler(IAuthoriser authoriser)
        {
            _authoriser = authoriser;
        }

        protected BaseHttpHandler() : this(new HttpRequestAuthoriser(new HttpRequestChecker(), new PackageInstallationConfigurationProvider(), new Logger()))
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
            if (!_authoriser.IsAllowed())
            {
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            }

            context.Items.Add(StartTime, DateTime.UtcNow);

            try
            {
                ProcessRequest(new HttpContextWrapper(context));
            }
            finally
            {
                AddProcessingTimeToResponse(context);
            }
        }

        public abstract void ProcessRequest(HttpContextBase context);

        private static void AddProcessingTimeToResponse(HttpContext context)
        {
            var processTime = (DateTime.UtcNow - (DateTime)context.Items[StartTime]).TotalMilliseconds;

            context.Response.AddHeader("x-processing-time", processTime.ToString(CultureInfo.InvariantCulture));
        }
    }
}