using System.Collections.Generic;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Contracts
{
    public interface IPackageRepository
    {
        PackageManifest AddPackage(InstallPackage package);
        IEnumerable<PackageManifest> AddPackages(InstallPackages package);
    }
}