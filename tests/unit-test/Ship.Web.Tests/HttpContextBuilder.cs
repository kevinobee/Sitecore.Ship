using System.Web;
using Moq;

namespace Ship.Web.Tests
{
    public static class HttpContextBuilder
    {
        public static Mock<HttpContextBase> MockHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            context.SetupAllProperties();
            request.SetupAllProperties();
            response.SetupAllProperties();
            session.SetupAllProperties();
            server.SetupAllProperties();
            return context;
        }

        public static HttpContextBase FakeHttpContext()
        {
            return MockHttpContext().Object;
        }
    }
}