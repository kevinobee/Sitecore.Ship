using System.IO;
using Moq;
using Nancy;
using Nancy.Testing;
using Xunit;

using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Package.Install;

namespace Sitecore.Ship.Test
{
    public class InstallerModuleTests
    {
        private readonly Browser _browser;

        private readonly Mock<IPackageRepository> _mockPackageRepos;
        private readonly Mock<IAuthoriser> _mockAuthoriser;
        private readonly Mock<ITempPackager> _mockTempPackager;
        private readonly Mock<IInstallationRecorder> _mockInstallationRecorder;

        public InstallerModuleTests()
        {
            _mockPackageRepos = new Mock<IPackageRepository>();
            _mockAuthoriser = new Mock<IAuthoriser>();
            _mockTempPackager = new Mock<ITempPackager>();
            _mockInstallationRecorder = new Mock<IInstallationRecorder>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<InstallerModule>();
                with.Dependency(_mockPackageRepos.Object);
                with.Dependency(_mockAuthoriser.Object);
                with.Dependency(_mockTempPackager.Object);
                with.Dependency(_mockInstallationRecorder.Object);
            });

            _browser = new Browser(bootstrapper);

            _mockAuthoriser.Setup(x => x.IsAllowed()).Returns(true);
        }

        [Fact]
        public void Should_return_status_created_when_installing_a_package_by_path()
        {
            // Arrange

            // Act
            var response = _browser.Post("/services/package/install", with =>
                                                               {
                                                                   with.HttpRequest();
                                                                   with.FormValue("path", @"d:\package.update");
                                                                   with.Header("Content-Type", "application/x-www-form-urlencoded");
                                                               });

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("application/x-www-form-urlencoded", response.Context.Request.Headers.ContentType);
            Assert.Equal("/services/package/latestversion", response.Headers["Location"]);
        }

        [Fact]
        public void Should_return_a_processing_time_header()
        {
            // Arrange

            // Act
            var response = _browser.Post("/services/package/install", with => with.HttpRequest());

            // Assert
            Assert.NotNull(response.Headers["x-processing-time"]);
        }

        [Fact]
        public void Should_return_status_not_found_when_package_path_is_invalid()
        {
            // Arrange
            _mockPackageRepos.Setup(x => x.AddPackage(It.IsAny<InstallPackage>())).Throws(new NotFoundException());

            // Act
            var response = _browser.Post("/services/package/install", with =>
            {
                with.HttpRequest();
                with.FormValue("path", @"y:\foo.update");
                with.Header("Content-Type", "application/x-www-form-urlencoded");
            });

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void Should_return_status_unauthorized_when_security_configuration_restricts_access_to_install()
        {
            // Arrange
            _mockPackageRepos.Setup(x => x.AddPackage(It.IsAny<InstallPackage>())).Throws(new NotFoundException());

            _mockAuthoriser.Setup(x => x.IsAllowed()).Returns(false);

            // Act
            var response = _browser.Post("/services/package/install", with =>
            {
                with.HttpRequest();
                with.FormValue("path", @"y:\foo.update");
                with.Header("Content-Type", "application/x-www-form-urlencoded");
            });

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public void Should_return_status_created_when_installing_a_package_by_file_upload()
        {
            // Arrange
            var stream = CreateFakeFileStream("This is the contents of a file");
            var multipart = new BrowserContextMultipartFormData(x => x.AddFile("foo", "foo.update", "text/plain", stream));

            _mockTempPackager.Setup(x => x.GetPackageToInstall(It.IsAny<Stream>())).Returns("foo.update");

            _mockPackageRepos.Setup(x => x.AddPackage(It.IsAny<InstallPackage>())).Returns(new PackageManifest());

            // Act
            var response = _browser.Post("/services/package/install/fileupload", with =>
            {
                with.HttpRequest();
                with.MultiPartFormData(multipart);
            });

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("/services/package/latestversion", response.Headers["Location"]);
        }

        private static Stream CreateFakeFileStream(string thisIsTheContentsOfAFile)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(thisIsTheContentsOfAFile);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [Fact]
        public void Should_return_status_no_content_when_package_recording_is_disabled()
        {
            _mockInstallationRecorder.Setup(x => x.GetLatestPackage()).Returns(new InstalledPackageNotFound());

            // Act
            var response = _browser.Get("/services/package/latestversion", with => with.HttpRequest());

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public void Should_return_valid_json_when_accessing_latestversion()
        {
            _mockInstallationRecorder.Setup(x => x.GetLatestPackage()).Returns(new InstalledPackage());

            // Act
            var response = _browser.Get("/services/package/latestversion", with => with.HttpRequest());

            // Assert
            var installedPackage = Newtonsoft.Json.JsonConvert.DeserializeObject<InstalledPackage>(response.Body.AsString());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(installedPackage);
        }

        [Fact]
        public void Should_return_json_containing_installed_package_details()
        {
            // Arrange
            var stream = CreateFakeFileStream("This is the contents of a file");
            var multipart = new BrowserContextMultipartFormData(x => x.AddFile("foo", "foo.update", "text/plain", stream));

            _mockTempPackager.Setup(x => x.GetPackageToInstall(It.IsAny<Stream>())).Returns("foo.update");

            _mockPackageRepos.Setup(x => x.AddPackage(It.IsAny<InstallPackage>())).Returns(new PackageManifest());

            // Act
            var response = _browser.Post("/services/package/install/fileupload", with =>
            {
                with.HttpRequest();
                with.MultiPartFormData(multipart);
            });

            //Assert
            var manifest = Newtonsoft.Json.JsonConvert.DeserializeObject<PackageManifest>(response.Body.AsString());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(manifest.Entries);
        }
    }
}