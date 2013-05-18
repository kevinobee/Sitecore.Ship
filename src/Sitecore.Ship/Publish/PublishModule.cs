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

        public PublishModule(IPublishService publishService): base("/services")
        {
            _publishService = publishService;
            Post["/publish/{mode}"] = InvokePublishing;
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

            _publishService.Run(publishRequest.Mode);

            return new Response
                {
                    StatusCode = HttpStatusCode.Accepted
                };
        }

        private static bool IsAllowedPublishingMode(string mode)
        {
            var publishingModes = new[] { "full", "smart", "incremental" };

            return publishingModes.Any(x => string.Compare(x, mode, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }
}