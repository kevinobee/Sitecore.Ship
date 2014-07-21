using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.About
{
    public class AboutModule : ShipBaseModule
    {
        public AboutModule(IAuthoriser authoriser)
            : base(authoriser, "/services/about")
        {
            Get["/"] = parameters => View["about.html"];
        }
    }
}