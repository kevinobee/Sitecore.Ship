using System.IO;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.Infrastructure.IO
{
    public sealed class TempPackager : ITempPackager
    {
        private readonly ITempFile _tempFile;
        private string _tempPackageFile;

        public TempPackager(ITempFile tempFile)
        {
            _tempFile = tempFile;
        }

        public string GetPackageToInstall(Stream source)
        {
            _tempPackageFile = _tempFile.Filename;
            ReadPosIntoFile(source, _tempPackageFile);
            return _tempPackageFile;
        }

        private static void ReadPosIntoFile(Stream source, string tempFile)
        {
            using (var @out = new FileStream(tempFile, FileMode.OpenOrCreate))
            {
                var buffer = new byte[1024];
                int c;
                while ((c = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    @out.Write(buffer, 0, c);
                }
            }
        }

        public void Dispose()
        {
            File.Delete(_tempPackageFile);
        }
    }
}