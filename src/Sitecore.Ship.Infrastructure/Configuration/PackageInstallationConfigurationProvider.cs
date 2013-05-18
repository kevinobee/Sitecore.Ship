using Sitecore.Ship.Core;

namespace Sitecore.Ship.Infrastructure.Configuration
{
    public class PackageInstallationConfigurationProvider : IConfigurationProvider<PackageInstallationSettings>
    {
        public PackageInstallationConfigurationProvider()
        {
            var config = PackageInstallationConfiguration.GetConfiguration();

            Settings = new PackageInstallationSettings
                           {
                               IsEnabled = config.Enabled,
                               AllowRemoteAccess = config.AllowRemoteAccess,
                               AllowPackageStreaming = config.AllowPackageStreaming
                           };
        }

        public PackageInstallationSettings Settings { get; private set; }
    }
}