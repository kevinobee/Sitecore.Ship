using System;
using System.Linq;

using Moq;
using Nancy;
using Nancy.Testing;
using Should;
using Xunit;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Publish;

namespace Sitecore.Ship.Test
{
    public class PublishModuleBehaviour
    {
        private readonly Browser _browser;

        private readonly Mock<IPublishService> _mockPublishService;
        private readonly Mock<IAuthoriser> _mockAuthoriser; 
        
        private PublishParameters _publishParameters;
            
        public PublishModuleBehaviour()
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

            _publishParameters = null;

            _mockPublishService
                .Setup(x => x.Run(It.IsAny<PublishParameters>()))
                .Callback<PublishParameters>(x => _publishParameters = x);
        }

        [Fact]
        public void Should_return_status_bad_request_if_publish_mode_is_unrecognised()
        {
            var response = _browser.Post("/services/publish/short");

            response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void Should_return_status_accepted_when_initiating_a_publish()
        {
            var response = _browser.Post("/services/publish/full");

            response.StatusCode.ShouldEqual(HttpStatusCode.Accepted);
        }

        [Fact]
        public void Should_pass_publish_targets_to_service_if_specified()
        {
            var response = _browser.Post("/services/publish/full", with =>
            {
                with.HttpRequest();
                with.FormValue("source", "master");
                with.FormValue("targets", "target1,target2");
                with.FormValue("languages", "lang1,lang2");
                with.Header("Content-Type", "application/x-www-form-urlencoded");
            });

            _publishParameters.Mode.ShouldEqual("full");
            _publishParameters.Source.ShouldEqual("master");

            _publishParameters.Targets.Count().ShouldEqual(2);
            _publishParameters.Targets.ShouldContain("target1");
            _publishParameters.Targets.ShouldContain("target2");

            _publishParameters.Languages.Count().ShouldEqual(2);
            _publishParameters.Languages.ShouldContain("lang1");
            _publishParameters.Languages.ShouldContain("lang2");

            response.StatusCode.ShouldEqual(HttpStatusCode.Accepted);
        }

        [Fact]
        public void Should_default_publish_source_to_master_if_not_specified()
        {
            var response = _browser.Post("/services/publish/full", with =>
            {
                with.HttpRequest();
                with.FormValue("targets", "target1,target2");
                with.FormValue("languages", "lang1,lang2");
                with.Header("Content-Type", "application/x-www-form-urlencoded");
            });

            _publishParameters.Source.ShouldEqual("master");
            response.StatusCode.ShouldEqual(HttpStatusCode.Accepted);
        }

        [Fact]
        public void Should_default_publish_targets_to_web_if_not_specified()
        {
            var response = _browser.Post("/services/publish/full", with =>
            {
                with.HttpRequest();
                with.FormValue("languages", "lang1,lang2");
                with.Header("Content-Type", "application/x-www-form-urlencoded");
            });

            _publishParameters.Targets.Count().ShouldEqual(1);
            _publishParameters.Targets[0].ShouldEqual("web");
            response.StatusCode.ShouldEqual(HttpStatusCode.Accepted);
        }

        [Fact]
        public void Should_default_publish_languages_to_en_if_not_specified()
        {
            var response = _browser.Post("/services/publish/full", with =>
            {
                with.HttpRequest();
                with.Header("Content-Type", "application/x-www-form-urlencoded");
            });

            _publishParameters.Languages.Count().ShouldEqual(1);
            _publishParameters.Languages[0].ShouldEqual("en");
            response.StatusCode.ShouldEqual(HttpStatusCode.Accepted);
        }

        [Fact]
        public void Should_return_status_unauthorized_when_security_configuration_restricts_access_to_publish()
        {
            _mockAuthoriser.Setup(x => x.IsAllowed()).Returns(false);

            var response = _browser.Post("/services/publish/full");

            response.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public void Should_return_status_unauthorized_when_security_configuration_restricts_access_to_lastcompleted()
        {
            _mockAuthoriser.Setup(x => x.IsAllowed()).Returns(false);

            var response = _browser.Get("/services/publish/lastcompleted");

            response.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public void Should_request_publish_date_of_master_and_web_in_en_when_last_completed_called_with_no_parameters()
        {
            PublishLastCompleted parameters = null;
            
            var expected = DateTime.Now;
            
            _mockPublishService
                .Setup(x => x.GetLastCompletedRun(It.IsAny<PublishLastCompleted>()))
                .Callback<PublishLastCompleted>(x => parameters = x)
                .Returns(expected);

            _browser.Get("/services/publish/lastcompleted");

            parameters.Source.ShouldEqual("master");
            parameters.Target.ShouldEqual("web");
            parameters.Language.ShouldEqual("en");
        }

        [Fact]
        public void Should_request_publish_date_of_source_and_target_in_specified_language_when_last_completed_called_with_parameters()
        {
            PublishLastCompleted parameters = null;

            var expected = DateTime.Now;

            const string sourceDb = "mysource";
            const string targetDb = "myTarget";
            const string language = "mylanguage";

            _mockPublishService
                .Setup(x => x.GetLastCompletedRun(It.IsAny<PublishLastCompleted>()))
                .Callback<PublishLastCompleted>(x => parameters = x)
                .Returns(expected);

            _browser.Get("/services/publish/lastcompleted/{0}/{1}/{2}".Formatted(sourceDb, targetDb,language));

            parameters.Source.ShouldEqual(sourceDb);
            parameters.Target.ShouldEqual(targetDb);
            parameters.Language.ShouldEqual(language);
        }

        [Fact]
        public void Should_return_publish_date_in_JSON_format_in_response_body_when_last_completed_called()
        {
            var expected = DateTime.Now;

            _mockPublishService
                .Setup(x => x.GetLastCompletedRun(It.IsAny<PublishLastCompleted>()))
                .Returns(expected);

            var response = _browser.Get("/services/publish/lastcompleted");

            var date = Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>(response.Body.AsString());

            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            date.ToLongTimeString().ShouldEqual(expected.ToLongTimeString());
        }

        [Fact]
        public void Should_return_http_accepted_when_listofitems_called()
        {
            _mockPublishService.Setup(x => x.Run(It.IsAny<ItemsToPublish>()));

            var response = _browser.Post("/services/publish/listofitems");

            response.StatusCode.ShouldEqual(HttpStatusCode.Accepted);
        }
    }
}