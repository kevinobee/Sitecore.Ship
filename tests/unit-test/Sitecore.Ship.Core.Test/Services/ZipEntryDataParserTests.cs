using System;

using Xunit;

using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Core.Services;

namespace Sitecore.Ship.Core.Test.Services
{
    public class ZipEntryDataParserTests
    {
        [Fact]
        public void Manifest_not_found_should_be_returned_for_meta_fields()
        {
            // Arrange
            const string dataKey = "installer/version";

            // Act
            var manifestEntry = ZipEntryDataParser.GetManifestEntry(dataKey);

            // Assert
            Assert.IsType<PackageManifestEntryNotFound>(manifestEntry);
        }

        [Fact]
        public void Item_key_can_be_parsed_into_path_and_guid()
        {
            // Arrange
            const string dataKey = "addeditems/master/sitecore/templates/user defined/mvc contrib_{2cfb38b7-708a-4f56-8b89-97038b03a22b}";

            // Act
            var manifestEntry = ZipEntryDataParser.GetManifestEntry(dataKey);

            // Assert
            Assert.Equal(new Guid("2cfb38b7-708a-4f56-8b89-97038b03a22b"), manifestEntry.ID);
            Assert.Equal("addeditems/master/sitecore/templates/user defined/mvc contrib", manifestEntry.Path);
        }

        [Fact]
        public void Template_key_can_be_parsed_into_path_and_guid()
        {
            // Arrange
            const string dataKey = "items/master/sitecore/templates/SitecoreShip/InstalledPackage/{C2B9B6C7-5F2F-4638-9A1C-244557BE959E}/en/1/xml";

            // Act
            var manifestEntry = ZipEntryDataParser.GetManifestEntry(dataKey);

            // Assert
            Assert.Equal(new Guid("C2B9B6C7-5F2F-4638-9A1C-244557BE959E"), manifestEntry.ID);
            Assert.Equal("items/master/sitecore/templates/SitecoreShip/InstalledPackage", manifestEntry.Path);
        }

        [Fact]
        public void File_key_can_be_parsed_into_path_and_empty_guid()
        {
            // Arrange
            const string dataKey = "addedfiles/xsl/system/WebEdit/Hidden Rendering.xslt";

            // Act
            var manifestEntry = ZipEntryDataParser.GetManifestEntry(dataKey);

            // Assert
            Assert.Null(manifestEntry.ID);
            Assert.Equal("/xsl/system/WebEdit/Hidden Rendering.xslt", manifestEntry.Path);
        }

        [Fact]
        public void Manifest_not_found_should_be_returned_for_properties()
        {
            // Arrange
            const string dataKey = "properties/files/xsl/system/WebEdit/Hidden Rendering.xslt";

            // Act
            var manifestEntry = ZipEntryDataParser.GetManifestEntry(dataKey);

            // Assert
            Assert.IsType<PackageManifestEntryNotFound>(manifestEntry);
        }
    }
}
