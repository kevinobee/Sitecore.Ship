using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Core.Contracts
{
    public interface IPostPackageInstall
    {
        void Execute(dynamic form, IEnumerable<PackageManifest> manifests);
    }
}
