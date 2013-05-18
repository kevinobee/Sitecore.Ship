using System.IO;
using Nancy;
using Sitecore.Ship.Core;

namespace Sitecore.Ship.Package.Install
{
    public static class PackageResponseExtensions
    {
        public static Response AsNewPackage(this IResponseFormatter formatter, InstallPackage package)
        {
            string fileName = Path.GetFileName(package.Path);
            string url = string.Format("{0}/{1}", formatter.Context.Request.Url, fileName);

            return new Response
                       {
                           StatusCode = HttpStatusCode.Created
                       }
                .WithHeader("Location", url);
        }
    }
}