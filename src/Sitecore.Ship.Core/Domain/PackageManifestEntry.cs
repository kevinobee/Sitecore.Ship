using System;

namespace Sitecore.Ship.Core.Domain
{
    public class PackageManifestEntry
    {
        public Guid? ID { get; set; }
        public string Path { get; set; }
    }

    public class FileManifestEntry : PackageManifestEntry
    {
        public FileManifestEntry(string path)
        {
            ID = null;
            Path = path;
        } 
    }

    public class PackageManifestEntryNotFound : PackageManifestEntry
    {
        public PackageManifestEntryNotFound()
        {
            ID = null;
            Path = string.Empty;
        }
    }
}