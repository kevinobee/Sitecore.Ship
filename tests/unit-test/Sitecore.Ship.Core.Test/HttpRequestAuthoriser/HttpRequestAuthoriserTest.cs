using Moq;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser
{
    public abstract class HttpRequestAuthoriserTest
    {
        protected readonly Mock<ICheckRequests> CheckRequests;
        protected readonly Core.HttpRequestAuthoriser RequestAuthoriser;
        protected readonly Mock<ILog> Logger;
        protected PackageInstallationSettings PackageInstallationSettings;

        protected HttpRequestAuthoriserTest()
        {
            CheckRequests = new Mock<ICheckRequests>();

            PackageInstallationSettings = new PackageInstallationSettings
            {
                IsEnabled = true
            };

            Logger = new Mock<ILog>();

            RequestAuthoriser = new Core.HttpRequestAuthoriser(CheckRequests.Object, PackageInstallationSettings, Logger.Object);            
        }
    }
}