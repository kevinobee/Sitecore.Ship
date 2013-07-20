using System.Web;

namespace Sitecore.Ship.AspNet
{
    public abstract class CommandHandler
    {
        protected CommandHandler Successor;

        public void SetSuccessor(CommandHandler successor)
        {
            Successor = successor;
        }

        public abstract void HandleRequest(HttpContextBase context);
    }
}