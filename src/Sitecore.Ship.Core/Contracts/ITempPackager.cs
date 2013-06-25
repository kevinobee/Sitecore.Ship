using System;
using System.IO;

namespace Sitecore.Ship.Core.Contracts
{
    public interface ITempPackager : IDisposable
    {
        string GetPackageToInstall(Stream source);
    }
}