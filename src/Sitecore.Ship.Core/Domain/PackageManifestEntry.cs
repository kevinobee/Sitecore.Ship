using System;

namespace Sitecore.Ship.Core.Domain
{
    public class PackageManifestEntry
    {
        public Guid ID { get; set; }
        public string Path { get; set; }
    }

    public class PackageManifestEntryNotFound : PackageManifestEntry
    {
        public PackageManifestEntryNotFound()
        {
            ID = Guid.Empty;
            Path = string.Empty;
        }
    }
}