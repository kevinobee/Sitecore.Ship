using Moq;
using Xunit;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser.Logging
{
    public class HttpRequestAuthoriserLoggingBehaviour : HttpRequestAuthoriserLoggingTest
    {
        public HttpRequestAuthoriserLoggingBehaviour()
        {
            RequestAuthoriser.IsAllowed();            
        }

        [Fact]
        public void Should_not_log_authorisation_succeeding()
        {
            Logger.Verify(x => x.Write(It.IsAny<string>()), Times.Never());
        }
    }
}