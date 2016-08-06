using Moq;
using System.Web;
using Xunit;
using System;
using Should;
using Sitecore.Ship.Core.Contracts;
using System.IO;

namespace Sitecore.Ship.AspNet.Test
{
    public class BaseHttpHandlerUnauthorisedTests
    {
        private HttpResponse _response;
        private SampleHandler _sut;

        public BaseHttpHandlerUnauthorisedTests()
        {
            var authoriser = new Mock<IAuthoriser>();
            authoriser.Setup(x => x.IsAllowed()).Returns(false);

            var request = new HttpRequest("", "http://tempuri.org", "");
            _response = new HttpResponse(new StringWriter());

            var context = new HttpContext(request, _response);

            _sut = new SampleHandler(authoriser.Object);
            _sut.ProcessRequest(context);
        }

        [Fact]
        public void ReturnsStatusCodeUnauthorized()
        {
            _response.StatusCode.ShouldEqual(401);
        }

        [Fact]
        public void DoesNotCallProcessRequest()
        {
            _sut.DidRun.ShouldBeFalse();
        }

        class SampleHandler : BaseHttpHandler
        {
            public SampleHandler(IAuthoriser authoriser)
                : base(authoriser)
            {

            }

            public override void ProcessRequest(HttpContextBase context)
            {
                DidRun = true;
                context.Response.StatusDescription = "Failed";
            }

            public bool DidRun;
        }
    }

}
