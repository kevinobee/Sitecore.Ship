using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Nancy;
using Nancy.ModelBinding;

using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Infrastructure.Web;

namespace Sitecore.Ship.Package.Install
{
    public class InstallerModule : NancyModule
    {
        private readonly IPackageRepository _repository;
        private readonly IAuthoriser _authoriser;
        private readonly ITempPackager _tempPackager;
        private readonly IInstallationRecorder _installationRecorder;

        const string StartTime = "start_time";

        public InstallerModule(IPackageRepository repository, IAuthoriser authoriser, ITempPackager tempPackager, IInstallationRecorder installationRecorder)
            : base("/services")
        {
            _repository = repository;
            _authoriser = authoriser;
            _tempPackager = tempPackager;
            _installationRecorder = installationRecorder;

            Before += AuthoriseRequest; 
            
            Before += ctx =>
            {
                ctx.Items.Add(StartTime, DateTime.UtcNow);
                return null;
            };

            After += AddProcessingTimeToResponse;

            Post["/package/install/fileupload"] = InstallUploadPackage;

            Post["/package/install"] = InstallPackage;

            Get["/package/latestversion"] = LatestVersion;
        }

        private Response AuthoriseRequest(NancyContext ctx)
        {
            if (!_authoriser.IsAllowed())
            {
                ctx.Response = new Response {StatusCode = HttpStatusCode.Unauthorized};
            }
            return null;
        }

        private static void AddProcessingTimeToResponse(NancyContext ctx)
        {
            var processTime = (DateTime.UtcNow - (DateTime)ctx.Items[StartTime]).TotalMilliseconds;

            ctx.Response.WithHeader("x-processing-time", processTime.ToString(CultureInfo.InvariantCulture));
        }

        private dynamic InstallPackage(dynamic o)
        {
            try
            {
                var package = this.Bind<InstallPackage>();
                var manifest = _repository.AddPackage(package);
                _installationRecorder.RecordInstall(package.Path, DateTime.Now);

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
                    var package = new InstallPackage { Path = _tempPackager.GetPackageToInstall(file.Value) };
                    manifest = _repository.AddPackage(package);
                    _installationRecorder.RecordInstall(uploadPackage.PackageId, uploadPackage.Description, DateTime.Now);
                }
                finally 
                {
                    _tempPackager.Dispose();
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