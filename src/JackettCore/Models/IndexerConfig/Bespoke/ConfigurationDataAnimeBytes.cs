namespace JackettCore.Models.IndexerConfig.Bespoke
{
    class ConfigurationDataAnimeBytes : ConfigurationDataBasicLogin
    {
        public BoolItem IncludeRaw { get; private set; }
        public DisplayItem DateWarning { get; private set; }
        public BoolItem InsertSeason { get; private set; }

        public ConfigurationDataAnimeBytes()
            : base()
        {
            IncludeRaw = new BoolItem() { Name = "IncludeRaw", Value = false };
            DateWarning = new DisplayItem("This tracker does not supply upload dates so they are based off year of release.") { Name = "DateWarning" };
            InsertSeason = new BoolItem() { Name = "Prefix episode number with S01 for Sonarr Compatability", Value = false };
        }
    }
}
