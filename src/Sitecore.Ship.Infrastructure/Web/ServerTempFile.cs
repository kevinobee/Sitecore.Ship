using System;
using System.Web;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.Infrastructure.Web
{
    public class ServerTempFile : ITempFile
    {
        public string Filename
        {
            get
            {
                return HttpContext.Current.Server.MapPath(Sitecore.IO.TempFolder.GetFilename(Guid.NewGuid() + ".update"));
            }
        }
    }
}