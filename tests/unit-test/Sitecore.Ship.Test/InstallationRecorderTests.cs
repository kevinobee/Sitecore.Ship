using System;
using System.Collections.Generic;

using Moq;
using Should;
using Xunit;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Core.Services;

namespace Sitecore.Ship.Test
{
    public class InstallationRecorderTests
    {
        private readonly Mock<IPackageHistoryRepository> _mockPackageHistoryRepository;
        private readonly PackageInstallationSettings _packageInstallationSettings;
        private readonly InstallationRecorder _recorder;
        private readonly DateTime _dateInstalled;

        public InstallationRecorderTests()
        {
            _mockPackageHistoryRepository = new Mock<IPackageHistoryRepository>();

            _packageInstallationSettings = new PackageInstallationSettings
            {
                RecordInstallationHistory = true
            };
            
            _recorder = new InstallationRecorder(_mockPackageHistoryRepository.Object, _packageInstallationSettings);

            _dateInstalled = DateTime.Now;
        }

        [Fact]
        public void Record_install_does_not_install_when_disabled()
        {
            _packageInstallationSettings.RecordInstallationHistory = false;

            _recorder.RecordInstall("01-Description.zip", _dateInstalled);

            _mockPackageHistoryRepository.Verify(x => x.Add(It.IsAny<InstalledPackage>()), Times.Never());
        }
       
        [Fact]
        public void Record_install_parses_packageid_correctly_when_relative_path_provided()
        {
            _recorder.RecordInstall("01-Description.zip", _dateInstalled);

            _mockPackageHistoryRepository.Verify(x => x.Add(It.Is<InstalledPackage>(p => p.PackageId == "01")));
        }

        [Fact]
        public void Record_install_parses_packageid_correctly_when_full_path_provided()
        {
            _recorder.RecordInstall("C:\\aaa\\bbb\\01-Description.zip", _dateInstalled);

            _mockPackageHistoryRepository.Verify(x => x.Add(It.Is<InstalledPackage>(p => p.PackageId == "01")));
        }

        [Fact]
        public void Record_install_parses_description_correctly()
        {
            _recorder.RecordInstall("01-Description.zip", _dateInstalled);

            _mockPackageHistoryRepository.Verify(x => x.Add(It.Is<InstalledPackage>(p => p.Description == "Description")));
        }

        [Fact]
        public void Recorder_returns_highest_version_from_get_latest_when_multiple_entries_exist()
        {
            _mockPackageHistoryRepository.Setup(x => x.GetAll()).Returns(
                new List<InstalledPackage>
                    {
                        new InstalledPackage {PackageId = "01"},
                        new InstalledPackage {PackageId = "02"},
                        new InstalledPackage {PackageId = "03"},
                        
                    });

            var result = _recorder.GetLatestPackage();

            result.PackageId.ShouldEqual("03");
        }
    }
}
