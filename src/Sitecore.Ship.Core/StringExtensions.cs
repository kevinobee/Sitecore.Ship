namespace Sitecore.Ship
{
    public static class StringExtensions
    {
        public static string Formatted(this string target, params object[] args)
        {
            return string.Format(target, args);
        }
    }
}
