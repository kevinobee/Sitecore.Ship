using Sitecore.Ship.Core.Domain;

using System.Collections.ObjectModel;

using Xunit;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser
{
    public class HttpRequestAuthoriserBehaviour : HttpRequestAuthoriserTest
    {
        [Fact]
        public void Should_return_false_when_configuration_settings_enabled_is_false()
        {
            // Arrange
            PackageInstallationSettings.IsEnabled = false;

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.False(isAllowed);
        }

        [Fact]
        public void Should_return_false_when_remote_request_is_disallowed_by_configuration_settings()
        {
            // Arrange
            PackageInstallationSettings.IsEnabled = false;
            PackageInstallationSettings.AllowRemoteAccess = false;
            
            CheckRequests.Setup(x => x.IsLocal).Returns(false);

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.False(isAllowed);
        }

        [Fact]
        public void Should_return_true_for_allowed_request()
        {
            // Arrange
            PackageInstallationSettings.IsEnabled = true;

            CheckRequests.Setup(x => x.IsLocal).Returns(true);

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.True(isAllowed);
        }

        [Fact]
        public void Should_return_false_if_whitelist_exists_and_does_not_contain_requesting_ip()
        {
            // Arrange
            PackageInstallationSettings.IsEnabled = true;
            PackageInstallationSettings.AddressWhitelist = new Collection<string> { "1.2.3.4"};

            CheckRequests.Setup(x => x.IsLocal).Returns(true);
            CheckRequests.Setup(x => x.UserHostAddress).Returns("2.3.4.5");

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.False(isAllowed);
        }

        [Fact]
        public void Should_return_true_if_whitelist_exists_and_does_contain_requesting_ip()
        {
            // Arrange
            PackageInstallationSettings.IsEnabled = true;
            PackageInstallationSettings.AddressWhitelist = new Collection<string> { "1.2.3.4" }; 
            
            CheckRequests.Setup(x => x.IsLocal).Returns(true);
            CheckRequests.Setup(x => x.UserHostAddress).Returns("1.2.3.4");

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.True(isAllowed);
        }




    }
}
