using System.Collections.Specialized;
using Moq;
using Should;
using Xunit;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser.Logging
{
    public class AccessTokenAuthoriserLoggingBehaviour : HttpRequestAuthoriserLoggingTest
    {
        public AccessTokenAuthoriserLoggingBehaviour()
        {
            PackageInstallationSettings.AllowRemoteAccess = true;
            PackageInstallationSettings.AccessToken = "MyToken";
        }

        [Fact]
        public void Should_log_authorisation_failures_for_missing_authorization_header()
        {
            //Act
            RequestAuthoriser.IsAllowed();

            //Assert
            Logger.Verify(x => x.Write(It.IsAny<string>()), Times.Once());
            DiagnosticMessage.ShouldContain("Authorization");
            DiagnosticMessage.ShouldContain("missing");
        }

        [Fact]
        public void Should_log_authorisation_failures_for_wrong_authentication_scheme()
        {
            //Arrange
            CheckRequests.Setup(x => x.Headers).Returns(new NameValueCollection
            {
                {"Authorization", "Basic MyToken"}
            });


            //Act
            RequestAuthoriser.IsAllowed();

            //Assert
            Logger.Verify(x => x.Write(It.IsAny<string>()), Times.Once());
            DiagnosticMessage.ShouldContain("Bearer");
            DiagnosticMessage.ShouldContain("required");
        }

        [Fact]
        public void Should_log_authorisation_failures_for_wrong_token()
        {
            //Arrange
            CheckRequests.Setup(x => x.Headers).Returns(new NameValueCollection
            {
                {"Authorization", "Bearer NotValid"}
            });
            
            //Act
            RequestAuthoriser.IsAllowed();

            //Assert
            Logger.Verify(x => x.Write(It.IsAny<string>()), Times.Once());
            DiagnosticMessage.ShouldContain("Token");
        }

        [Fact]
        public void Should_not_log_success_authorisation()
        {
            //Arrange
            CheckRequests.Setup(x => x.Headers).Returns(new NameValueCollection
            {
                {"Authorization", "Bearer MyToken"}
            });
            
            //Act
            RequestAuthoriser.IsAllowed();

            //Assert
            Logger.Verify(x => x.Write(It.IsAny<string>()), Times.Never());
        }
    }
}