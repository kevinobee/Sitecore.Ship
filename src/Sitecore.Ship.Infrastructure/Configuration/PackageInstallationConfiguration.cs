using System.Configuration;

namespace Sitecore.Ship.Infrastructure.Configuration
{
    internal class PackageInstallationConfiguration : ConfigurationSection
    {
        public static PackageInstallationConfiguration GetConfiguration()
        {
            var configuration = ConfigurationManager.GetSection("packageInstallation") as PackageInstallationConfiguration;
            return configuration ?? new PackageInstallationConfiguration();
        }

        private const string EnabledKey = "enabled";
        private const string AllowRemoteKey = "allowRemote";
        private const string AllowPackageStreamingKey = "allowPackageStreaming";
        private const string RecordInstallationHistoryKey = "recordInstallationHistory";

        [ConfigurationProperty(EnabledKey, IsRequired = false, DefaultValue = false)]
        public bool Enabled { get { return (bool)this[EnabledKey]; } }

        [ConfigurationProperty(AllowRemoteKey, IsRequired = false, DefaultValue = false)]
        public bool AllowRemoteAccess { get { return (bool) this[AllowRemoteKey]; } }

        [ConfigurationProperty(AllowPackageStreamingKey, IsRequired = false, DefaultValue = false)]
        public bool AllowPackageStreaming { get { return (bool)this[AllowPackageStreamingKey]; } }

        [ConfigurationProperty(RecordInstallationHistoryKey, IsRequired = false, DefaultValue = false)]
        public bool RecordInstallationHistory { get { return (bool)this[RecordInstallationHistoryKey]; } }
    }
}