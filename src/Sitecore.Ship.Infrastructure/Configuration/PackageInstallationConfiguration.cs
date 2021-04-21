using System;
using System.Configuration;

namespace Sitecore.Ship.Infrastructure.Configuration
{
    public class PackageInstallationConfiguration : ConfigurationSection
    {
        const string SectionName = "packageInstallation";
        const string EnabledKey = "enabled";
        const string AllowRemoteKey = "allowRemote";
        const string AllowPackageStreamingKey = "allowPackageStreaming";
        const string RecordInstallationHistoryKey = "recordInstallationHistory";
        const string WhitelistElementName = "Whitelist";
        const string MuteAuthorisationFailureLoggingKey = "muteAuthorisationFailureLogging";

        public static PackageInstallationConfiguration GetConfiguration()
        {
            var configuration = ConfigurationManager.GetSection(SectionName) as PackageInstallationConfiguration;
            return configuration ?? new PackageInstallationConfiguration();
        }

        [ConfigurationProperty(EnabledKey, IsRequired = false, DefaultValue = false)]
        public bool Enabled { get { return (bool)this[EnabledKey]; } }

        [ConfigurationProperty(AllowRemoteKey, IsRequired = false, DefaultValue = false)]
        public bool AllowRemoteAccess { get { return (bool)this[AllowRemoteKey]; } }

        [ConfigurationProperty(AllowPackageStreamingKey, IsRequired = false, DefaultValue = false)]
        public bool AllowPackageStreaming { get { return (bool)this[AllowPackageStreamingKey]; } }

        [ConfigurationProperty(RecordInstallationHistoryKey, IsRequired = false, DefaultValue = false)]
        public bool RecordInstallationHistory { get { return (bool)this[RecordInstallationHistoryKey]; } }
        
        [ConfigurationProperty(WhitelistElementName)]
        public WhitelistCollection Whitelist
        {
            get { return ((WhitelistCollection)(base[WhitelistElementName])); }
        }

        [ConfigurationProperty(MuteAuthorisationFailureLoggingKey, IsRequired = false, DefaultValue = false)]
        public bool MuteAuthorisationFailureLogging { get { return (bool)this[MuteAuthorisationFailureLoggingKey]; } }
    }

    [ConfigurationCollection(typeof(WhitelistElement))]
    public class WhitelistCollection : GenericConfigurationElementCollection<WhitelistElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new WhitelistElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            return ((WhitelistElement)element).Name;
        }

        public WhitelistElement this[int idx]
        {
            get
            {
                return (WhitelistElement)BaseGet(idx);
            }
        }
    }

    public class WhitelistElement : ConfigurationElement
    {
        private const string NameKey = "name";

        [ConfigurationProperty(NameKey, DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return ((string)(base[NameKey]));
            }
            set
            {
                base[NameKey] = value;
            }
        }

        private const string IpKey = "IP";

        [ConfigurationProperty(IpKey, DefaultValue = "", IsKey = false, IsRequired = true)]
        public string IP
        {
            get
            {
                return ((string)(base[IpKey]));
            }
            set
            {
                base[IpKey] = value;
            }
        }
    }
}