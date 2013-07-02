using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.Ship.Publish
{
    public class PublishLastCompletedRequest
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public string Language { get; set; }
    }
}
