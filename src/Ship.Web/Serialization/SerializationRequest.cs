using System.Linq;
using System.Web;

using Sitecore.Configuration;
using Sitecore.Data.Serialization;

namespace Ship.Web.Serialization
{
    public class SerializationRequest
    {
        private readonly HttpContextBase _context;

        public SerializationRequest(HttpContextBase context)
        {
            _context = context;
        }

        public void Execute()
        {
            foreach (var db in Factory.GetDatabaseNames().Where(name => name != "filesystem").Select(Factory.GetDatabase))
            {
                Manager.DumpTree(db.GetRootItem());
            }
        }
    }
}
