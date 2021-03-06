﻿using System.Collections.Generic;

namespace Jackett.Models
{
    class TrackerCache
    {
        public string TrackerId { set; get; }
        public string TrackerName { set; get; }

        public List<CachedResult> Results = new List<CachedResult>();
    }
}
