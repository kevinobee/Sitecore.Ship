using System.Linq;
using Moq;
using Nancy;
using Nancy.Testing;
using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Publish;
using Xunit;

namespace Sitecore.Ship.Test
{
    public class PublishModuleTests
    {
        private readonly Browser _browser;

        private readonly Mock<IPublishService> _mockPublishService;
        private readonly Mock<IAuthoriser> _mockAuthoriser;

        public PublishModuleTests()
        {
            _mockPublishService = new Mock<IPublishService>();

            _mockAuthoriser = new Mock<IAuthoriser>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<PublishModule>();
                with.Dependency(_mockPublishService.Object);
                with.Dependency(_mockAuthoriser.Object);
            });

            _browser = new Browser(bootstrapper);

            _mockAuthoriser.Setup(x => x.IsAllowed()).Returns(true);
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

        [Fact]
        public void Should_pass_publish_targets_to_service_if_specified()
        {
            // Arrange
            PublishParameters publishParameters = null;
            _mockPublishService.Setup(x => x.Run(It.IsAny<PublishParameters>())).Callback<PublishParameters>(x => publishParameters = x);

            // Act
            var response = _browser.Post("/services/publish/full", with =>
            {
                with.HttpRequest();
                with.FormValue("source", "master");
                with.FormValue("targets", "target1,target2");
                with.FormValue("languages", "lang1,lang2");
                with.Header("Content-Type", "application/x-www-form-urlencoded");
            });

            // Assert
            Assert.Equal("full", publishParameters.Mode);
            Assert.Equal("master", publishParameters.Source);
            Assert.Equal(2, publishParameters.Targets.Count());
            Assert.True(publishParameters.Targets.Contains("target1"));
            Assert.True(publishParameters.Targets.Contains("target2"));
            Assert.Equal(2, publishParameters.Languages.Count());
            Assert.True(publishParameters.Languages.Contains("lang1"));
            Assert.True(publishParameters.Languages.Contains("lang2"));
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public void Should_default_publish_source_to_master_if_not_specified()
        {
            // Arrange
            PublishParameters publishParameters = null;
            _mockPublishService.Setup(x => x.Run(It.IsAny<PublishParameters>())).Callback<PublishParameters>(x => publishParameters = x);

            // Act
            var response = _browser.Post("/services/publish/full", with =>
            {
                with.HttpRequest();
                with.FormValue("targets", "target1,target2");
                with.FormValue("languages", "lang1,lang2");
                with.Header("Content-Type", "application/x-www-form-urlencoded");
            });

            // Assert
            Assert.Equal("master", publishParameters.Source);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }


        [Fact]
        public void Should_default_publish_targets_to_web_if_not_specified()
        {
            // Arrange
            PublishParameters publishParameters = null;
            _mockPublishService.Setup(x => x.Run(It.IsAny<PublishParameters>())).Callback<PublishParameters>(x => publishParameters = x);

            // Act
            var response = _browser.Post("/services/publish/full", with =>
            {
                with.HttpRequest();
                with.FormValue("languages", "lang1,lang2");
                with.Header("Content-Type", "application/x-www-form-urlencoded");
            });

            // Assert
            Assert.Equal(1, publishParameters.Targets.Count());
            Assert.Equal("web", publishParameters.Targets[0]);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public void Should_default_publish_languages_to_en_if_not_specified()
        {
            // Arrange
            PublishParameters publishParameters = null;
            _mockPublishService.Setup(x => x.Run(It.IsAny<PublishParameters>())).Callback<PublishParameters>(x => publishParameters = x);

            // Act
            var response = _browser.Post("/services/publish/full", with =>
            {
                with.HttpRequest();
                with.Header("Content-Type", "application/x-www-form-urlencoded");
            });

            // Assert
            Assert.Equal(1, publishParameters.Languages.Count());
            Assert.Equal("en", publishParameters.Languages[0]);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public void Should_return_status_unauthorized_when_security_configuration_restricts_access_to_publish()
        {
            // Arrange
            _mockAuthoriser.Setup(x => x.IsAllowed()).Returns(false);

            // Act
            var response = _browser.Post("/services/publish/full");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
