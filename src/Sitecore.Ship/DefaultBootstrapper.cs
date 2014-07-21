using System;
using System.Collections.Generic;
using System.Reflection;

using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.ViewEngines;
using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Core.Services;
using Sitecore.Ship.Infrastructure;
using Sitecore.Ship.Infrastructure.Configuration;
using Sitecore.Ship.Infrastructure.DataAccess;
using Sitecore.Ship.Infrastructure.Diagnostics;
using Sitecore.Ship.Infrastructure.IO;
using Sitecore.Ship.Infrastructure.Install;
using Sitecore.Ship.Infrastructure.Update;
using Sitecore.Ship.Infrastructure.Web;

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

//            container.Register<IConfigurationProvider<PackageInstallationSettings>>(
//                new PackageInstallationConfigurationProvider());
//
            container.AutoRegister();
//            container.Register<IPackageRepository>(
//                new PackageRepository(new UpdatePackageRunner(new PackageManifestReader())));
//
//            container.Register<ILog, Logger>();
//
//            container.Register<IAuthoriser>(
//                new HttpRequestAuthoriser(new HttpRequestChecker(), new PackageInstallationConfigurationProvider()));
//
//            container.Register<ITempPackager>(
//                new TempPackager(new ServerTempFile()));
//
//            container.Register<IPublishService>(
//                new PublishService());
//
//            container.Register<IInstallationRecorder>(
//                new InstallationRecorder(new PackageHistoryRepository(), new PackageInstallationConfigurationProvider()));

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