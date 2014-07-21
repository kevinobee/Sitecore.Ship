using Moq;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser
{
    public abstract class HttpRequestAuthoriserTest
    {
        protected readonly Mock<ICheckRequests> CheckRequests;
        protected readonly Mock<IConfigurationProvider<PackageInstallationSettings>> ConfigurationProvider; 
        protected readonly Core.HttpRequestAuthoriser RequestAuthoriser;
        protected readonly Mock<ILog> Logger;

        protected HttpRequestAuthoriserTest()
        {
            CheckRequests = new Mock<ICheckRequests>();
            ConfigurationProvider = new Mock<IConfigurationProvider<PackageInstallationSettings>>(); 
            Logger = new Mock<ILog>();

            RequestAuthoriser = new Core.HttpRequestAuthoriser(CheckRequests.Object, ConfigurationProvider.Object, Logger.Object);            
        }
    }
}