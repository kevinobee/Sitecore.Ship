using System;
using Moq;
using Should;
using Xunit;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser.Logging
{
    public class HttpRequestAuthoriserLoggingDisabledBehaviour : HttpRequestAuthoriserLoggingTest
    {
        public HttpRequestAuthoriserLoggingDisabledBehaviour()
        {
            PackageInstallationSettings.IsEnabled = false;

            RequestAuthoriser.IsAllowed();            
        }

        [Fact]
        public void failures_are_logged()
        {
            Logger.Verify(x => x.Write(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void log_message_contains_diagnostic()
        {
            DiagnosticMessage.ShouldContain("packageInstallation 'enabled' setting is false", StringComparison.OrdinalIgnoreCase);
        }
    }
}