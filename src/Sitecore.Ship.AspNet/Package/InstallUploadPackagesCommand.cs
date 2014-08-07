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
    public class InstallUploadPackagesCommand : CommandHandler
    {
        private readonly IPackageRepository _repository;
        
        private readonly IInstallationRecorder _installationRecorder;

        public InstallUploadPackagesCommand(IPackageRepository repository, IInstallationRecorder installationRecorder)
        {
            _repository = repository;
        
            _installationRecorder = installationRecorder;
        }

        public InstallUploadPackagesCommand()
            : this(new PackageRepository(new UpdatePackageRunner(new PackageManifestReader())), 
                  
                   new InstallationRecorder(new PackageHistoryRepository(), new PackageInstallationConfigurationProvider()))
        {           
        }

        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                List<string> paths = new List<string>();
                    List<TempPackager> tempPackagers = new List<TempPackager>();

                try
                {
                    var uploadPackage = GetRequest(context.Request);

                    if (context.Request.Files.Count == 0)
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
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


                    var json = Json.Encode(manifests.Select(x=>x.Entries).ToArray());

                    JsonResponse(json, HttpStatusCode.Created, context);

                    context.Response.AddHeader("Location", ShipServiceUrl.PackageLatestVersion);


                }
                catch (NotFoundException)
                {
                    context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                }
                finally
                {
                    foreach (var tempPackager in tempPackagers)
                    {
                        tempPackager.Dispose();
                    }
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
                   context.Request.Url.PathAndQuery.EndsWith("/services/package/install/fileupload/multi", StringComparison.InvariantCultureIgnoreCase) && 
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