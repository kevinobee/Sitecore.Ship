using System;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Sitecore.Ship.Core;

namespace Sitecore.Ship.Publish
{
    public class PublishModule : NancyModule
    {
        private readonly IPublishService _publishService;

        public PublishModule(IPublishService publishService)
            : base("/services/publish")
        {
            _publishService = publishService;
            Post["/{mode}"] = InvokePublishing;
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
                    Targets = DecodeCsvStringParam(publishRequest.Targets, new [] { "web"}),
                    Languages = DecodeCsvStringParam(publishRequest.Languages, new[] { "en" }),
                };

            _publishService.Run(publishParameters);

            return new Response
                {
                    StatusCode = HttpStatusCode.Accepted
                };
        }

        private static string[] DecodeCsvStringParam(string inputValue, string[] defaultValue)
        {
            if (string.IsNullOrWhiteSpace(inputValue)) return defaultValue;

            return inputValue.Split(new[] { ',' }).Select(x => x.Trim()).ToArray();
        }

        private static bool IsAllowedPublishingMode(string mode)
        {
            var publishingModes = new[] { "full", "smart", "incremental" };

            return publishingModes.Any(x => string.Compare(x, mode, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }
}