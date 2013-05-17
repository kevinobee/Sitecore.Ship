using System.Web;

using Moq;
using NUnit.Framework;

using Ship.Web.Update;

namespace Ship.Web.Tests.Update
{
    [TestFixture]
    public class HttpRequestAuthoriserTests
    {
        private IAuthoriser _authoriser;
        private Mock<HttpContextBase> _context;
        private Mock<IConfigurationProvider<PackageInstallationSettings>> _configurationProvider;

        [SetUp]
        public void SetUp()
        {
            _context = HttpContextBuilder.MockHttpContext();
            _configurationProvider = new Mock<IConfigurationProvider<PackageInstallationSettings>>();
            _authoriser = new HttpRequestAuthoriser(_context.Object, _configurationProvider.Object);
        }

        [Test]
        public void IsAllowed_WithoutConfiguration_ReturnsFalse()
        {
            // Arrange
            _configurationProvider
                .Setup(x => x.Settings)
                .Returns(new PackageInstallationSettingsBuilder().Build());

            // Act 
            var isAllowed = _authoriser.IsAllowed();

            // Assert
            Assert.That(isAllowed, Is.False);
        }

        [Test]
        public void IsAllowed_ByLocalRequest_ReturnsTrueIfConfigurationAvalilable()
        {
            // Arrange
            _configurationProvider
                .Setup(x => x.Settings)
                .Returns(new PackageInstallationSettingsBuilder()
                                .WithEnabled()
                                .Build());

            _context.Setup(x => x.Request.IsLocal).Returns(true);

            // Act 
            var isAllowed = _authoriser.IsAllowed();

            // Assert
            Assert.That(isAllowed, Is.True);
        }

        [Test]
        public void IsAllowed_ByRemoteRequest_ReturnsFalseByDefault()
        {
            // Arrange
            _configurationProvider
                .Setup(x => x.Settings)
                .Returns(
                    new PackageInstallationSettingsBuilder().Build());

            // Act 
            var isAllowed = _authoriser.IsAllowed();

            // Assert
            Assert.That(isAllowed, Is.False);
        }

        [Test]
        public void IsAllowed_ByRemoteRequest_ReturnsTrueWhenConfigured()
        {
            // Arrange
            _configurationProvider
                .Setup(x => x.Settings)
                .Returns(new PackageInstallationSettingsBuilder()
                                .WithEnabled()
                                .WithRemoteAccess()
                                .Build());

            // Act 
            var isAllowed = _authoriser.IsAllowed();

            // Assert
            Assert.That(isAllowed, Is.True);
        }

        [Test]
        public void IsAllowed_ByPostRequest_ReturnsFalseByDefault()
        {
            // Arrange
            _configurationProvider
                .Setup(x => x.Settings)
                .Returns(new PackageInstallationSettingsBuilder()
                                .WithEnabled()
                                .Build());

            _context.Setup(x => x.Request.IsLocal).Returns(true);
            _context.Setup(x => x.Request.HttpMethod).Returns("POST");

            // Act 
            var isAllowed = _authoriser.IsAllowed();

            // Assert
            Assert.That(isAllowed, Is.False);
        }

        [Test]
        public void IsAllowed_ByPostRequest_ReturnsTrueWhenConfigured()
        {
            // Arrange
            _configurationProvider
                .Setup(x => x.Settings)
                .Returns(new PackageInstallationSettingsBuilder()
                                .WithEnabled()
                                .WithPackageStreaming()
                                .Build());
            _context.Setup(x => x.Request.IsLocal).Returns(true);
            _context.Setup(x => x.Request.HttpMethod).Returns("POST");

            // Act 
            var isAllowed = _authoriser.IsAllowed();

            // Assert
            Assert.That(isAllowed, Is.True);
        }
    }
}
