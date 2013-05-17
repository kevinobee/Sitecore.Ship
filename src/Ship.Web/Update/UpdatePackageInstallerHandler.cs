using System;
using System.Web;

namespace Ship.Web.Update
{
    public class UpdatePackageInstallerHandler : BaseHttpHandler
    {
//        private readonly IPackageInstaller _packageInstaller;

        public Func<HttpContextBase, IPackageInstaller> PackageInstaller = (c) => PackageInstallerBuilder.Build(c);
            
//        public UpdatePackageInstallerHandler() 
//            : this(PackageInstallerBuilder.Build())
//        {
//        }
//
//        public UpdatePackageInstallerHandler(IPackageInstaller packageInstaller)
//        {
//            _packageInstaller = packageInstaller;
//        }

        public override void ProcessRequest(HttpContextBase context)
        {
            PackageInstaller(context).Execute();
        }
    }
}
