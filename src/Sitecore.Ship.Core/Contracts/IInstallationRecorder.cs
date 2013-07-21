using System;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Contracts
{
    public interface IInstallationRecorder
    {
        void RecordInstall(string packageFileName, DateTime dateInstalled);
        void RecordInstall(string packageId, string description, DateTime dateInstalled);

        InstalledPackage GetLatestPackage();
    }
}
