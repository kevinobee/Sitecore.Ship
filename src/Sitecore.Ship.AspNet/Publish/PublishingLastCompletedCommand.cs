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

                // serialize and send..
                var json = Json.Encode(new { date });

                context.Response.StatusCode = (int) HttpStatusCode.Accepted;
                context.Response.Clear();
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.Write(json);
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
                Target = request.Form["target"] ?? "web", // TODO check form parmaeter
                Language = request.Form["language"]?? "en"  // TODO check form parmaeter
            };

            return parameters;
        }
        private static bool CanHandle(HttpContextBase context)
        {
//            Get["/lastcompleted"] = LastCompleted;
//            Get["/lastcompleted/{source}/{target}/{language}"] = LastCompleted;

            return context.Request.Url != null &&
                   context.Request.Url.PathAndQuery.ToLowerInvariant().Contains("/services/publish/lastcompleted");
        }
    }
}