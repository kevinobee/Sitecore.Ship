using Moq;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser.Logging
{
    public abstract class HttpRequestAuthoriserLoggingTest : HttpRequestAuthoriserTest
    {
        protected string DiagnosticMessage;
        protected readonly PackageInstallationSettings PackageInstallationSettings;

        protected HttpRequestAuthoriserLoggingTest()
        {
            PackageInstallationSettings
                = new PackageInstallationSettings
            {
                IsEnabled = true
            };

            ConfigurationProvider
                .Setup(x => x.Settings)
                .Returns(PackageInstallationSettings);

            CheckRequests
                .Setup(x => x.IsLocal)
                .Returns(true);

            Logger
                .Setup(x => x.Write(It.IsAny<string>()))
                .Callback<string>(x => DiagnosticMessage = x);
        }






        // TODO add a test to prevent logging by configuration
    }
}