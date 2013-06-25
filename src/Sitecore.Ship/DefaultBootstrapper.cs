using System;
using System.Collections.Generic;
using System.Reflection;

using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.ViewEngines;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Infrastructure.Configuration;

namespace Sitecore.Ship
{
    public class DefaultBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.ViewLocationConventions.Add((viewName, model, context) =>
                                                         string.Concat(context.ModuleName, "/views/", viewName.ToLower()));
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IConfigurationProvider<PackageInstallationSettings>>(
                new PackageInstallationConfigurationProvider());

            var assembly = GetType().Assembly;
            ResourceViewLocationProvider
                .RootNamespaces
                .Add(assembly, "Sitecore.Ship.About.Views");
        }

        protected override IEnumerable<Func<Assembly, bool>> AutoRegisterIgnoredAssemblies
        {
            get
            {
                var ignoredAssemblies = new List<Func<Assembly, bool>>();
                ignoredAssemblies.AddRange(base.AutoRegisterIgnoredAssemblies);
                ignoredAssemblies.AddRange(
                    new Func<Assembly, bool>[]
                        {
                            asm => asm.FullName.StartsWith("Oracle", StringComparison.InvariantCulture)
                        }
                    );

                ignoredAssemblies.Remove(asm => asm.FullName.StartsWith("Sitecore.Ship", StringComparison.InvariantCulture));

                return ignoredAssemblies;
            }
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(OnConfigurationBuilder);
            }
        }

        void OnConfigurationBuilder(NancyInternalConfiguration x)
        {
            x.ViewLocationProvider = typeof(ResourceViewLocationProvider);
        }
    }
}