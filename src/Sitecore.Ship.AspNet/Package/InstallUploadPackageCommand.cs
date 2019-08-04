using System;
using System.Net;
using System.Web;
using Newtonsoft.Json;
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

        public InstallUploadPackageCommand(IPackageRepository repository, ITempPackager tempPackager, IInstallationRecorder installationRecorder)
        {
            _repository = repository;
            _tempPackager = tempPackager;
            _installationRecorder = installationRecorder;
        }

        public InstallUploadPackageCommand()
            : this(new PackageRepository(new UpdatePackageRunner(new PackageManifestReader())), 
                   new TempPackager(new ServerTempFile()), 
                   new InstallationRecorder(new PackageHistoryRepository(), new PackageInstallationConfigurationProvider().Settings))
        {           
        }

        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                try
                {
                    if (context.Request.Files.Count == 0)
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
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

                    var json = JsonConvert.SerializeObject(new { manifest.Entries });

                    JsonResponse(json, HttpStatusCode.Created, context);

                    context.Response.AddHeader("Location", ShipServiceUrl.PackageLatestVersion);                       
                }
                catch (NotFoundException)
                {
                    context.Response.StatusCode = (int) HttpStatusCode.NotFound;
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
    }
}