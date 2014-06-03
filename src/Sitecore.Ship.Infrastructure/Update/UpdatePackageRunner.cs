using System;
using System.Collections.Generic;
using System.IO;
using Sitecore.Diagnostics;
using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Update;
using Sitecore.Update.Installer;
using Sitecore.Update.Installer.Exceptions;
using Sitecore.Update.Installer.Installer.Utils;
using Sitecore.Update.Installer.Utils;
using Sitecore.Update.Metadata;
using Sitecore.Update.Utils;
using Sitecore.Update.Wizard;

namespace Sitecore.Ship.Infrastructure.Update
{
    public class UpdatePackageRunner : IPackageRunner
    {
        private readonly IPackageManifestRepository _manifestRepository;

        public UpdatePackageRunner(IPackageManifestRepository manifestRepository)
        {
            _manifestRepository = manifestRepository;
        }

        public PackageManifest Execute(string packagePath, bool disableIndexing)
        {
            if (!File.Exists(packagePath)) throw new NotFoundException();

            using (new ShutdownGuard())
            {
                if (disableIndexing)
                {
                    Sitecore.Configuration.Settings.Indexing.Enabled = false;
                }

                var installationInfo = GetInstallationInfo(packagePath);
                string historyPath = null;
                List<ContingencyEntry> entries = null;
                try
                {
                    var logger = Diagnostics.LoggerFactory.GetLogger(this); // TODO abstractions
                    entries = UpdateHelper.Install(installationInfo, logger, out historyPath);

                    string error = string.Empty;

                    logger.Info("Executing post installation actions.");

                    MetadataView metadata = PreviewMetadataWizardPage.GetMetadata(packagePath, out error);

                    if (string.IsNullOrEmpty(error))
                    {
                        DiffInstaller diffInstaller = new DiffInstaller(UpgradeAction.Upgrade);
                        diffInstaller.ExecutePostInstallationInstructions(packagePath, historyPath,
                            installationInfo.Mode, metadata, logger, ref entries);
                    }
                    else
                    {
                        logger.Info("Post installation actions error.");
                        logger.Error(error);
                    }

                    logger.Info("Executing post installation actions finished.");

                    return _manifestRepository.GetManifest(packagePath);

                }
                catch (PostStepInstallerException exception)
                {
                    entries = exception.Entries;
                    historyPath = exception.HistoryPath;
                    throw;
                }
                finally
                {
                    if (disableIndexing)
                    {
                        Sitecore.Configuration.Settings.Indexing.Enabled = true;
                    }

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
