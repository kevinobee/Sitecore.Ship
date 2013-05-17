using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace Ship.Web.Update
{
    public class PackageInstaller : IPackageInstaller
    {
        private readonly HttpContextBase _context;
        private readonly IAuthoriser _requestAuthoriser;
        private readonly IDictionary<string, Action<HttpContextBase>> _methodActions;

        public PackageInstaller(HttpContextBase context, IAuthoriser requestAuthoriser, IDictionary<string, Action<HttpContextBase>> httpMethodActions)
        {
            _context = context;
            _requestAuthoriser = requestAuthoriser;
            _methodActions = httpMethodActions;
        }

        public void Execute()
        {
            if (!_requestAuthoriser.IsAllowed())
            {
                _context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                return;
            }

            var requestMethod = _context.Request.HttpMethod;

            if (! _methodActions.ContainsKey(requestMethod))
            {
                _context.Response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
            }
            else
            {
                _methodActions[requestMethod].Invoke(_context);
            }
        }
    }
}