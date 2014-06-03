using System;
using System.Collections.Generic;

using Moq;
using Xunit;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Core.Services;

namespace Sitecore.Ship.Test
{
    public class InstallationRecorderTests
    {
        private readonly Mock<IPackageHistoryRepository> _mockPackageHistoryRepository;
        private readonly Mock<IConfigurationProvider<PackageInstallationSettings>> _configurationProvider;

        public InstallationRecorderTests()
        {
            _mockPackageHistoryRepository = new Mock<IPackageHistoryRepository>();
            _configurationProvider = new Mock<IConfigurationProvider<PackageInstallationSettings>>();
            var settingsObject = new PackageInstallationSettings { RecordInstallationHistory = true };
            _configurationProvider.Setup(x => x.Settings).Returns(settingsObject);
        }

        [Fact]
        public void Record_install_does_not_install_when_disabled()
        {
            // Arrange
            var settingsObject = new PackageInstallationSettings {RecordInstallationHistory = false};

            _configurationProvider.Setup(x => x.Settings).Returns(settingsObject);
            var recorder = new InstallationRecorder(_mockPackageHistoryRepository.Object, _configurationProvider.Object);
            var dateInstalled = DateTime.Now;

            // Act
            recorder.RecordInstall("01-Description.zip", dateInstalled);

            // Assert
            _mockPackageHistoryRepository.Verify(x => x.Add(It.IsAny<InstalledPackage>()), Times.Never());
        }
       
        [Fact]
        public void Record_install_parses_packageid_correctly_when_relative_path_provided()
        {
            // Arrange
            var recorder = new InstallationRecorder(_mockPackageHistoryRepository.Object, _configurationProvider.Object);
            var dateInstalled = DateTime.Now;

            // Act
            recorder.RecordInstall("01-Description.zip", dateInstalled);

            // Assert
            _mockPackageHistoryRepository.Verify(x => x.Add(It.Is<InstalledPackage>(p => p.PackageId == "01")));
        }

        [Fact]
        public void Record_install_parses_packageid_correctly_when_full_path_provided()
        {
            // Arrange
            var recorder = new InstallationRecorder(_mockPackageHistoryRepository.Object, _configurationProvider.Object);
            var dateInstalled = DateTime.Now;

            // Act
            recorder.RecordInstall("C:\\aaa\\bbb\\01-Description.zip", dateInstalled);

            // Assert
            _mockPackageHistoryRepository.Verify(x => x.Add(It.Is<InstalledPackage>(p => p.PackageId == "01")));
        }

        [Fact]
        public void Record_install_parses_description_correctly()
        {
            // Arrange
            var recorder = new InstallationRecorder(_mockPackageHistoryRepository.Object, _configurationProvider.Object);
            var dateInstalled = DateTime.Now;

            // Act
            recorder.RecordInstall("01-Description.zip", dateInstalled);

            // Assert
            _mockPackageHistoryRepository.Verify(x => x.Add(It.Is<InstalledPackage>(p => p.Description == "Description")));
        }

        [Fact]
        public void Recorder_returns_highest_version_from_get_latest_when_multiple_entries_exist()
        {
            // Arrange
            _mockPackageHistoryRepository.Setup(x => x.GetAll()).Returns(
                new List<InstalledPackage>
                    {
                        new InstalledPackage {PackageId = "01"},
                        new InstalledPackage {PackageId = "02"},
                        new InstalledPackage {PackageId = "03"},
                        
                    });

            var recorder = new InstallationRecorder(_mockPackageHistoryRepository.Object, _configurationProvider.Object);

            var expected = new InstalledPackage {PackageId = "03"};

            // Act
            var result = recorder.GetLatestPackage();

            // Assert
            Assert.Same(result.PackageId, expected.PackageId);
        }
    }
}
