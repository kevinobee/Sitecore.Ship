using System;
using System.Globalization;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Sitecore.Ship.Core;
using Sitecore.Ship.Core.Contracts;
using Sitecore.Ship.Core.Domain;

namespace Sitecore.Ship.Package.Install
{
    public class InstallerModule : NancyModule
    {
        private readonly IPackageRepository _repository;
        private readonly IAuthoriser _authoriser;
        private readonly ITempPackager _tempPackager;

        const string StartTime = "start_time";

        public InstallerModule(IPackageRepository repository, IAuthoriser authoriser, ITempPackager tempPackager)
            : base("/services")
        {
            _repository = repository;
            _authoriser = authoriser;
            _tempPackager = tempPackager;

            Before += AuthoriseRequest; 
            
            Before += ctx =>
            {
                ctx.Items.Add(StartTime, DateTime.UtcNow);
                return null;
            };

            After += AddProcessingTimeToResponse;

            Post["/package/install/fileupload"] = InstallUploadPackage;

            Post["/package/install"] = InstallPackage;
        }

        private Response AuthoriseRequest(NancyContext ctx)
        {
            if (!_authoriser.IsAllowed())
            {
                ctx.Response = 
                 new Response {StatusCode = HttpStatusCode.Unauthorized};
            }
            return null;
        }

        private static void AddProcessingTimeToResponse(NancyContext ctx)
        {
            var processTime = (DateTime.UtcNow - (DateTime)ctx.Items[StartTime]).TotalMilliseconds;
            System.Diagnostics.Debug.WriteLine("Processing Time: " + processTime);
            ctx.Response.WithHeader("x-processing-time", processTime.ToString(CultureInfo.InvariantCulture));
        }

        private dynamic InstallPackage(dynamic o)
        {
            try
            {
                var package = this.Bind<InstallPackage>();
                _repository.AddPackage(package);
                return Response.AsNewPackage(package);
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

                if (file == null)
                {
                    return new Response {StatusCode = HttpStatusCode.BadRequest};
                }

                try
                {
                    var package = new InstallPackage { Path = _tempPackager.GetPackageToInstall(file.Value) };
                    _repository.AddPackage(package);
                }
                finally 
                {
                    _tempPackager.Dispose();
                }

                return Response.AsNewPackage(new InstallPackage {Path = file.Name});
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