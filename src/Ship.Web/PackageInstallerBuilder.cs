using System;
using System.Collections.Generic;
using System.Web;
using Ship.Web.Update;
using Ship.Web.Update.Configuration;

namespace Ship.Web
{
    class PackageInstallerBuilder
    {
        public static IPackageInstaller Build(HttpContextBase httpContext)
        {
            var configurationProvider = new PackageInstallationConfigurationProvider();
            var packageRunner = new PackageRunner();

            IPackageInstaller serverPackageInstaller = new ServerPackageFileInstaller(httpContext, packageRunner);
            IPackageInstaller streamedPackageInstaller = new StreamedPackageInstaller(httpContext, packageRunner);

            IDictionary<string, Action<HttpContextBase>> httpMethodActions = new Dictionary<string, Action<HttpContextBase>>
            {
                { "GET",  c => serverPackageInstaller.Execute() },
                { "POST", c => streamedPackageInstaller.Execute() }
            };

            var packageInstaller = new PackageInstaller(httpContext,
                                                        new HttpRequestAuthoriser(httpContext, configurationProvider),
                                                        httpMethodActions);
            return packageInstaller;
        }
    }
}
