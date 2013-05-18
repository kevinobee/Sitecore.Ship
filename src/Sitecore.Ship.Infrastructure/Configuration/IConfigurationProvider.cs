namespace Sitecore.Ship.Infrastructure.Configuration
{
    public interface IConfigurationProvider<out T>
    {
        T Settings { get; }
    }
}