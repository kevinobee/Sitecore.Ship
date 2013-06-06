using System.Web;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.Infrastructure.Web
{
    public class HttpRequestChecker : ICheckRequests
    {
        public bool IsLocal
        {
            get { return HttpContext.Current.Request.IsLocal; }
        }
    }
}