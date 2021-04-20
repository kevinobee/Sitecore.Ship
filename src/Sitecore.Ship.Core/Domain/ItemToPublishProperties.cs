using System;
using System.Collections.Generic;

namespace Sitecore.Ship.Core.Domain
{
    public class ItemToPublishProperties
    {
        public Guid ItemId { get; set; }
        public bool PublishChildren { get; set; }
        public bool PublishRelatedItems { get; set; }

        public ItemToPublishProperties()
        {
            ItemId = new Guid();
            PublishChildren = false;
            PublishRelatedItems = false;
        }
    }
}