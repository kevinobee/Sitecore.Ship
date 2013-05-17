using System;
using System.Collections.Generic;
using System.IO;
using Sitecore.Update;
using Sitecore.Update.Installer;
using Sitecore.Update.Installer.Exceptions;
using Sitecore.Update.Installer.Installer.Utils;
using Sitecore.Update.Installer.Utils;
using Sitecore.Update.Utils;

namespace Ship.Web.Update
{
    public class PackageRunner : IPackageRunner
    {
        public void Execute()
        {
            if (!File.Exists(PackagePath)) throw new NotFoundException();

            using (new ShutdownGuard())
            {
                var installationInfo = GetInstallationInfo();
                string historyPath = null;
                List<ContingencyEntry> entries = null;
                try
                {
                    var logger = Sitecore.Diagnostics.LoggerFactory.GetLogger(this);
                    entries = UpdateHelper.Install(installationInfo, logger, out historyPath);
                }
                catch (PostStepInstallerException exception)
                {
                    entries = exception.Entries;
                    historyPath = exception.HistoryPath;
                    throw;
                }
                finally
                {
                    UpdateHelper.SaveInstallationMessages(entries, historyPath);
                }
            }
        }

        private PackageInstallationInfo GetInstallationInfo()
        {
            var info = new PackageInstallationInfo
            {
                Mode = InstallMode.Install,
                Action = UpgradeAction.Upgrade,
                Path = PackagePath
            };
            if (string.IsNullOrEmpty(info.Path))
            {
                throw new Exception("Package is not selected.");
            }
            return info;
        }

        public string PackagePath { get; set; }
    }

    [Serializable]
    public class NotFoundException : Exception { }
}
