using System.Linq;
using Sitecore.Ship.Core.Domain;
using Sitecore.Update.Installer;

namespace Sitecore.Ship.Infrastructure.Update.Installer
{
    public static class ContingencyEntryExtensions
    {
        public static InstallUpdateResult ToInstallUpdateResult(this ContingencyEntry entry)
        {
            return new InstallUpdateResult
            {
                Entity = entry.CommandKey.Split(new[] { '_' }).FirstOrDefault(),
                Action = entry.Action,
                Behaviour = entry.Behavior.ToString(),
                Level = entry.Level.ToString(),
                MessageID = entry.MessageID,
                MessageType = entry.MessageType,
                Number = entry.Number,
                ShortDescription = entry.ShortDescription
            };
        }
    }
}
