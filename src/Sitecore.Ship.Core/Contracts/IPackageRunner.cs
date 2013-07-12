using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Contracts
{
    public interface IPackageRunner
    {
        PackageManifest Execute(string packagePath);
    }
}