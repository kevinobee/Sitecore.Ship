using System.Collections.Generic;
using System.Configuration;

namespace Sitecore.Ship.Infrastructure.Configuration
{
    public abstract class GenericConfigurationElementCollection<T> : ConfigurationElementCollection, IEnumerable<T>
        where T : ConfigurationElement, new()
    {
        public new IEnumerator<T> GetEnumerator()
        {
            var count = Count;
            for (var i = 0; i < count; i++)
            {
                yield return BaseGet(i) as T;
            }
        }
    }
}