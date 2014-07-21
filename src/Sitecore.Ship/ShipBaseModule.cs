using Nancy;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship
{
    public abstract class ShipBaseModule : NancyModule
    {
        private readonly IAuthoriser _authoriser;

        protected ShipBaseModule(IAuthoriser authoriser, string modulePath)
            : base(modulePath)
        {
            _authoriser = authoriser;

            Before += AuthoriseRequest; 
        }

        private Response AuthoriseRequest(NancyContext ctx)
        {
            if (!_authoriser.IsAllowed())
            {
                ctx.Response = new Response { StatusCode = HttpStatusCode.Unauthorized };
            }
            return null;
        }
    }
}