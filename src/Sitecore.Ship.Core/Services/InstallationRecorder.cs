using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Services
{
    public class InstallationRecorder : IInstallationRecorder
    {
        private readonly IPackageHistoryRepository _packageHistoryRepository;
        private readonly IConfigurationProvider<PackageInstallationSettings> _configurationProvider;

        public InstallationRecorder(IPackageHistoryRepository packageHistoryRepository, IConfigurationProvider<PackageInstallationSettings> configurationProvider)
        {
            _packageHistoryRepository = packageHistoryRepository;
            _configurationProvider = configurationProvider;
        }

        public void RecordInstall(string packagePath, DateTime dateInstalled)
        {
            if (!_configurationProvider.Settings.RecordInstallationHistory) return;

            var packageId = GetPackageIdFromName(packagePath);
            var description = GetDescription(packagePath);
            
            var record = new InstalledPackage
                {
                    DateInstalled = dateInstalled,
                    PackageId = packageId,
                    Description = description
                };

            _packageHistoryRepository.Add(record);
        }

        public void RecordInstall(string packageId, string description, DateTime dateInstalled)
        {
            if (!_configurationProvider.Settings.RecordInstallationHistory) return;

            const string formatString = "Missing {0} parameter, required as installation is being recorded";

            if (string.IsNullOrEmpty(packageId)) throw new ArgumentException(string.Format(formatString, "PackageId"));
            if (string.IsNullOrEmpty(description)) throw new ArgumentException(string.Format(formatString, "Description"));

            var record = new InstalledPackage
                {
                    DateInstalled = dateInstalled,
                    PackageId = packageId,
                    Description = description
                };

            _packageHistoryRepository.Add(record);
        }

        public InstalledPackage GetLatestPackage()
        {
            if (_configurationProvider.Settings.RecordInstallationHistory)
            {
                List<InstalledPackage> children = _packageHistoryRepository.GetAll();
                InstalledPackage package = children.OrderByDescending(x => int.Parse(x.PackageId)).FirstOrDefault();

                if (package != null) return package;
            }

            return new InstalledPackageNotFound();
        }

        private string GetDescription(string packagePath)
        {
           return Path.GetFileName(packagePath).Split('-').Last().Split('.').First();
        }

        private string GetPackageIdFromName(string packagePath)
        {
            //Abstract this so can inject own convention?

            //So, Covention is currently: {ID}-DescriptiveName.extension
            // E.G 01-AboutPage.update
            // E.G 02-HomePage.zip
            return Path.GetFileName(packagePath).Split('-').First();
        }
    }
}