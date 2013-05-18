using System;
using Nancy.Testing;
using Nancy.ViewEngines;
using Sitecore.Ship.About;
using Xunit;

namespace Sitecore.Ship.Test
{
    public class AboutModuleTests
    {
        [Fact]
        public void Should_return_about_page_as_view()
        {
            // Arrange
            var assembly = typeof(AboutModule).Assembly;
            ResourceViewLocationProvider
                    .RootNamespaces
                    .Add(assembly, "Sitecore.Ship.About.Views");

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<AboutModule>();
                with.ViewLocationProvider<ResourceViewLocationProvider>();
            });

            var browser = new Browser(bootstrapper);

            // Act
            var response = browser.Get("/services/about");

            // Assert
            response.Body["body"].ShouldExist();
            response.Body["h1"].ShouldContain("Sitecore.Ship", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}