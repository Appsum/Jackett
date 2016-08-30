namespace Jackett.Models.IndexerConfig.Bespoke
{
    public class ConfigurationDataStrike : ConfigurationDataUrl
    {
        public DisplayItem StrikeWarning { get; private set; }

        public ConfigurationDataStrike(string url) : base(url)
        {
            StrikeWarning = new DisplayItem("This indexer does not support RSS Sync, only Search") { Name = "Warning" };
        }
    }
}
