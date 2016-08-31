using System.Collections.Generic;

namespace Jackett.Models
{
    public class ManualSearchResult
    {
        public List<TrackerCacheResult> Results { get; set; }
        public List<string> Indexers { get; set; }
    }
}
