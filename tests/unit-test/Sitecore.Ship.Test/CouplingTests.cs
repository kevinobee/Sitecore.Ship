using TestUtils;

using Xunit;

namespace Sitecore.Ship.Test
{
    public class CouplingTests
    {
        [Fact]
        public void Sitecore_Ship_should_not_reference_Sitecore_Kernel()
        {
            // Arrange
            var assemblyType = typeof(Sitecore.Ship.Package.Install.InstallerModule);
            const string unwantedAssemblyName = "Sitecore.Kernel";

            // Act
            var containsReference = assemblyType.AssemblyContainsReferencesTo(unwantedAssemblyName);

            // Assert
            Assert.False(containsReference, $"{unwantedAssemblyName} should not be referenced by Sitecore.Ship");
        }
    }
}