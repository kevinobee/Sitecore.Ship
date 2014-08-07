using System.Collections.Generic;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Contracts
{
    public interface IPackageRunner
    {
        PackageManifest Execute(string packagePath, bool disableIndexing);
        IEnumerable<PackageManifest> Execute(IEnumerable<string> packagePath, bool disableIndexing);
    }
}