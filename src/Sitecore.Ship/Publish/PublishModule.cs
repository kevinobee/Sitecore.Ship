using System;
using System.Linq;

using Nancy;
using Nancy.ModelBinding;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Publish
{
    public class PublishModule : ShipBaseModule
    {
        private readonly IPublishService _publishService;

        public PublishModule(IPublishService publishService, IAuthoriser authoriser)
            : base(authoriser, "/services/publish")
        {
            _publishService = publishService;

            Post["/{mode}"] = InvokePublishing;
            Post["/listofitems"] = InvokePublishingOfListOfItems;
            Get["/lastcompleted"] = LastCompleted;
            Get["/lastcompleted/{source}/{target}/{language}"] = LastCompleted;
        }

        private dynamic LastCompleted(dynamic o)
        {
            var completedRequest = this.Bind<PublishLastCompletedRequest>();

            var parameters = new PublishLastCompleted
                {
                    Source = completedRequest.Source ?? "master",
                    Target = completedRequest.Target ?? "web",
                    Language = completedRequest.Language ?? "en"
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

        private dynamic InvokePublishingOfListOfItems(dynamic o)
        {
			ItemsToPublish itemsToPublish;

	        try
	        {
				itemsToPublish = this.Bind<ItemsToPublish>();
	        }
	        catch (Exception exception)
	        {
				return Response.AsJson(exception.Message, HttpStatusCode.BadRequest);
	        }

            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			
			if (itemsToPublish == null)
	        {
				return Response.AsJson(date, HttpStatusCode.BadRequest);
	        }

			if (itemsToPublish.Items.Count == 0)
			{
				return Response.AsJson(date, HttpStatusCode.NoContent);
			}

            _publishService.Run(itemsToPublish);

            return Response.AsJson(date, HttpStatusCode.Accepted);
        }

        private static bool IsAllowedPublishingMode(string mode)
        {
            var publishingModes = new[] { "full", "smart", "incremental" };

            return publishingModes.Any(x => string.Compare(x, mode, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }
}
