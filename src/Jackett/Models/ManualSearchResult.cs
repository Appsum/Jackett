using Jackett.Models;
using System.Collections.Generic;

namespace Jackett
{
    public class ManualSearchResult
    {
        public List<TrackerCacheResult> Results { get; set; }
        public List<string> Indexers { get; set; }
    }
}
