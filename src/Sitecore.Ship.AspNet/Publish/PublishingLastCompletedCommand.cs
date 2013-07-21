using System;
using System.Net;
using System.Web;
using System.Web.Helpers;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Infrastructure;

namespace Sitecore.Ship.AspNet.Publish
{
    public class PublishingLastCompletedCommand : CommandHandler
    {
        private readonly IPublishService _publishService;

        public PublishingLastCompletedCommand(IPublishService publishService)
        {
            _publishService = publishService;
        }

        public PublishingLastCompletedCommand() : this(new PublishService())
        {            
        }

        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                var completedRequest = GetRequest(context.Request);

                var date = _publishService.GetLastCompletedRun(completedRequest);

                var json = Json.Encode(new { date });

                JsonResponse(json, HttpStatusCode.Accepted, context);
            }
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }

        private static PublishLastCompleted GetRequest(HttpRequestBase request)
        {
            if (request.Url == null) throw new InvalidOperationException("Missing Url");

            var parameters = new PublishLastCompleted
            {
                Source = request.Form["source"] ?? "master",
                Target = request.Form["target"] ?? "web",
                Language = request.Form["language"] ?? "en"
            };

            return parameters;
        }

        private static bool CanHandle(HttpContextBase context)
        {
            return context.Request.Url != null &&
                   context.Request.Url.PathAndQuery.ToLowerInvariant().Contains("/services/publish/lastcompleted");
        }
    }
}