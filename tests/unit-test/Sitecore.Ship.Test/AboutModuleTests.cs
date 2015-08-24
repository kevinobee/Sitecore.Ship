using System;

using Nancy.Testing;
using Nancy.ViewEngines;
using Sitecore.Ship.About;

using Nancy;
using Should;
using Sitecore.Ship.Core.Contracts;
using Xunit;

namespace Sitecore.Ship.Test
{
	public class AboutModuleTests
    {
		private readonly Browser browser;

		public AboutModuleTests()
        {
			var assembly = typeof(DefaultBootstrapper).Assembly;

			const string sitecoreShipAboutViews = "Sitecore.Ship.About.Views";

			if (!ResourceViewLocationProvider.RootNamespaces.ContainsKey(assembly))
			{
				ResourceViewLocationProvider
					.RootNamespaces
					.Add(assembly, sitecoreShipAboutViews);
			}

			var bootstrapper = new ConfigurableBootstrapper(with =>
			{
				with.Module<AboutModule>();
				with.ViewLocationProvider(new ResourceViewLocationProvider());
				with.Dependency<IAuthoriser>(new FakeRequestAuthoriser());
			}); 
			
			browser = new Browser(bootstrapper);
        }

        [Fact]
        public void About_page_is_returned_successfully()
        {
			browser
				.Get("/services/about")
				.StatusCode
				.ShouldEqual(HttpStatusCode.OK);
        }

		[Fact]
		public void About_page_is_returned_as_a_view()
		{
			browser
				.Get("/services/about")
				.Body["h1"]
				.AllShouldContain("Sitecore.Ship", StringComparison.InvariantCultureIgnoreCase);
		}
    }

	public class FakeRequestAuthoriser : IAuthoriser
	{
		public bool IsAllowed()
		{
			return true;
		}
	}
}