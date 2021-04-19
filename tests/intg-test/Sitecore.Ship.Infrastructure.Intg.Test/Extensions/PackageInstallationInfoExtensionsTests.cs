using Should;
using Sitecore.Ship.Infrastructure.Extensions;
using Sitecore.Update;
using Xunit;

#if SITECORE822 
using Sitecore.Update.Utils;
#endif


namespace Sitecore.Ship.Infrastructure.Intg.Test.Extensions
{
    public class PackageInstallationInfoExtensionsTests
    {
        [Fact]
        public void SetProcessingMode_sets_Mode_All()
        {
            //Arrange
            var sut = new PackageInstallationInfo();

            //Act
            sut.SetProcessingMode();

            //Assert
#if SITECORE822
            sut.ProcessingMode.ShouldEqual(ProcessingMode.All);
#endif
        }
    }
}
