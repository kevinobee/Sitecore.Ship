using System.Collections.Generic;

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
            AddressWhitelist = new List<string>();
            MuteAuthorisationFailureLogging = false;
        }

        public bool IsEnabled { get; set; }
        public bool AllowRemoteAccess { get; set; }
        public bool AllowPackageStreaming { get; set; }
        public bool RecordInstallationHistory { get; set; }
        public List<string> AddressWhitelist { get; set; }
        public bool MuteAuthorisationFailureLogging { get; set; }

        public bool HasAddressWhitelist { get { return AddressWhitelist.Count > 0; } }
    }
}