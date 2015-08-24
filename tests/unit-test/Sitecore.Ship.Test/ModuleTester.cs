using System;
using Moq;
using Nancy;
using Nancy.Testing;
using Nancy.ViewEngines;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.Test
{
	public class ModuleTester<T> where T : INancyModule
	{
		private readonly DefaultNancyBootstrapper _bootstrapper;
		public ConfigurableBootstrapper bootstrapper;

		public Browser Browser { get; private set; }

		public Mock<IAuthoriser> MockAuthoriser { get; private set; }

		public ModuleTester(DefaultNancyBootstrapper bootstrapper)
		{
			if (bootstrapper == null) throw new ArgumentNullException("bootstrapper");

			_bootstrapper = bootstrapper;

			MockAuthoriser = new Mock<IAuthoriser>();

			MockAuthoriser
				.Setup(x => x.IsAllowed())
				.Returns(true);

//			bootstrapper..Dependency(MockAuthoriser.Object);

			Browser = new Browser(_bootstrapper);
		}

		public ModuleTester(IViewLocationProvider viewLocationProvider) 
		{
			MockAuthoriser = new Mock<IAuthoriser>();

			MockAuthoriser
				.Setup(x => x.IsAllowed())
				.Returns(true);

			bootstrapper = new ConfigurableBootstrapper(
				with => ConfigureBootstrapper(with, viewLocationProvider));

			Browser = new Browser(bootstrapper);
		}

		private void ConfigureBootstrapper(ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator bootstrapperConfigurator, IViewLocationProvider viewLocationProvider)
		{
			bootstrapperConfigurator.Module<T>();

			if (viewLocationProvider != null)
			{
				bootstrapperConfigurator.ViewLocationProvider(viewLocationProvider);
			}

			bootstrapperConfigurator.Dependency(MockAuthoriser.Object);
		}
	}
}