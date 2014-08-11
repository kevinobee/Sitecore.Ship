using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Nancy;
using Nancy.ModelBinding;

using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Infrastructure.IO;
using Sitecore.Ship.Infrastructure.Web;

namespace Sitecore.Ship.Package.Install
{
    public class InstallerModule : NancyModule
    {
        private readonly IPackageRepository _repository;
        private readonly IAuthoriser _authoriser;
        private readonly ITempPackager _tempPackager;
        private readonly IInstallationRecorder _installationRecorder;
        private readonly IEnumerable<IPostPackageInstall> _postPackageInstalls;

        const string StartTime = "start_time";

        public InstallerModule
            (IPackageRepository repository, IAuthoriser authoriser, ITempPackager tempPackager, IInstallationRecorder installationRecorder, IEnumerable<IPostPackageInstall> postPackageInstalls    )
            : base("/services")
        {
            _repository = repository;
            _authoriser = authoriser;
            _tempPackager = tempPackager;
            _installationRecorder = installationRecorder;
            _postPackageInstalls = postPackageInstalls;

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


                foreach (var post in _postPackageInstalls)
                {
                    post.Execute(Request, new []{manifest});
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
            if (Request.Files.Count() > 1)
            {
                return InstallMultiplePackages(o);
            }
            else
            {
                return InstallSinglePackage(o);
            }


        }

        private dynamic InstallSinglePackage(dynamic o)
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

                    foreach (var post in _postPackageInstalls)
                    {
                        post.Execute(Request.Form, new[] { manifest });
                    }

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

        private dynamic InstallMultiplePackages(dynamic o)
        {
            try
            {

                var uploadPackage = this.Bind<InstallUploadPackage>();

                IEnumerable<PackageManifest> manifests;

                List<string> filePaths = new List<string>();

                try
                {

                    for (var i = 0; i < Request.Files.Count(); i++)
                    {
                        var file = Request.Files.Skip(i).FirstOrDefault();

                        if (file != null)
                        {
                            var filePath = _tempPackager.GetPackageToInstall(file.Value);
                            filePaths.Add(filePath);
                        }

                    }

                    InstallPackages packages = new InstallPackages();
                    packages.Paths = filePaths;
                    packages.DisableIndexing = Request.Form["disableIndexing"] == "1";

                    manifests = _repository.AddPackages(packages);
                    _installationRecorder.RecordInstall(uploadPackage.PackageId, uploadPackage.Description, DateTime.Now);

                    foreach (var post in _postPackageInstalls)
                    {
                        post.Execute(Request.Form, manifests);
                    }

                }
                finally
                {
                    _tempPackager.Dispose();
                }

                return Response
                            .AsJson(manifests, HttpStatusCode.Created)
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