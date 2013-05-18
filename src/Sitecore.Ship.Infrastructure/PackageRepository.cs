using Sitecore.Ship.Core;
using Sitecore.Ship.Infrastructure.Update;

namespace Sitecore.Ship.Infrastructure
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