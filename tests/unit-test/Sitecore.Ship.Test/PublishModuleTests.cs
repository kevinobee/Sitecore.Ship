using Moq;
using Nancy;
using Nancy.Testing;
using Sitecore.Ship.Core;
using Sitecore.Ship.Publish;
using Xunit;

namespace Sitecore.Ship.Test
{
    public class PublishModuleTests
    {
        private readonly Browser _browser;

        private readonly Mock<IPublishService> _mockPublishService;

        public PublishModuleTests()
        {
            _mockPublishService = new Mock<IPublishService>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<PublishModule>();
                with.Dependency(_mockPublishService.Object);
            });

            _browser = new Browser(bootstrapper);            
        }

        [Fact]
        public void Should_return_status_bad_request_if_publish_mode_is_unrecognised()
        {
            // Arrange

            // Act
            var response = _browser.Post("/services/publish/short");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public void Should_return_status_accepted_when_initiating_a_publish()
        {
            // Arrange

            // Act
            var response = _browser.Post("/services/publish/full");

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }
    }
}
