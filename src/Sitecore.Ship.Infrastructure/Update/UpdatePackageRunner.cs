using System;
using System.Collections.Generic;
using System.IO;
using Sitecore.Ship.Core;
using Sitecore.Update;
using Sitecore.Update.Installer;
using Sitecore.Update.Installer.Exceptions;
using Sitecore.Update.Installer.Installer.Utils;
using Sitecore.Update.Installer.Utils;
using Sitecore.Update.Utils;

namespace Sitecore.Ship.Infrastructure.Update
{
    public interface IPackageRunner
    {
        void Execute(string packagePath);
    }

    public class UpdatePackageRunner : IPackageRunner
    {
        public void Execute(string packagePath)
        {
            if (!File.Exists(packagePath)) throw new NotFoundException();

            using (new ShutdownGuard())
            {
                var installationInfo = GetInstallationInfo(packagePath);
                string historyPath = null;
                List<ContingencyEntry> entries = null;
                try
                {
                    var logger = Sitecore.Diagnostics.LoggerFactory.GetLogger(this);  // TODO abstractions
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

        private PackageInstallationInfo GetInstallationInfo(string packagePath)
        {
            var info = new PackageInstallationInfo
            {
                Mode = InstallMode.Install,
                Action = UpgradeAction.Upgrade,
                Path = packagePath
            };
            if (string.IsNullOrEmpty(info.Path))
            {
                throw new Exception("Package is not selected.");
            }
            return info;
        }
    }
}
