using System;
using System.Linq;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core
{
    public class HttpRequestAuthoriser : IAuthoriser
    {
        private readonly ICheckRequests _checkRequests;
        private readonly IConfigurationProvider<PackageInstallationSettings> _configurationProvider;
        private readonly ILog _logger;

        public HttpRequestAuthoriser(ICheckRequests checkRequests, IConfigurationProvider<PackageInstallationSettings> configurationProvider, ILog logger)
        {
            _checkRequests = checkRequests;
            _configurationProvider = configurationProvider;
            _logger = logger;
        }

        public bool IsAllowed()
        {
            if (! _configurationProvider.Settings.IsEnabled)
            {
                LogAccessDenial("packageInstallation 'enabled' setting is false");
                return false;
            }

            if ((!_checkRequests.IsLocal) && (!_configurationProvider.Settings.AllowRemoteAccess))
            {
                LogAccessDenial("packageInstallation 'allowRemote' setting is false");
                return false;
            }

//            if ((_context.Request.HttpMethod == "POST") && (!_configurationProvider.Settings.AllowPackageStreaming)) return false;

            if (_configurationProvider.Settings.HasAddressWhitelist)
            {
                var foundAddress = _configurationProvider.Settings.AddressWhitelist.Any(
                    x =>
                        string.Compare(x, _checkRequests.UserHostAddress, StringComparison.InvariantCultureIgnoreCase) == 0);

                if (!foundAddress)
                {
                    LogAccessDenial(string.Format("packageInstallation whitelist is denying access to {0}", _checkRequests.UserHostAddress));

                    return false;
                }
            }

            return true;
        }

        private void LogAccessDenial(string diagnostic)
        {
            _logger.Write(string.Format("Ship access denied: {0}", diagnostic));
        }
    }
}