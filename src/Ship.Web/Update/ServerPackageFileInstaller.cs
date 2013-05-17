using System.Web;

namespace Ship.Web.Update
{
    public class ServerPackageFileInstaller : PackageInstallerBase
    {
        public ServerPackageFileInstaller(HttpContextBase context, IPackageRunner packageRunner) : base(context, packageRunner)
        {
        }

        protected override string GetPackageToInstall()
        {
            var pathString = Context.Request.QueryString["package"];
            return pathString == null ? string.Empty : HttpUtility.UrlDecode(pathString);
        }
    }
}