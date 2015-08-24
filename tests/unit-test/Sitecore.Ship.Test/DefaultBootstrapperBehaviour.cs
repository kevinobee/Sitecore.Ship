using System;
using System.Linq;
using Nancy;
using Nancy.Extensions;
using Nancy.Testing;
using Should;
using Xunit;

namespace Sitecore.Ship.Test
{
	[Janitor.SkipWeaving]
	public class DefaultBootstrapperBehaviour : IDisposable
	{
		private DefaultNancyBootstrapper _sut;
		private NancyContext _context;

		public DefaultBootstrapperBehaviour()
		{
			_sut = new DefaultBootstrapper();
			_sut.Initialise();
			_context = new NancyContext();
		}

		[Fact]
		public void Engine_is_initialised()
		{
			_sut.GetEngine().ShouldNotBeNull();
		}

		[Theory]
		[InlineData("About")]
		[InlineData("Installer")]
		[InlineData("Publish")]
		public void Expected_modules_are_registered(string expectedModuleName)
		{
			var modules = _sut.GetAllModules(_context);
			
			var moduleNames = modules.Select(x => x.GetModuleName());

			moduleNames.ShouldContain(expectedModuleName);
		}

		// Dev. Note: Left empty for Janitor.Fody to populate. ref: https://github.com/Fody/Janitor
		public void Dispose()
		{
			if (_context != null) _context.Dispose();
			if (_sut != null) _sut.Dispose();
		}
	}
}