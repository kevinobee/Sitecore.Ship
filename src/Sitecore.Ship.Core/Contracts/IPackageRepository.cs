using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Contracts
{
    public interface IPackageRepository
    {
        void AddPackage(InstallPackage package);
    }
}