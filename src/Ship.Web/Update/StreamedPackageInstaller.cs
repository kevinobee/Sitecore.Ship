using System;
using System.IO;
using System.Web;

namespace Ship.Web.Update
{
    public sealed class StreamedPackageInstaller : PackageInstallerBase
    {
        private string _tempFile;

        public StreamedPackageInstaller(HttpContextBase context, IPackageRunner packageRunner)
            : base(context, packageRunner)
        {
        }

        protected override string GetPackageToInstall()
        {
            _tempFile = Context.Server.MapPath(Sitecore.IO.TempFolder.GetFilename(Guid.NewGuid() + ".update"));
            ReadPosIntoFile(Context.Request, _tempFile);
            return _tempFile;
        }

        private static void ReadPosIntoFile(HttpRequestBase clientRequest, string tempFile)
        {
            using (var @out = new FileStream(tempFile, FileMode.OpenOrCreate))
            {
                var buffer = new byte[1024];
                int c;
                while ((c = clientRequest.Files[0].InputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    @out.Write(buffer, 0, c);
                }
            }
        }

        public sealed override void Dispose()
        {
            File.Delete(_tempFile);

            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}