using System.Configuration;

using Sitecore.Ship.Infrastructure.Configuration;

using Should;
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
            var section = (PackageInstallationConfiguration) ConfigurationManager.GetSection("packageInstallation");


            section.ShouldNotBeNull();
        }

        [Fact]
        public void PackageInstallation_enabled_can_be_set_true()
        {
            _section.Enabled.ShouldBeTrue();
        }

        [Fact]
        public void PackageInstallation_allowRemoteAccess_can_be_set_true()
        {
            _section.AllowRemoteAccess.ShouldBeTrue();
        }

        [Fact]
        public void PackageInstallation_allowPackageStreaming_can_be_set_true()
        {
            _section.AllowPackageStreaming.ShouldBeTrue();
        }

        [Fact]
        public void PackageInstallation_whitelist_can_be_read()
        {
            _section.Whitelist.ShouldNotBeNull();
            _section.Whitelist.Count.ShouldEqual(2);
        }

        [Fact]
        public void PackageInstallation_recordInstallationHistory_can_be_set_true()
        {
            _section.RecordInstallationHistory.ShouldBeTrue();
        }
    }
}
