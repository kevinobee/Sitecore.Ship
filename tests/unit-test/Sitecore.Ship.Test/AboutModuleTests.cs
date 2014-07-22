using System;
using Moq;
using Nancy.Testing;
using Nancy.ViewEngines;
using Sitecore.Ship.About;
using Sitecore.Ship.Core.Contracts;
using Xunit;

namespace Sitecore.Ship.Test
{
    public class AboutModuleTests
    {
        private readonly Mock<IAuthoriser> _mockAuthoriser;
        private readonly Browser _browser;

        public AboutModuleTests()
        {
            var assembly = typeof(AboutModule).Assembly;
            ResourceViewLocationProvider
                    .RootNamespaces
                    .Add(assembly, "Sitecore.Ship.About.Views");

            _mockAuthoriser = new Mock<IAuthoriser>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<AboutModule>();
                with.ViewLocationProvider<ResourceViewLocationProvider>();
                with.Dependency(_mockAuthoriser.Object);
            });

            _browser = new Browser(bootstrapper);

            _mockAuthoriser.Setup(x => x.IsAllowed()).Returns(true);
        }

        [Fact]
        public void Should_return_about_page_as_view()
        {
            var response = _browser.Get("/services/about");

            response.Body["h1"].AllShouldContain("Sitecore.Ship", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}