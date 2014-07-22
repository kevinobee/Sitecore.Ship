using System;
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
        private readonly string _testPackagePath;
        private readonly PackageManifestReader _reader;

        public PackageReaderTests()
        {
            _testPackagePath = @"..\..\..\..\acceptance-test\01-package.update";

            _reader = new PackageManifestReader();
        }

        [Fact]
        public void Package_reader_implements_package_manifest_repository()
        {
            _reader.ShouldImplement<IPackageManifestRepository>();
        }
        
        [Fact]
        public void Process_should_return_a_package_manifest()
        {
            var response = _reader.GetManifest(_testPackagePath);

            response.ShouldBeType<PackageManifest>();
        }

        [Fact]
        public void Process_should_return_added_item_field_values_for_single_item()
        {
            var response = _reader.GetManifest(_testPackagePath);

            response.Entries.Count.ShouldEqual(1);
            response.Entries[0].ID.ShouldEqual(new Guid("110d559f-dea5-42ea-9c1c-8a5df7e70ef9"));
            response.Entries[0].Path.ShouldEqual("addeditems/master/sitecore/content/home");
        }

        [Fact]
        public void Process_should_return_added_item_field_values_for_multiple_items()
        {
            const string testPackagePath = @"..\..\..\..\acceptance-test\Sitecore.Mvc.Contrib.1.0.0.130626.update";

            var response = _reader.GetManifest(testPackagePath);

            response.Entries.Count.ShouldBeGreaterThan(1);
        }

        [Fact]
        public void Process_should_throw_invalid_operation_exception_if_package_not_found()
        {
            const string filename = @"..\..\..\acceptance-test\package.update";

            InvalidOperationException exceptionThrown = null;

            try
            {
                _reader.GetManifest(filename);
            }
            catch (InvalidOperationException ex)
            {
                exceptionThrown = ex;
            }

            exceptionThrown.ShouldNotBeNull();
        }

        [Fact]
        public void Process_should_return_added_item_field_values_for_templates()
        {
            const string testPackagePath = @"..\..\..\..\..\build\sitecore packages\SitecoreShip.zip";

            var response = _reader.GetManifest(testPackagePath);

            response.Entries.Count(x => x.Path.ToLower().Contains("master/sitecore/templates")).ShouldBeGreaterThan(1);
        }
    }
}
