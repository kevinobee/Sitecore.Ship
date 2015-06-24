using System;
using System.Linq;

using Nancy;
using Nancy.ModelBinding;

using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Infrastructure.Web;

namespace Sitecore.Ship.Package.Install
{
    public class InstallerModule : ShipBaseModule
    {
        private readonly IPackageRepository _repository;
        private readonly ITempPackager _tempPackager;
        private readonly IInstallationRecorder _installationRecorder;

        public InstallerModule(IPackageRepository repository, IAuthoriser authoriser, ITempPackager tempPackager, IInstallationRecorder installationRecorder)
            : base(authoriser, "/services")
        {
            _repository = repository;
            _tempPackager = tempPackager;
            _installationRecorder = installationRecorder;

            Post["/package/install/fileupload"] = InstallUploadPackage;

            Post["/package/install"] = InstallPackage;

            Get["/package/latestversion"] = LatestVersion;
        }

        private dynamic InstallPackage(dynamic o)
        {
            try
            {
                var package = this.Bind<InstallPackage>();
                var manifest = _repository.AddPackage(package);
                _installationRecorder.RecordInstall(package.Path, DateTime.Now);

                if (package.DisableManifest)
                {
                    // Skip manifest reporting. Nancy will return an empty message body.
                    manifest = null;
                }

                return Response
                            .AsJson(manifest, HttpStatusCode.Created)
                            .WithHeader("Location", ShipServiceUrl.PackageLatestVersion);
            }
            catch (NotFoundException)
            {
                return new Response
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }
        }

        private dynamic InstallUploadPackage(dynamic o)
        {
            try
            {
                var file = Request.Files.FirstOrDefault();

                var uploadPackage = this.Bind<InstallUploadPackage>();

                if (file == null)
                {
                    return new Response {StatusCode = HttpStatusCode.BadRequest};
                }

                PackageManifest manifest;
                try
                {
                    var package = new InstallPackage
                                      {
                                          Path = _tempPackager.GetPackageToInstall(file.Value), 
                                          DisableIndexing = uploadPackage.DisableIndexing
                                      };
                    manifest = _repository.AddPackage(package);
                    _installationRecorder.RecordInstall(uploadPackage.PackageId, uploadPackage.Description, DateTime.Now);
                }
                finally 
                {
                    _tempPackager.Dispose();
                }

                if (uploadPackage.DisableManifest)
                {
                    // Skip manifest reporting. Nancy will return an empty message body.
                    manifest = null;
                }

                return Response
                            .AsJson(manifest, HttpStatusCode.Created)
                            .WithHeader("Location", ShipServiceUrl.PackageLatestVersion);
            }
            catch (NotFoundException)
            {
                return new Response
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }
        }

        private dynamic LatestVersion(dynamic o)
        {
            try
            {
                var installedPackage = _installationRecorder.GetLatestPackage();

                if (installedPackage is InstalledPackageNotFound)
                {
                    return new Response
                    {
                        StatusCode = HttpStatusCode.NoContent
                    }; 
                }

                return Response.AsJson(installedPackage);
            }
            catch (NotFoundException)
            {
                return new Response
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }
        }
    }
}