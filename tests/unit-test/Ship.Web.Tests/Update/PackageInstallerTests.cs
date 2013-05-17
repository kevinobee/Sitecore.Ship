using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Moq;
using NUnit.Framework;
using Ship.Web.Update;

namespace Ship.Web.Tests.Update
{
    [TestFixture]
    public class PackageInstallerTests
    {
        private Mock<IAuthoriser> _authoriser;
        private Mock<HttpContextBase> _context;
        private PackageInstaller _packageInstaller;
        private Mock<IPackageInstaller> _specificPackageInstaller;

        [SetUp]
        public void SetUp()
        {
            _context = HttpContextBuilder.MockHttpContext();
            _authoriser = new Mock<IAuthoriser>();
            _specificPackageInstaller = new Mock<IPackageInstaller>();

            IDictionary<string, Action<HttpContextBase>> httpMethodActions = new Dictionary<string, Action<HttpContextBase>>
            {
                { "GET",  c => _specificPackageInstaller.Object.Execute() },
            };

            _packageInstaller = new PackageInstaller(_context.Object, _authoriser.Object, httpMethodActions);
        }

        [Test]
        public void Execute_WithoutAuthorization_IsForbidden()
        {
            // Arrange
            _authoriser.Setup(x => x.IsAllowed()).Returns(false);

            // Act 
            _packageInstaller.Execute();

            // Assert
            Assert.That(_context.Object.Response.StatusCode, Is.EqualTo((int) HttpStatusCode.Forbidden));
        }

        [Test]
        public void Execute_NotDefinedHttpMethod_IsNotAllowed()
        {
            // Arrange
            _authoriser.Setup(x => x.IsAllowed()).Returns(true);
            _context.Setup(x => x.Request.HttpMethod).Returns("PUT");

            // Act 
            _packageInstaller.Execute();

            // Assert
            Assert.That(_context.Object.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.MethodNotAllowed));
        }

        [Test]
        public void Execute_DefinedHttpMethod_Invokes_SpecificPackageInstaller()
        {
            // Arrange
            _authoriser.Setup(x => x.IsAllowed()).Returns(true);
            _context.Setup(x => x.Request.HttpMethod).Returns("GET");

            // Act 
            _packageInstaller.Execute();

            // Assert
            _specificPackageInstaller.Verify(x => x.Execute(), Times.Once());
        }
    }
}
