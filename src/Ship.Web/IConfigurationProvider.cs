namespace Ship.Web
{
    public interface IConfigurationProvider<out T>
    {
        T Settings { get; }
    }
}