namespace Sitecore.Ship.Core.Contracts
{
    public interface IConfigurationProvider<out T>
    {
        T Settings { get; }
    }
}