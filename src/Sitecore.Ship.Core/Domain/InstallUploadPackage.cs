namespace Sitecore.Ship.Core.Domain
{
    public class InstallUploadPackage
    {
        public string PackageId { get; set; }
        public string Description { get; set; }
        public bool DisableIndexing { get; set; }
    }
}