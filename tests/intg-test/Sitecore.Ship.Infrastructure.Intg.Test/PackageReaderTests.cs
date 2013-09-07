using System;
using System.Linq;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Infrastructure.Install;
using Xunit;

namespace Sitecore.Ship.Infrastructure.Intg.Test
{
    public class PackageReaderTests
    {
        private readonly string _testPackagePath;

        public PackageReaderTests()
        {
            _testPackagePath = @"..\..\..\..\acceptance-test\01-package.update";
        }

        [Fact]
        public void Package_reader_implements_package_manifest_repository()
        {
            // Arrange

            // Act
            var reader = new PackageManifestReader();

            // Assert
            Assert.IsAssignableFrom<IPackageManifestRepository>(reader);
        }
        
        [Fact]
        public void Process_should_return_a_package_manifest()
        {
            // Arrange
            var reader = new PackageManifestReader();

            // Act
            var response = reader.GetManifest(_testPackagePath);

            // Assert
            Assert.IsType<PackageManifest>(response);
        }

        [Fact]
        public void Process_should_return_added_item_field_values_for_single_item()
        {
            // Arrange
            var reader = new PackageManifestReader();

            // Act
            var response = reader.GetManifest(_testPackagePath);

            // Assert
            Assert.Equal(1, response.Entries.Count);
            Assert.Equal(new Guid("110d559f-dea5-42ea-9c1c-8a5df7e70ef9"), response.Entries[0].ID);
            Assert.Equal("addeditems/master/sitecore/content/home", response.Entries[0].Path);
        }

        [Fact]
        public void Process_should_return_added_item_field_values_for_multiple_items()
        {
            // Arrange
            const string testPackagePath = @"..\..\..\..\acceptance-test\Sitecore.Mvc.Contrib.1.0.0.130626.update";
            var reader = new PackageManifestReader();

            // Act
            var response = reader.GetManifest(testPackagePath);

            // Assert
            Assert.True(response.Entries.Count > 1);
        }

        [Fact]
        public void Process_should_throw_invalid_operation_exception_if_package_not_found()
        {
            // Arrange
            const string filename = @"..\..\..\acceptance-test\package.update";
            var reader = new PackageManifestReader();
            InvalidOperationException exceptionThrown = null;

            // Act
            try
            {
                reader.GetManifest(filename);
            }
            catch (InvalidOperationException ex)
            {
                exceptionThrown = ex;
            }

            // Assert
            Assert.NotNull(exceptionThrown);
        }

        [Fact]
        public void Process_should_return_added_item_field_values_for_templates()
        {
            // Arrange
            const string testPackagePath = @"..\..\..\..\..\build\sitecore packages\SitecoreShip.zip";
            var reader = new PackageManifestReader();

            // Act
            var response = reader.GetManifest(testPackagePath);

            // Assert
            Assert.True(response.Entries.Count(x => x.Path.ToLower().Contains("master/sitecore/templates")) > 1, "No template items found in package");
        }
    }
}
