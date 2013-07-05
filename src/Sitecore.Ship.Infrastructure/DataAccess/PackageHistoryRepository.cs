using System;
using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Infrastructure.DataAccess
{
    public class PackageHistoryRepository : IPackageHistoryRepository
    {
        private const string HISTORY_FOLDER_PATH = "/sitecore/content/PackageHistory";
        private const string PACKAGE_HISTORY_TEMPLATE_PATH = "SitecoreShip/InstalledPackage";
        private const string DATABASE_NAME = "master";
        private const string PACKAGE_ID_FIELD_NAME = "PackageId";
        private const string DATE_INSTALLED_FIELD_NAME = "DateInstalled";
        private const string DESCRIPTION_FIELD_NAME = "Description";

        public void Add(InstalledPackage package)
        {
            using (new SecurityDisabler())
            {
                var database = Factory.GetDatabase(DATABASE_NAME);
                var rootItem = database.GetItem(HISTORY_FOLDER_PATH);
                TemplateItem template = database.GetTemplate(PACKAGE_HISTORY_TEMPLATE_PATH);
                var item = rootItem.Add(package.PackageId, template);

                try
                {
                    item.Editing.BeginEdit();
                    item.Fields[PACKAGE_ID_FIELD_NAME].Value = package.PackageId;
                    item.Fields[DATE_INSTALLED_FIELD_NAME].Value = package.DateInstalled.ToString();
                    item.Fields[DESCRIPTION_FIELD_NAME].Value = package.Description;
                }
                finally
                {
                    item.Editing.EndEdit();
                }
            }
        }

        public List<InstalledPackage> GetAll()
        {
            var database = Factory.GetDatabase(DATABASE_NAME);
            var rootItem = database.GetItem(HISTORY_FOLDER_PATH);
            var entries = new List<InstalledPackage>();
            foreach (Item child in rootItem.Children)
            {
                entries.Add(new InstalledPackage()
                    {
                        DateInstalled = DateTime.Parse(child.Fields[DATE_INSTALLED_FIELD_NAME].Value),
                        PackageId = child.Fields[PACKAGE_ID_FIELD_NAME].Value,
                        Description = child.Fields[DESCRIPTION_FIELD_NAME].Value
                    });
            }

            return entries;
        }
    }
}
