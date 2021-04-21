using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Services
{
    public class InstallationRecorder : IInstallationRecorder
    {
        private readonly IPackageHistoryRepository _packageHistoryRepository;
        private readonly PackageInstallationSettings _packageInstallationSettings;

        public InstallationRecorder(IPackageHistoryRepository packageHistoryRepository, PackageInstallationSettings packageInstallationSettings)
        {
            _packageHistoryRepository = packageHistoryRepository;
            _packageInstallationSettings = packageInstallationSettings;
        }

        public void RecordInstall(string packagePath, DateTime dateInstalled)
        {
            if (!_packageInstallationSettings.RecordInstallationHistory) return;

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
            if (!_packageInstallationSettings.RecordInstallationHistory) return;

            const string formatString = "Missing {0} parameter, required as installation is being recorded";

            if (string.IsNullOrEmpty(packageId)) throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, formatString, "PackageId"));
            if (string.IsNullOrEmpty(description)) throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, formatString, "Description"));

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
            if (_packageInstallationSettings.RecordInstallationHistory)
            {
                var children = _packageHistoryRepository.GetAll();
                var package = children.OrderByDescending(x => int.Parse(x.PackageId, CultureInfo.CurrentCulture)).FirstOrDefault();

                if (package != null) return package;
            }

            return new InstalledPackageNotFound();
        }

        private static string GetDescription(string packagePath)
        {
           return Path.GetFileName(packagePath).Split('-').Last().Split('.').First();
        }

        private static string GetPackageIdFromName(string packagePath)
        {
            //Abstract this so can inject own convention?

            //So, Convention is currently: {ID}-DescriptiveName.extension
            // E.G 01-AboutPage.update
            // E.G 02-HomePage.zip
            return Path.GetFileName(packagePath).Split('-').First();
        }
    }
}