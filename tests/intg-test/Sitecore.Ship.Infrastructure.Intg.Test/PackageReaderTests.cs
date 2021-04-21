using System;
using System.Globalization;
using System.Linq;
using Should;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Infrastructure.Install;
using Xunit;

namespace Sitecore.Ship.Infrastructure.Intg.Test
{
    public class PackageReaderTests
    {
        private const string TestPackagePath = @"test-packages\01-package.update";
        
        private readonly PackageManifestReader _reader;

        public PackageReaderTests()
        {
            _reader = new PackageManifestReader();
        }

        [Fact]
        public void PackageManifestReader_Implements_IPackageManifestRepository()
        {
            _reader.ShouldImplement<IPackageManifestRepository>();
        }
        
        [Theory]
        [InlineData(TestPackagePath)]
        public void PackageManifestReader_GetManifest_ReturnsPackageManifest(string testPackagePath)
        {
            var response = _reader.GetManifest(testPackagePath);

            response.ShouldBeType<PackageManifest>();
        }
        
        [Theory]
        [InlineData(TestPackagePath)]
        public void PackageManifestReader_GetManifest_ReturnsManifestContainingSingleItem(string testPackagePath)
        {
            var response = _reader.GetManifest(testPackagePath);

            response.Entries.Count.ShouldEqual(1);
            response.Entries[0].ID.ShouldEqual(new Guid("110d559f-dea5-42ea-9c1c-8a5df7e70ef9"));
            response.Entries[0].Path.ShouldEqual("addeditems/master/sitecore/content/home");
        }

        [Theory]
        [InlineData(@"test-packages\Sitecore.Mvc.Contrib.1.0.0.130626.update")]
        public void PackageManifestReader_GetManifest_ReturnsManifestContainingMultpipleItems(string testPackagePath)
        {
            var response = _reader.GetManifest(testPackagePath);

            response.Entries.Count.ShouldBeGreaterThan(1);
        }

        [Theory]
        [InlineData(@"test-packages\package.update")]
        public void PackageManifestReader_GetManifest_ThrowsInvalidOperationExceptionIfPackageNotFound(string testPackagePath)
        {
            var exception = Assert.Throws<InvalidOperationException>(() => _reader.GetManifest(testPackagePath));

            exception.ShouldNotBeNull();
        }

        [Theory]
        [InlineData(@"test-packages\SitecoreShip.zip")]
        public void PackageManifestReader_GetManifest_ContainsItemFieldsForTemplates(string testPackagePath)
        {
            var response = _reader.GetManifest(testPackagePath);

            response
                .Entries
                .Count(x => x.Path.ToLower(CultureInfo.CurrentCulture).Contains("master/sitecore/templates"))
                .ShouldBeGreaterThan(1);
        }
    }
}
