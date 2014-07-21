using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.Infrastructure.Diagnostics
{
    public class Logger : ILog
    {
        public void Write(string message)
        {
            Sitecore.Diagnostics.Log.Warn(message, this);
        }
    }
}