using System.Configuration;
using Sitecore.Ship.Infrastructure.Configuration;
using Xunit;

namespace Sitecore.Ship.Infrastructure.Intg.Test
{
    public class PackageInstallationConfigurationTests
    {
        private readonly PackageInstallationConfiguration _section;

        public PackageInstallationConfigurationTests()
        {
            _section = (PackageInstallationConfiguration)ConfigurationManager.GetSection("packageInstallation");
        }

        [Fact]
        public void PackageInstallation_section_can_be_loaded()
        {
            // Arrange

            // Act
            var section = (PackageInstallationConfiguration) ConfigurationManager.GetSection("packageInstallation");

            // Assert
            Assert.NotNull(section);
        }

        [Fact]
        public void PackageInstallation_enabled_can_be_set_true()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(_section.Enabled);
        }

        [Fact]
        public void PackageInstallation_allowRemoteAccess_can_be_set_true()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(_section.AllowRemoteAccess);
        }

        [Fact]
        public void PackageInstallation_allowPackageStreaming_can_be_set_true()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(_section.AllowPackageStreaming);
        }


        [Fact]
        public void PackageInstallation_whitelist_can_be_read()
        {
            // Arrange

            // Act

            // Assert
            Assert.NotNull(_section.Whitelist);
            Assert.Equal(2, _section.Whitelist.Count);
        }

        // TODO add support for recordInstallationHistory="false" 
    }
}
