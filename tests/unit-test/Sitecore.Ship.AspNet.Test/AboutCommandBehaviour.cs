using Moq;
using System.Web;
using Xunit;
using System;
using Should;

namespace Sitecore.Ship.AspNet.Test
{
    public class AboutCommandBehaviour
    {
        private Mock<HttpResponseBase> _response;
        private CommandHandler _sut;

        public AboutCommandBehaviour()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            _response = new Mock<HttpResponseBase>();

            context.Setup(x => x.Request).Returns(request.Object);
            context.Setup(x => x.Response).Returns(_response.Object);

            request.Setup(x => x.Url).Returns(new Uri("http://sitecore.ship/services/about"));
            _response.SetupAllProperties();

            _sut = new AboutCommand();

            _sut.HandleRequest(context.Object);
        }

        [Fact]
        public void ReturnsContentTypeTextPlain()
        {
            _response.Object
                .ContentType
                .ShouldEqual("text/plain");
        }
    }
}
