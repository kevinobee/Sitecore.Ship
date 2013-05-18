using Sitecore.Ship.Core;

namespace Sitecore.Ship.Test
{
    internal class PackageInstallationSettingsBuilder
    {
        private readonly PackageInstallationSettings _packageInstallationSettings;

        public PackageInstallationSettingsBuilder()
        {
            _packageInstallationSettings = new PackageInstallationSettings();
        }

        public PackageInstallationSettingsBuilder WithEnabled()
        {
            _packageInstallationSettings.IsEnabled = true;
            return this;
        }

        public PackageInstallationSettingsBuilder WithRemoteAccess()
        {
            _packageInstallationSettings.AllowRemoteAccess = true;
            return this;
        }

        public PackageInstallationSettingsBuilder WithPackageStreaming()
        {
            _packageInstallationSettings.AllowPackageStreaming = true;
            return this;
        }

        public PackageInstallationSettings Build()
        {
            return _packageInstallationSettings;
        }
    }
}