using Moq;
using Xunit;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser.Logging
{
    public class HttpRequestAuthoriserLoggingConfigurationBehaviour : HttpRequestAuthoriserLoggingTest
    {
        public HttpRequestAuthoriserLoggingConfigurationBehaviour()
        {
            PackageInstallationSettings.IsEnabled = false;
        }

        [Fact]
        public void Should_log_authorisation_failures_by_default()
        {
            RequestAuthoriser.IsAllowed();

            Logger.Verify(x => x.Write(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void Should_not_log_authorisation_failures_when_prevented_by_configuration()
        {
            PackageInstallationSettings.MuteAuthorisationFailureLogging = true;

            RequestAuthoriser.IsAllowed();

            Logger.Verify(x => x.Write(It.IsAny<string>()), Times.Never());
        }
    }
}