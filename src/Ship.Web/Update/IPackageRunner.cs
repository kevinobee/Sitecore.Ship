namespace Ship.Web.Update
{
    public interface IPackageRunner
    {
        void Execute();
        string PackagePath { get; set; }
    }
}