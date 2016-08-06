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
                               RecordInstallationHistory = config.RecordInstallationHistory,
                               MuteAuthorisationFailureLogging = config.MuteAuthorisationFailureLogging,
                               AccessToken = config.AccessToken
                           };

            if (config.Whitelist.Count > 0)
            {
                foreach (var item in config.Whitelist)
                {
                    Settings.AddressWhitelist.Add(item.IP);
                }
            }
        }

        public PackageInstallationSettings Settings { get; private set; }
    }
}