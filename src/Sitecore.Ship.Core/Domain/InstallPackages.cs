using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.Ship.Core.Domain
{
    public class InstallPackages
    {
        public IEnumerable<string> Paths { get; set; }

        public bool DisableIndexing { get; set; }
    }
}
