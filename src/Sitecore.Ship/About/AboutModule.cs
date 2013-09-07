using Nancy;

namespace Sitecore.Ship.About
{
    public class AboutModule : NancyModule
    {
        public AboutModule()
            : base("/services/about")
        {
            Get["/"] = parameters => View["about.html"];
        }
    }
}