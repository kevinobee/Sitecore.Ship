using System;
using System.Globalization;
using Nancy;
using Nancy.ModelBinding;
using Sitecore.Ship.Core;

namespace Sitecore.Ship.Package.Install
{
    public class InstallerModule : NancyModule
    {
        private readonly IPackageRepository _repository;

        const string StartTime = "start_time";

        public InstallerModule(IPackageRepository repository)
            : base("/services")
        {
            _repository = repository;

            Before += ctx =>
            {
                ctx.Items.Add(StartTime, DateTime.UtcNow);
                return null;
            };

            After += AddProcessingTimeToResponse;

            Post["/package/install"] = InstallPackage;
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
    }
}
