using System.Collections.Specialized;

namespace Sitecore.Ship.Core.Contracts
{
    public interface ICheckRequests
    {
        bool IsLocal { get; }

        string UserHostAddress { get; }

        NameValueCollection Headers { get; }
    }
}