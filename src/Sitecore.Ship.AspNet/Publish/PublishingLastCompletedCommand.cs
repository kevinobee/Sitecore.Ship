using System.Text;
using System.Web;

namespace Sitecore.Ship.AspNet.Publish
{
    public class PublishingLastCompletedCommand : CommandHandler
    {
        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                var builder = new StringBuilder();
                builder.AppendFormat("TODO implement PublishingLastCompletedCommand command");

                context.Response.Write(builder.ToString());
            }
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
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