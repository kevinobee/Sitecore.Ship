using System;
using System.Linq;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core
{
    public class HttpRequestAuthoriser : IAuthoriser
    {
        private readonly ICheckRequests _checkRequests;
        private readonly PackageInstallationSettings _packageInstallationSettings;
        private readonly ILog _logger;

        public HttpRequestAuthoriser(ICheckRequests checkRequests, PackageInstallationSettings packageInstallationSettings, ILog logger)
        {
            _checkRequests = checkRequests;
            _packageInstallationSettings = packageInstallationSettings;
            _logger = logger;
        }

        public bool IsAllowed()
        {
            if (!_packageInstallationSettings.IsEnabled)
            {
                LogAccessDenial("packageInstallation 'enabled' setting is false");
                return false;
            }

            if ((!_checkRequests.IsLocal) && (!_packageInstallationSettings.AllowRemoteAccess))
            {
                LogAccessDenial("packageInstallation 'allowRemote' setting is false");
                return false;
            }

//            if ((_context.Request.HttpMethod == "POST") && (!_configurationProvider.Settings.AllowPackageStreaming)) return false;

            if (_packageInstallationSettings.HasAddressWhitelist)
            {
                var foundAddress = _packageInstallationSettings.AddressWhitelist.Any(
                    x =>
                        string.Compare(x, _checkRequests.UserHostAddress, StringComparison.OrdinalIgnoreCase) == 0);

                if (!foundAddress)
                {
                    LogAccessDenial($"packageInstallation whitelist is denying access to {_checkRequests.UserHostAddress}");

                    return false;
                }
            }

            return true;
        }

        private void LogAccessDenial(string diagnostic)
        {
            if (!_packageInstallationSettings.MuteAuthorisationFailureLogging)
            {
                _logger.Write($"Sitecore.Ship access denied: {diagnostic}");
            }
        }
    }
}