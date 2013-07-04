namespace Sitecore.Ship.Core.Domain
{
    public class PackageInstallationSettings
    {
        public PackageInstallationSettings()
        {
            IsEnabled = false;
            AllowRemoteAccess = false;
            AllowPackageStreaming = false;
            RecordInstallationHistory = false;
        }

        public bool IsEnabled { get; set; }
        public bool AllowRemoteAccess { get; set; }
        public bool AllowPackageStreaming { get; set; }
        public virtual bool RecordInstallationHistory { get; set; }
    }
}