namespace Sitecore.Ship.Core.Domain
{
    public class InstallPackage
    {
        public string Path { get; set; }

        public bool DisableIndexing { get; set; }

        /// <summary>
        /// Set to true to disable reporting of items contained in the package.
        /// </summary>
        public bool DisableManifest { get; set; }
    }
}