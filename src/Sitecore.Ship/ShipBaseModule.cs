using System;
using System.Globalization;
using Nancy;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship
{
    public abstract class ShipBaseModule : NancyModule
    {
        private readonly IAuthoriser _authoriser;

        const string StartTime = "start_time";


        protected ShipBaseModule(IAuthoriser authoriser, string modulePath)
            : base(modulePath)
        {
            _authoriser = authoriser;

            Before += AuthoriseRequest;

            Before += ctx =>
            {
                ctx.Items.Add(StartTime, DateTime.UtcNow);
                return null;
            };

            After += AddProcessingTimeToResponse;
        }

        private Response AuthoriseRequest(NancyContext ctx)
        {
            return !_authoriser.IsAllowed() 
                        ? new Response { StatusCode = HttpStatusCode.Unauthorized } 
                        : null;
        }

        private static void AddProcessingTimeToResponse(NancyContext ctx)
        {
            if (!ctx.Items.ContainsKey(StartTime)) return;

            var processTime = (DateTime.UtcNow - (DateTime)ctx.Items[StartTime]).TotalMilliseconds;

            ctx.Response.WithHeader("x-processing-time", processTime.ToString(CultureInfo.InvariantCulture));
        }
    }
}