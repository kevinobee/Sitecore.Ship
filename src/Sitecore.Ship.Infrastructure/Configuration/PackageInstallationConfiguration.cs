using System.Configuration;

namespace Sitecore.Ship.Infrastructure.Configuration
{
    public class PackageInstallationConfiguration : ConfigurationSection
    {
        private const string SectionName = "packageInstallation";
        private const string EnabledKey = "enabled";
        private const string AllowRemoteKey = "allowRemote";
        private const string AllowPackageStreamingKey = "allowPackageStreaming";
        private const string WhitelistElementName = "Whitelist";

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

        [ConfigurationProperty(WhitelistElementName)]
        public WhitelistCollection Whitelist
        {
            get { return ((WhitelistCollection)(base[WhitelistElementName])); }
        }
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
            return ((WhitelistElement)(element)).Name;
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