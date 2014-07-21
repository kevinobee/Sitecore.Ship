using Moq;
using Should;
using Xunit;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser.Logging
{
    public class HttpRequestAuthoriserLoggingWhitelistBehaviour : HttpRequestAuthoriserLoggingTest
    {
        private readonly string _userHostAddress;

        public HttpRequestAuthoriserLoggingWhitelistBehaviour()
        {
            PackageInstallationSettings.AddressWhitelist.Add("123.456.0.1");

            _userHostAddress = "123.456.0.2";

            CheckRequests.Setup(x => x.UserHostAddress).Returns(_userHostAddress);

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
            DiagnosticMessage.ShouldContain("packageInstallation whitelist is denying access to ");
        }

        [Fact]
        public void log_message_contains_diagnostic_contains_callers_IP()
        {
            DiagnosticMessage.ShouldContain(_userHostAddress);
        }
    }
}