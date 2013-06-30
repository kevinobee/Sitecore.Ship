using System.Collections.Generic;
using System.Linq;

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
                               AllowPackageStreaming = config.AllowPackageStreaming
                           };

            if (config.Whitelist.Count > 0)
            {
                foreach (var item in config.Whitelist)
                {
                    Settings.AddressWhitelist.Add(item.IP);
                }

//                Settings.AddressWhitelist = from x in config.Whitelist select x.IP;
            }
        }

        public PackageInstallationSettings Settings { get; private set; }
    }
}