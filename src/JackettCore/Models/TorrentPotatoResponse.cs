﻿using System.Collections.Generic;

namespace JackettCore.Models
{
   public class TorrentPotatoResponse
    {
        public TorrentPotatoResponse()
        {
            results = new List<TorrentPotatoResponseItem>();
        }
        public List<TorrentPotatoResponseItem> results { get; set; }

        public int total_results
        {
            get { return results.Count; }
        }
    }
}
