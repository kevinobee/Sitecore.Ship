using System.Collections.Generic;
using System.Collections.ObjectModel;

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
            AddressWhitelist = new Collection<string>();
            MuteAuthorisationFailureLogging = false;
        }

        public bool IsEnabled { get; set; }
        public bool AllowRemoteAccess { get; set; }
        public bool AllowPackageStreaming { get; set; }
        public bool RecordInstallationHistory { get; set; }
        public Collection<string> AddressWhitelist { get; set; }
        public bool MuteAuthorisationFailureLogging { get; set; }

        public bool HasAddressWhitelist { get { return AddressWhitelist.Count > 0; } }
    }
}