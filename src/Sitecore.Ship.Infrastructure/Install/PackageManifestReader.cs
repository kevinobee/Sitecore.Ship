using System;
using System.IO;
using System.Text;
using Sitecore.Install;
using Sitecore.Install.Zip;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Core.Services;
using Sitecore.Zip;

namespace Sitecore.Ship.Infrastructure.Install
{
    public class PackageManifestReader : IPackageManifestRepository
    {
        public PackageManifest GetManifest(string filename)
        {
            var manifest = new PackageManifest();

            ZipReader reader;
            try
            {
                reader = new ZipReader(filename, Encoding.UTF8);
            }
            catch (Exception exception)
            {          
                throw new InvalidOperationException("Failed to open package", exception);
            }

            string tempFileName = Path.GetTempFileName();
            ZipEntry entry = reader.GetEntry("package.zip");
            if (entry != null)
            {
                using (FileStream stream = File.Create(tempFileName))
                {
                    StreamUtil.Copy(entry.GetStream(), stream, 0x4000);
                }
                reader.Dispose();
                reader = new ZipReader(tempFileName, Encoding.UTF8);
            }
            try
            {
                foreach (ZipEntry entry2 in reader.Entries)
                {
                    var data = new ZipEntryData(entry2);

                    var packageManifestEntry = ZipEntryDataParser.GetManifestEntry(data.Key);

                    if (! (packageManifestEntry is PackageManifestEntryNotFound))
                    {
                        manifest.Entries.Add(packageManifestEntry);
                    }
                }
            }
            finally
            {
                reader.Dispose();
                File.Delete(tempFileName);
            }

            return manifest;
        }
    }
}