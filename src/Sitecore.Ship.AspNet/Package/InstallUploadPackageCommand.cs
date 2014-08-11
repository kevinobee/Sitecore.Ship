using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;

using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;
using Sitecore.Ship.Core.Services;
using Sitecore.Ship.Infrastructure.Configuration;
using Sitecore.Ship.Infrastructure.DataAccess;
using Sitecore.Ship.Infrastructure.IO;
using Sitecore.Ship.Infrastructure.Install;
using Sitecore.Ship.Infrastructure.Update;
using Sitecore.Ship.Infrastructure.Web;

namespace Sitecore.Ship.AspNet.Package
{
    public class InstallUploadPackageCommand : CommandHandler
    {
        private readonly IPackageRepository _repository;
        private readonly ITempPackager _tempPackager;
        private readonly IInstallationRecorder _installationRecorder;
        private readonly IEnumerable<IPostPackageInstall> _postPackageInstalls;

        public InstallUploadPackageCommand(
            IPackageRepository repository, 
            ITempPackager tempPackager, 
            IInstallationRecorder installationRecorder,
            IEnumerable<IPostPackageInstall> postPackageInstalls)
        {
            _repository = repository;
            _tempPackager = tempPackager;
            _installationRecorder = installationRecorder;
            _postPackageInstalls = postPackageInstalls;
        }

        public InstallUploadPackageCommand()
            : this(new PackageRepository(new UpdatePackageRunner(new PackageManifestReader())), 
                   new TempPackager(new ServerTempFile()), 
                   new InstallationRecorder(new PackageHistoryRepository(), new PackageInstallationConfigurationProvider()),
                    new IPostPackageInstall[]{new PostInstallConfigFix()}
            )
        {           
        }

        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                if (context.Request.Files.Count > 1)
                {
                    UploadMultiple(context);
                }
                else
                {
                    UploadSingle(context);
                }
            }
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }

        private static bool CanHandle(HttpContextBase context)
        {
            return context.Request.Url != null &&
                   context.Request.Url.PathAndQuery.EndsWith("/services/package/install/fileupload", StringComparison.InvariantCultureIgnoreCase) && 
                   context.Request.HttpMethod == "POST";
        }

        private static InstallUploadPackage GetRequest(HttpRequestBase request)
        {
            return new InstallUploadPackage
                {
                    PackageId = request.Form["packageId"],
                    Description = request.Form["description"]
                };
        }

        protected void UploadSingle(HttpContextBase context)
        {
            try
            {
                if (context.Request.Files.Count == 0)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                var file = context.Request.Files[0];

                var uploadPackage = GetRequest(context.Request);

                PackageManifest manifest;
                try
                {
                    var package = new InstallPackage { Path = _tempPackager.GetPackageToInstall(file.InputStream) };
                    manifest = _repository.AddPackage(package);

                    _installationRecorder.RecordInstall(uploadPackage.PackageId, uploadPackage.Description, DateTime.Now);

                }
                finally
                {
                    _tempPackager.Dispose();
                }

                foreach (var postInstall in _postPackageInstalls)
                {
                    postInstall.Execute(context.Request, new[] { manifest });
                }



                var json = Json.Encode(new { manifest.Entries });




                JsonResponse(json, HttpStatusCode.Created, context);

                context.Response.AddHeader("Location", ShipServiceUrl.PackageLatestVersion);
            }
            catch (NotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }

        protected void UploadMultiple(HttpContextBase context)
        {
            List<string> paths = new List<string>();
            List<TempPackager> tempPackagers = new List<TempPackager>();

            try
            {
                var uploadPackage = GetRequest(context.Request);

                if (context.Request.Files.Count == 0)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }


                for (var i = 0; i < context.Request.Files.Count; i++)
                {
                    var file = context.Request.Files[i];


                    var tempPackager = new TempPackager(new ServerTempFile());

                    tempPackagers.Add(tempPackager);

                    paths.Add(tempPackager.GetPackageToInstall(file.InputStream));
                }


                InstallPackages packages = new InstallPackages();
                packages.Paths = paths;
                packages.DisableIndexing = context.Request.Form["disableIndexing"] == "1";

                var manifests = _repository.AddPackages(packages);


                _installationRecorder.RecordInstall(uploadPackage.PackageId, uploadPackage.Description, DateTime.Now);


                foreach (var postInstall in _postPackageInstalls)
                {
                    postInstall.Execute(context.Request, manifests);
                }

                var json = Json.Encode(manifests.Select(x => x.Entries).ToArray());

                JsonResponse(json, HttpStatusCode.Created, context);

                context.Response.AddHeader("Location", ShipServiceUrl.PackageLatestVersion);


            }
            catch (NotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            finally
            {
                foreach (var tempPackager in tempPackagers)
                {
                    tempPackager.Dispose();
                }
            }
        }
    }
}