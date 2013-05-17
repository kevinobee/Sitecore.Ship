using System.Collections.Specialized;
using System.Web;
using Moq;
using NUnit.Framework;
using Ship.Web.Update;

namespace Ship.Web.Tests.Update
{
    [TestFixture]
    public class ServerPackageInstallerTests
    {
        private Mock<HttpContextBase> _context;
        private TestableServerPackageFileInstaller _installer;
        private Mock<IPackageRunner> _packageRunner;

        [SetUp]
        public void SetUp()
        {
            _context = HttpContextBuilder.MockHttpContext();
            _packageRunner = new Mock<IPackageRunner>();
            _installer = new TestableServerPackageFileInstaller(_context.Object, _packageRunner.Object);
        }

        [Test]
        public void GetPackageToInstall_WithoutPackageParameter_ReturnsEmptyString()
        {
            // Arrange            
            var queryString = new NameValueCollection();
            _context.Setup(x => x.Request.QueryString).Returns(queryString);

            // Act 
            var packageToInstall = _installer.GetPackageToInstall();

            // Assert
            Assert.That(packageToInstall, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetPackageToInstall_WithPackageParameter_ReturnsValue()
        {
            // Arrange
            var queryString = new NameValueCollection() {{"package", @"c:\package.update"}};
            _context.Setup(x => x.Request.QueryString).Returns(queryString);

            // Act 
            var packageToInstall = _installer.GetPackageToInstall();

            // Assert
            Assert.That(packageToInstall, Is.EqualTo(@"c:\package.update"));
        }
    }

    class TestableServerPackageFileInstaller : ServerPackageFileInstaller
    {
        public TestableServerPackageFileInstaller(HttpContextBase context, IPackageRunner packageRunner)
            : base(context, packageRunner)
        {
        }

        public new string GetPackageToInstall()
        {
            return base.GetPackageToInstall();
        }
    }
}
