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
using Sitecore.Ship.Infrastructure.Install;
using Sitecore.Ship.Infrastructure.Update;
using Sitecore.Ship.Infrastructure.Web;

namespace Sitecore.Ship.AspNet.Package
{
    public class InstallPackageCommand : CommandHandler
    {
        private readonly IPackageRepository _repository;
        private readonly IInstallationRecorder _installationRecorder;

        public InstallPackageCommand(IPackageRepository repository, IInstallationRecorder installationRecorder)
        {
            _repository = repository;
            _installationRecorder = installationRecorder;
        }

        public InstallPackageCommand() 
            : this(new PackageRepository(new UpdatePackageRunner(new PackageManifestReader())),
                   new InstallationRecorder(new PackageHistoryRepository(), new PackageInstallationConfigurationProvider().Settings))
        {           
        }

        public override void HandleRequest(HttpContextBase context)
        {
            if (CanHandle(context))
            {
                try
                {
                    var package = GetRequest(context.Request);
                    var manifest = _repository.AddPackage(package);
                    _installationRecorder.RecordInstall(package.Path, DateTime.Now);       

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
                   context.Request.Url.PathAndQuery.EndsWith("/services/package/install", StringComparison.InvariantCultureIgnoreCase) && 
                   context.Request.HttpMethod == "POST";
        }

        private static InstallPackage GetRequest(HttpRequestBase request)
        {
            return new InstallPackage { Path = request.Form["path"] };
        }
    }
}