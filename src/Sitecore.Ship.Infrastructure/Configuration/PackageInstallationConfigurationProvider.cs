using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

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
                               AllowPackageStreaming = config.AllowPackageStreaming,
                               RecordInstallationHistory = config.RecordInstallationHistory
                           };
        }

        public PackageInstallationSettings Settings { get; private set; }
    }
}