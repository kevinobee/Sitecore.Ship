namespace Sitecore.Ship.Core.Domain
{
    public class InstallUploadPackage
    {
        public string PackageId { get; set; }
        public string Description { get; set; }
        public bool DisableIndexing { get; set; }

        /// <summary>
        /// Set to true to disable reporting of items contained in the package.
        /// </summary>
        public bool DisableManifest { get; set; }
    }
}