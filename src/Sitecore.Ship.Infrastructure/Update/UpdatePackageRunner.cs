using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.IO;
using Sitecore.SecurityModel;
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

                var logger = Sitecore.Diagnostics.LoggerFactory.GetLogger(this); // TODO abstractions
                try
                {
                    entries = UpdateHelper.Install(installationInfo, logger, out historyPath);

                    string error = string.Empty;

                    logger.Info("Executing post installation actions.");

                    MetadataView metadata = PreviewMetadataWizardPage.GetMetadata(packagePath, out error);

                    if (string.IsNullOrEmpty(error))
                    {
                        DiffInstaller diffInstaller = new DiffInstaller(UpgradeAction.Upgrade);
                        using (new SecurityDisabler())
                        {
                            diffInstaller.ExecutePostInstallationInstructions(packagePath, historyPath, installationInfo.Mode, metadata, logger, ref entries);
                        }
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

                    try
                    {
                        SaveInstallationMessages(entries, historyPath);
                    }
                    catch (Exception)
                    {
                        logger.Error("Failed to record installation messages");
                        foreach (var entry in entries ?? Enumerable.Empty<ContingencyEntry>())
                        {
                            logger.Info(string.Format("Entry [{0}]-[{1}]-[{2}]-[{3}]-[{4}]-[{5}]-[{6}]-[{7}]-[{8}]-[{9}]-[{10}]-[{11}]",
                                entry.Action,
                                entry.Behavior,
                                entry.CommandKey,
                                entry.Database,
                                entry.Level,
                                entry.LongDescription,
                                entry.MessageGroup,
                                entry.MessageGroupDescription,
                                entry.MessageID,
                                entry.MessageType,
                                entry.Number,
                                entry.ShortDescription));
                        }
                        throw;
                    }
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

        private void SaveInstallationMessages(List<ContingencyEntry> entries, string historyPath)
        {
            string path = Path.Combine(historyPath, "messages.xml");

            FileUtil.EnsureFolder(path);

            using (FileStream fileStream = File.Create(path))
            {
                new XmlEntrySerializer().Serialize(entries, fileStream);
            }
        }
    }
}