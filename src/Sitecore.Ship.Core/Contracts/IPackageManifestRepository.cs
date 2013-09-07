using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Contracts
{
    public interface IPackageManifestRepository
    {
        PackageManifest GetManifest(string filename);
    }
}