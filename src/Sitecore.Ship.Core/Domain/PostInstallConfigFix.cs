using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Web;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.Core.Domain
{
    public class PostInstallConfigFix : IPostPackageInstall
    {
        public void Execute(HttpRequestBase request, IEnumerable<PackageManifest> manifests)
        {

            if (request.Form["configFix"] == "1" && manifests.Any())
            {
                RenameConfigs();
            }//if
        }

        public void Execute(dynamic form, IEnumerable<PackageManifest> manifests)
        {
            if (form["configFix"] == "1" && manifests.Any())
            {
                RenameConfigs();
            }//if
        }

        protected void RenameConfigs()
        {
            var currentFolder = HttpContext.Current.Server.MapPath("~");
            var newConfigs =
                Directory.GetFiles(currentFolder, "*.config.*", SearchOption.AllDirectories)
                    .Select(x => x.ToLowerInvariant()).ToList();
            var existingConfigs =
                Directory.GetFiles(currentFolder, "*.config", SearchOption.AllDirectories)
                    .Select(x => x.ToLowerInvariant());

            foreach (var existingConfig in existingConfigs)
            {
                newConfigs.Remove(existingConfig);
            }

            foreach (var existingConfig in existingConfigs)
            {
                var changes = newConfigs
                    .Where(x => x.StartsWith(existingConfig))
                    .Select(x => new FileInfo(x))
                    .OrderByDescending(x => x.CreationTime)
                    .ToList();

                if (changes.Any())
                {
                    var latest = changes.First();

                    foreach (var remove in changes.Skip(1))
                    {
                        File.Delete(remove.FullName);
                    }

                    File.Delete(existingConfig);
                    File.Move(latest.FullName, existingConfig);
                }
            }
        }
    }
}
