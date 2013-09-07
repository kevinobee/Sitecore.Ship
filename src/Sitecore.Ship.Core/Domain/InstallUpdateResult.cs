namespace Sitecore.Ship.Core.Domain
{
    public class InstallUpdateResult
    {
        public string Entity { get; set; }
        public string Action { get; set; }
        public string Behaviour { get; set; }
        public string Level { get; set; }
        public string MessageType { get; set; }
        public string ShortDescription { get; set; }
        public long Number { get; set; }
        public string MessageID { get; set; }
    }
}