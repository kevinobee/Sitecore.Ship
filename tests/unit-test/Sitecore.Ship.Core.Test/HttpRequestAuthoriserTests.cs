using System.Collections.Generic;
using Moq;
using Xunit;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Test
{
    public class HttpRequestAuthoriserTests
    {
        private readonly Mock<ICheckRequests> _checkRequests;
        private readonly Mock<IConfigurationProvider<PackageInstallationSettings>> _configurationProvider;
        private readonly HttpRequestAuthoriser _requestAuthoriser;

        public HttpRequestAuthoriserTests()
        {
            _checkRequests = new Mock<ICheckRequests>();
            _configurationProvider = new Mock<IConfigurationProvider<PackageInstallationSettings>>();
            _requestAuthoriser = new HttpRequestAuthoriser(_checkRequests.Object, _configurationProvider.Object);
        }

        [Fact]
        public void Should_return_false_when_configuration_settings_enabled_is_false()
        {
            // Arrange
            _configurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings {IsEnabled = false});

            // Act
            var isAllowed = _requestAuthoriser.IsAllowed();

            // Assert
            Assert.False(isAllowed);
        }

        [Fact]
        public void Should_return_false_when_remote_request_is_disallowed_by_configuration_settings()
        {
            // Arrange
            _configurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings { IsEnabled = true, AllowRemoteAccess = false });
            _checkRequests.Setup(x => x.IsLocal).Returns(false);

            // Act
            var isAllowed = _requestAuthoriser.IsAllowed();

            // Assert
            Assert.False(isAllowed);
        }

        [Fact]
        public void Should_return_true_for_allowed_request()
        {
            // Arrange
            _configurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings { IsEnabled = true });
            _checkRequests.Setup(x => x.IsLocal).Returns(true);

            // Act
            var isAllowed = _requestAuthoriser.IsAllowed();

            // Assert
            Assert.True(isAllowed);
        }

        [Fact]
        public void Should_return_false_if_whitelist_exists_and_does_not_contain_requesting_ip()
        {
            // Arrange
            _configurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings { IsEnabled = true, AddressWhitelist = new List<string> { "1.2.3.4"} });
            _checkRequests.Setup(x => x.IsLocal).Returns(true);
            _checkRequests.Setup(x => x.UserHostAddress).Returns("2.3.4.5");

            // Act
            var isAllowed = _requestAuthoriser.IsAllowed();

            // Assert
            Assert.False(isAllowed);
        }

        [Fact]
        public void Should_return_true_if_whitelist_exists_and_does_contain_requesting_ip()
        {
            // Arrange
            _configurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings { IsEnabled = true, AddressWhitelist = new List<string> { "1.2.3.4" } });
            _checkRequests.Setup(x => x.IsLocal).Returns(true);
            _checkRequests.Setup(x => x.UserHostAddress).Returns("1.2.3.4");

            // Act
            var isAllowed = _requestAuthoriser.IsAllowed();

            // Assert
            Assert.True(isAllowed);
        }
    }
}
