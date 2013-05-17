using System.Web;

namespace Ship.Web.Serialization
{
    public class SerializationHandler : BaseHttpHandler
    {
        // TODO KO
//        public bool IsReusable
//        {
//            // Return false in case your Managed Handler cannot be reused for another request.
//            // Usually this would be false in case you have some state information preserved per request.
//            get { return true; }
//        }

        public override void ProcessRequest(HttpContextBase context)
        {
            var serializer = new SerializationRequest(context);
            serializer.Execute();
        }        
    }
}