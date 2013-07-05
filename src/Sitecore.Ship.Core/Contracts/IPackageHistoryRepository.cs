using System.Collections.Generic;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Contracts
{
    public interface IPackageHistoryRepository
    {
        void Add(InstalledPackage record);
        List<InstalledPackage> GetAll();
    }
}
