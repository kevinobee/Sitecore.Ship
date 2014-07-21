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
        private readonly PackageInstallationSettings _packageInstallationSettings;

        public InstallationRecorderTests()
        {
            _packageHistoryRepository = new Mock<IPackageHistoryRepository>();
            _packageInstallationSettings = new PackageInstallationSettings();

            _recorder = new InstallationRecorder(_packageHistoryRepository.Object, _packageInstallationSettings);
        }

        [Fact]
        public void Should_return_installed_package_not_found_when_package_recording_is_disabled()
        {
            _packageInstallationSettings.RecordInstallationHistory = false;

            // Act
            var response = _recorder.GetLatestPackage();

            // Assert
            Assert.IsType<InstalledPackageNotFound>(response);
        }

        [Fact]
        public void Should_return_installed_package_not_found_when_no_package_installations_are_recorded()
        {
            _packageInstallationSettings.RecordInstallationHistory = true;

            _packageHistoryRepository.Setup(x => x.GetAll()).Returns(new List<InstalledPackage>());

            // Act
            var response = _recorder.GetLatestPackage();

            // Assert
            Assert.IsType<InstalledPackageNotFound>(response);
        }
    }
}