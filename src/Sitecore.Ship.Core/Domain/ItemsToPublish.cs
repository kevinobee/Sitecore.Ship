using System;
using System.Collections.Generic;

namespace Sitecore.Ship.Core.Domain
{
	public class ItemsToPublish
	{
		public List<Guid> Items { get; set; }
		public string[] TargetDatabases { get; set; }
		public string[] TargetLanguages { get; set; }

		public ItemsToPublish()
		{
			Items = new List<Guid>();
			TargetDatabases = new string[] { };
			TargetLanguages = new string[] { };
		}
	}
}