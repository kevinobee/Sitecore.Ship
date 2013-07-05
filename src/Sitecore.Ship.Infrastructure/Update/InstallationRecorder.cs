using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Infrastructure.Update
{
    public class InstallationRecorder : IInstallationRecorder
    {
        private readonly IPackageHistoryRepository _packageHistoryRepository;
        private readonly IConfigurationProvider<PackageInstallationSettings> m_configurationProvider;

        public InstallationRecorder(IPackageHistoryRepository packageHistoryRepository, IConfigurationProvider<PackageInstallationSettings> configurationProvider)
        {
            _packageHistoryRepository = packageHistoryRepository;
            m_configurationProvider = configurationProvider;
        }

        public void RecordInstall(string packagePath, DateTime dateInstalled)
        {
            if (m_configurationProvider.Settings.RecordInstallationHistory)
            {
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
        }

        public InstalledPackage GetLatestPackage()
        {
            List<InstalledPackage> children = _packageHistoryRepository.GetAll();
            return children.OrderByDescending(x => int.Parse(x.PackageId)).FirstOrDefault();
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