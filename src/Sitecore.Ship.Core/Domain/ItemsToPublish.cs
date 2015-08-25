using System;
using System.Collections.Generic;

namespace Sitecore.Ship.Core.Domain
{
    public class ItemsToPublish
    {
        public List<Guid> Items { get; set; }
        public string[] TargetDatabases { get; set; }
        public string[] TargetLanguages { get; set; }
        public bool SmartPublish { get; set; }
        public bool DeepPublish { get; set; }
        public bool PublishRelatedItems { get; set; }
    }
}
