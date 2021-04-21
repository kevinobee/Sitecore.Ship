using Sitecore.Ship.Core.Domain;

using System.Collections.ObjectModel;

namespace Sitecore.Ship.Core.Contracts
{
    public interface IPackageHistoryRepository
    {
        void Add(InstalledPackage record);

        Collection<InstalledPackage> GetAll();
    }
}
