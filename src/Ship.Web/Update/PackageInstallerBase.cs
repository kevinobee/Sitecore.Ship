using System;
using System.Net;
using System.Web;

namespace Ship.Web.Update
{
    public abstract class PackageInstallerBase : IPackageInstaller, IDisposable
    {
        protected readonly HttpContextBase Context;
        private readonly IPackageRunner _packageRunner;

        protected PackageInstallerBase(HttpContextBase context, IPackageRunner packageRunner)
        {
            Context = context;
            _packageRunner = packageRunner;
        }

        protected abstract string GetPackageToInstall();

        public void Execute()
        {
            _packageRunner.PackagePath = GetPackageToInstall();

            try
            {
                _packageRunner.Execute();
            }
            catch (NotFoundException)
            {
                Context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            }
            catch (Exception)
            {
                Context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            }
        }

        public virtual void Dispose()
        {
            // By default noting to clean up
        }
    }
}