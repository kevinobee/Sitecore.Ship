using System;
using System.Web;

namespace Ship.Web
{
    public abstract class BaseHttpHandler : IHttpHandler
    {
        public virtual bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            // TODO KO use abstractions ILog
            Sitecore.Diagnostics.Log.Audit(this, "Started at: {0}", new[] { DateTime.Now.ToLongTimeString() });

            ProcessRequest(new HttpContextWrapper(context));
            
            Sitecore.Diagnostics.Log.Audit(this, "Ended at: {0}", new[] { DateTime.Now.ToLongTimeString() });
        }

        public abstract void ProcessRequest(HttpContextBase context);
    }
}