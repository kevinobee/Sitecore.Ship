using System;
using System.Linq;

using Nancy;
using Nancy.ModelBinding;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Publish
{
    public class PublishModule : NancyModule
    {
        private readonly IPublishService _publishService;
        private readonly IAuthoriser _authoriser;

        public PublishModule(IPublishService publishService, IAuthoriser authoriser)
            : base("/services/publish")
        {
            _publishService = publishService;
            _authoriser = authoriser;

            Before += AuthoriseRequest; 

            Post["/{mode}"] = InvokePublishing;
            Get["/lastcompleted"] = LastCompleted;
            Get["/lastcompleted/{source}/{target}/{language}"] = LastCompleted;
        }

        private Response AuthoriseRequest(NancyContext ctx)
        {
            if (!_authoriser.IsAllowed())
            {
                ctx.Response =
                 new Response { StatusCode = HttpStatusCode.Unauthorized };
            }
            return null;
        }

        private dynamic LastCompleted(dynamic o)
        {
            var completedRequest = this.Bind<PublishLastCompletedRequest>();

            var parameters = new PublishLastCompleted
                {
                    Language = completedRequest.Language ?? "en",
                    Target = completedRequest.Target ?? "web",
                    Source = completedRequest.Source ?? "master"
                };
            
            var date = _publishService.GetLastCompletedRun(parameters);

            return Response.AsJson(date);
        }

        private dynamic InvokePublishing(dynamic o)
        {
            var publishRequest = this.Bind<PublishRequest>();

            if (!IsAllowedPublishingMode(publishRequest.Mode))
            {
                return new Response
                    {
                        StatusCode = HttpStatusCode.BadRequest
                    };
            }

            var publishParameters = new PublishParameters
                {
                    Mode = publishRequest.Mode,
                    Source = publishRequest.Source ?? "master",
                    Targets = publishRequest.Targets.CsvStringToStringArray(new[] {"web"}),
                    Languages = publishRequest.Languages.CsvStringToStringArray(new[] {"en"}),
                };

            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

            _publishService.Run(publishParameters);

            return Response.AsJson(date, HttpStatusCode.Accepted);
        }

        private static bool IsAllowedPublishingMode(string mode)
        {
            var publishingModes = new[] { "full", "smart", "incremental" };

            return publishingModes.Any(x => string.Compare(x, mode, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }
}
