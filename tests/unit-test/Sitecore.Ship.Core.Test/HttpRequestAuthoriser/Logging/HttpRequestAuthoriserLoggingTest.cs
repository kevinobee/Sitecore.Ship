using Moq;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser.Logging
{
    public abstract class HttpRequestAuthoriserLoggingTest : HttpRequestAuthoriserTest
    {
        protected string DiagnosticMessage;

        protected HttpRequestAuthoriserLoggingTest()
        {
            CheckRequests
                .Setup(x => x.IsLocal)
                .Returns(true);

            Logger
                .Setup(x => x.Write(It.IsAny<string>()))
                .Callback<string>(x => DiagnosticMessage = x);
        }
    }
}