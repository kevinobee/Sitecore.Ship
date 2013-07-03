using System;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Contracts
{
    public interface IInstallationRecorder
    {
        void RecordInstall(string packageFileName, DateTime dateInstalled);
        InstalledPackage GetLatestPackage();
    }
}
