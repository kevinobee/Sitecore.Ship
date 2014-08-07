using System.Collections.Generic;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core
{
    public class PackageRepository : IPackageRepository
    {
        private readonly IPackageRunner _packageRunner;

        public PackageRepository(IPackageRunner packageRunner)
        {
            _packageRunner = packageRunner;
        }

        public PackageManifest AddPackage(InstallPackage package)
        {
            return _packageRunner.Execute(package.Path, package.DisableIndexing);
        }

        public IEnumerable<PackageManifest> AddPackages(InstallPackages packages)
        {
            return _packageRunner.Execute(packages.Paths, packages.DisableIndexing);
        }
    }
}