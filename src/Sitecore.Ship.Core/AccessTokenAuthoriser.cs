using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core
{
    public class AccessTokenAuthoriser : IAuthoriser
    {
        private readonly ICheckRequests _checkRequests;
        private readonly PackageInstallationSettings _packageInstallationSettings;
        private readonly ILog _logger;

        private const string AuthorizationHeaderKey = "Authorization";

        public AccessTokenAuthoriser(ICheckRequests checkRequests,
            PackageInstallationSettings packageInstallationSettings, ILog logger)
        {
            if (checkRequests == null) 
                throw new ArgumentNullException("checkRequests");
            if (packageInstallationSettings == null) 
                throw new ArgumentNullException("packageInstallationSettings");

            _checkRequests = checkRequests;
            _packageInstallationSettings = packageInstallationSettings;
            _logger = logger;
        }

        public bool IsAllowed()
        {
            if (!_packageInstallationSettings.TokenRequired)
                return true;

            if (_checkRequests.Headers == null || !_checkRequests.Headers.AllKeys.Contains(AuthorizationHeaderKey))
            {
                LogAccessDenial(AuthorizationHeaderKey + " header missing");
                return false;
            }

            var allAuthorizationHeadervalues = _checkRequests.Headers.GetValues(AuthorizationHeaderKey);

// ReSharper disable once AssignNullToNotNullAttribute
            string bearerHeader = allAuthorizationHeadervalues.FirstOrDefault(authorizationHeader => authorizationHeader.StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase));

            if (bearerHeader == null)
            {
                LogAccessDenial("Bearer authentication scheme required");
                return false;
            }

            var token = bearerHeader.Substring(6).Trim();

            var success = token == _packageInstallationSettings.AccessToken;

            if (!success)
            {
                LogAccessDenial("Wrong Authentication Token");
            }

            return success;
        }

        private void LogAccessDenial(string diagnostic)
        {
            if (!_packageInstallationSettings.MuteAuthorisationFailureLogging)
            {
                _logger.Write(string.Format("Sitecore.Ship access denied: {0}", diagnostic));
            }
        }
    }
}
