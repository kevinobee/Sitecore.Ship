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
using Sitecore.Ship.Infrastructure.DataAccess;
using Sitecore.Ship.Package.Install;

namespace Sitecore.Ship
{
    public class DefaultBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            Nancy.Json.JsonSettings.MaxJsonLength = int.MaxValue;
        }

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.ViewLocationConventions.Add((viewName, model, context) =>
                                                         string.Concat(context.ModuleName, "/views/", viewName.ToLower()));
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.AutoRegister(new[]
            {
                typeof (IPackageRepository).Assembly, 
                typeof (InstallerModule).Assembly,
                typeof (PackageHistoryRepository).Assembly
            });

            container.Register(typeof(PackageInstallationSettings), new PackageInstallationConfigurationProvider().Settings);

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
                            asm => asm.FullName.StartsWith("Oracle", StringComparison.InvariantCulture),
                            asm => asm.FullName.StartsWith("Sitecore", StringComparison.InvariantCulture)
                        }
                    );

                ignoredAssemblies.Remove(asm => ! asm.FullName.StartsWith("Sitecore.Ship", StringComparison.InvariantCulture));

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