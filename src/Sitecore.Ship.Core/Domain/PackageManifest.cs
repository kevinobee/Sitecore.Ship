using System.Collections.Generic;

namespace Sitecore.Ship.Core.Domain
{
    public class PackageManifest
    {
        public PackageManifest()
        {
            Entries = new List<PackageManifestEntry>();
        }
        
        public List<PackageManifestEntry> Entries { get; private set; } 
    }
}