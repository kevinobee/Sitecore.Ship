using TestUtils;

using Xunit;

namespace Sitecore.Ship.AspNet.Test
{
    public class CouplingTests
    {
        [Fact]
        public void Sitecore_Ship_AspNet_should_not_reference_Sitecore_Kernel()
        {
            // Arrange
            var assemblyType = typeof(Sitecore.Ship.AspNet.BaseHttpHandler);
            const string unwantedAssemblyName = "Sitecore.Kernel";

            // Act
            var containsReference = assemblyType.AssemblyContainsReferencesTo(unwantedAssemblyName);

            // Assert
            Assert.False(containsReference, string.Format("{0} should not be referenced by Sitecore.Ship.AspNet", unwantedAssemblyName));
        }

        [Fact]
        public void Sitecore_Ship_AspNet_should_not_reference_Nancy()
        {
            // Arrange
            var assemblyType = typeof(Sitecore.Ship.AspNet.BaseHttpHandler);
            const string unwantedAssemblyName = "Nancy";

            // Act
            var containsReference = assemblyType.AssemblyContainsReferencesTo(unwantedAssemblyName);

            // Assert
            Assert.False(containsReference, string.Format("{0} should not be referenced by Sitecore.Ship.AspNet", unwantedAssemblyName));
        }
    }
}