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

        public InstallerModuleTests()
        {
            _mockPackageRepos = new Mock<IPackageRepository>();
            _mockAuthoriser = new Mock<IAuthoriser>();
            _mockTempPackager = new Mock<ITempPackager>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<InstallerModule>();
                with.Dependency(_mockPackageRepos.Object);
                with.Dependency(_mockAuthoriser.Object);
                with.Dependency(_mockTempPackager.Object);
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
            Assert.True(response.Headers["Location"].Contains("/services/package/install/package.update"), "Location Header mismatch");
        }

        [Fact]
        public void Should_return_a_processing_time_header()
        {
            // Arrange
            
            // Act
            var response = _browser.Post("/services/package/install");

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

            // Act
            var response = _browser.Post("/services/package/install/fileupload", with =>
            {
                with.HttpRequest();
                with.MultiPartFormData(multipart);
            });

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.True(response.Headers["Location"].Contains("/services/package/install/fileupload/foo.update"), "Location Header mismatch");
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
    }
}
