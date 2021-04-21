using System.Collections.ObjectModel;

namespace Sitecore.Ship.Core.Domain
{
    public class PackageManifest
    {
        public PackageManifest()
        {
            Entries = new Collection<PackageManifestEntry>();
        }
        
        public Collection<PackageManifestEntry> Entries { get; private set; } 
    }
}