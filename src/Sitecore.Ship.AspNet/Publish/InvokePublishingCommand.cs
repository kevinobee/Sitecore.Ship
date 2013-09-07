using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Infrastructure;

namespace Sitecore.Ship.AspNet.Publish
{
    public class InvokePublishingCommand : CommandHandler
    {
        private readonly IPublishService _publishService;

        public InvokePublishingCommand(IPublishService publishService)
        {
            _publishService = publishService;
        }

        public InvokePublishingCommand() : this(new PublishService())
        {            
        }

        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                var publishParameters = GetRequest(context.Request);

                var now = DateTime.Now;
                var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                _publishService.Run(publishParameters);

                var json = Json.Encode(new { date });

                JsonResponse(json, HttpStatusCode.Accepted, context);
            }
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }

        private static bool CanHandle(HttpContextBase context)
        {
            return context.Request.Url != null &&
                   IsPublishModeUrl(context.Request.Url.PathAndQuery.ToLowerInvariant()) && 
                   context.Request.HttpMethod == "POST";
        }

        private static bool IsPublishModeUrl(string urlPath)
        {
            return urlPath.EndsWith("/services/publish/full") ||  
                   urlPath.EndsWith("/services/publish/smart") ||  
                   urlPath.EndsWith("/services/publish/incremental");
        }

        private static PublishParameters GetRequest(HttpRequestBase request)
        {
            if (request.Url == null) throw new InvalidOperationException("Missing Url");

            var publishRequest = new PublishParameters
                {
                    Mode = request.Url.PathAndQuery.Split(new[] {'/'}).Last(),
                    Source = request.Form["source"] ?? "master",
                    Targets = request.Form["targets"].CsvStringToStringArray(new[] { "web" }),
                    Languages = request.Form["languages"].CsvStringToStringArray(new[] { "en" })
                };

            return publishRequest;
        }
    }
}