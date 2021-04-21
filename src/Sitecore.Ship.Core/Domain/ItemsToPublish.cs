using System;
using System.Collections.ObjectModel;

namespace Sitecore.Ship.Core.Domain
{
    public class ItemsToPublish
	{
		public Collection<Guid> Items { get; set; }
		public string[] TargetDatabases { get; set; }
		public string[] TargetLanguages { get; set; }

		public ItemsToPublish()
		{
			Items = new Collection<Guid>();
			TargetDatabases = new string[] { };
			TargetLanguages = new string[] { };
		}
	}
}