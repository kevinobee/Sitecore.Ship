using System.Collections.Generic;
using System.IO;
using Sitecore.Ship.Core.Contracts;

namespace Sitecore.Ship.Infrastructure.IO
{
    public sealed class TempPackager : ITempPackager
    {
        private readonly ITempFile _tempFile;
        private IList<string> _tempPackageFiles;

        public TempPackager(ITempFile tempFile)
        {
            _tempFile = tempFile;
            _tempPackageFiles = new List<string>();
        }

        public string GetPackageToInstall(Stream source)
        {
            var fileName = _tempFile.Filename;
            _tempPackageFiles.Add(fileName);
            ReadPosIntoFile(source, fileName);
            return fileName;
        }

        private static void ReadPosIntoFile(Stream source, string tempFile)
        {
            using (var @out = new FileStream(tempFile, FileMode.OpenOrCreate))
            {
                source.CopyTo(@out);
            }
        }

        public void Dispose()
        {
            foreach (var fileName in _tempPackageFiles)
            {
                File.Delete(fileName);
            }
        }
    }
}