using Moq;
using Nancy.Testing;
using Sitecore.Ship.Core.Contracts;
using Xunit;

namespace Sitecore.Ship.Test
{
    public class AuthorizationTests
    {
        private readonly Mock<IAuthoriser> _mockAuthoriser;
        private readonly Browser _browser;
        private readonly Mock<IDepencency> _mockDependency;

        public AuthorizationTests()
        {
            _mockAuthoriser = new Mock<IAuthoriser>();
            _mockDependency = new Mock<IDepencency>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                //todo: mock module and Verify if EmptyMethod() called instead of using Dependency
                with.Module<TestModule>();
                //dependency is used for verification of method execution
                with.Dependency(_mockAuthoriser.Object);
                with.Dependency(_mockDependency.Object);
            });

            _browser = new Browser(bootstrapper);
        }

        [Fact]
        public void method_executed_when_request_authorized()
        {
            //Arrange
            _mockAuthoriser.Setup(x => x.IsAllowed()).Returns(true);

            //Act
            _browser.Get("/services/test/empty");

            //Assert
            _mockDependency.Verify(t => t.DoSomething(), Times.Once);
        }

        [Fact]
        public void method_not_executed_when_request_not_authorized()
        {
            //Arrange
            _mockAuthoriser.Setup(x => x.IsAllowed()).Returns(false);

            //Act
            _browser.Get("/services/test/empty");

            //Assert
            _mockDependency.Verify(t => t.DoSomething(), Times.Never);
        }
    }
}