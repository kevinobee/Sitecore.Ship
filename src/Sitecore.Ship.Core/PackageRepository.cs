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

        public void AddPackage(InstallPackage package)
        {
            _packageRunner.Execute(package.Path);
        }
    }
}