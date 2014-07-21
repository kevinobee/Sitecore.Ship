using System.Collections.Generic;
using Sitecore.Ship.Core.Domain;
using Xunit;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser
{
    public class HttpRequestAuthoriserBehaviour : HttpRequestAuthoriserTest
    {
        [Fact]
        public void Should_return_false_when_configuration_settings_enabled_is_false()
        {
            // Arrange
            ConfigurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings {IsEnabled = false});

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.False(isAllowed);
        }

        [Fact]
        public void Should_return_false_when_remote_request_is_disallowed_by_configuration_settings()
        {
            // Arrange
            ConfigurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings { IsEnabled = true, AllowRemoteAccess = false });
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
            ConfigurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings { IsEnabled = true });
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
            ConfigurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings { IsEnabled = true, AddressWhitelist = new List<string> { "1.2.3.4"} });
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
            ConfigurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings { IsEnabled = true, AddressWhitelist = new List<string> { "1.2.3.4" } });
            CheckRequests.Setup(x => x.IsLocal).Returns(true);
            CheckRequests.Setup(x => x.UserHostAddress).Returns("1.2.3.4");

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.True(isAllowed);
        }




    }
}
