using System.Collections.Generic;

using Moq;
using Xunit;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Core.Services;


namespace Sitecore.Ship.Core.Test.Services
{
    public class InstallationRecorderTests
    {
        private readonly InstallationRecorder _recorder;
        private readonly Mock<IPackageHistoryRepository> _packageHistoryRepository;
        private readonly Mock<IConfigurationProvider<PackageInstallationSettings>> _configurationProvider;

        public InstallationRecorderTests()
        {
            _packageHistoryRepository = new Mock<IPackageHistoryRepository>();
            _configurationProvider = new Mock<IConfigurationProvider<PackageInstallationSettings>>();

            _recorder = new InstallationRecorder(_packageHistoryRepository.Object, _configurationProvider.Object);
        }

        [Fact]
        public void Should_return_installed_package_not_found_when_package_recording_is_disabled()
        {
            _configurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings { RecordInstallationHistory = false});

            // Act
            var response = _recorder.GetLatestPackage();

            // Assert
            Assert.IsType<InstalledPackageNotFound>(response);
        }

        [Fact]
        public void Should_return_installed_package_not_found_when_no_package_installations_are_recorded()
        {
            _configurationProvider.Setup(x => x.Settings).Returns(new PackageInstallationSettings { RecordInstallationHistory = true });

            _packageHistoryRepository.Setup(x => x.GetAll()).Returns(new List<InstalledPackage>());

            // Act
            var response = _recorder.GetLatestPackage();

            // Assert
            Assert.IsType<InstalledPackageNotFound>(response);
        }
    }
}