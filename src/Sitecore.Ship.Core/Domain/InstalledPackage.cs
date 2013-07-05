using System;

namespace Sitecore.Ship.Core.Domain
{
    public class InstalledPackage
    {
        public string PackageId { get; set; }
        public DateTime DateInstalled { get; set; }
        public string Description { get; set; }
    }
}
