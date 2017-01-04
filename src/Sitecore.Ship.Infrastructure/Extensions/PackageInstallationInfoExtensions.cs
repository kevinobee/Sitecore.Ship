using System.Reflection;
using Sitecore.Update;

namespace Sitecore.Ship.Infrastructure.Extensions
{
    public static class PackageInstallationInfoExtensions
    {
        public static void SetProcessingMode(this PackageInstallationInfo info)
        {
            var prop = info.GetType().GetProperty("ProcessingMode", BindingFlags.Public | BindingFlags.Instance);
            if (prop == null || !prop.CanWrite)
            {
                //Pre Sitecore 8.2 Update 2 Versions have no such property
                return;
            }

            // ProcessingMode.All = 2147483647
            prop.SetValue(info, 2147483647);
        }
    }
}
