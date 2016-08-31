using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using JackettCore.Indexers;
using JackettCore.Models;

namespace JackettCore.Services
{
    public interface ICacheService
    {
        void CacheRssResults(IIndexer indexer, IEnumerable<ReleaseInfo> releases);
        List<TrackerCacheResult> GetCachedResults();
        int GetNewItemCount(IIndexer indexer, IEnumerable<ReleaseInfo> releases);
    }

    public class CacheService : ICacheService
    {
        private readonly List<TrackerCache> _cache = new List<TrackerCache>();
        private const int MaxResultsPerTracker = 1000;

        public void CacheRssResults(IIndexer indexer, IEnumerable<ReleaseInfo> releases)
        {
            lock (_cache)
            {
                var trackerCache = _cache.FirstOrDefault(c => c.TrackerId == indexer.ID);
                if (trackerCache == null)
                {
                    trackerCache = new TrackerCache
                    {
                        TrackerId = indexer.ID,
                        TrackerName = indexer.DisplayName
                    };
                    _cache.Add(trackerCache);
                }

                foreach(var release in releases.OrderByDescending(i=>i.PublishDate))
                {
                    var existingItem = trackerCache.Results.FirstOrDefault(i => i.Result.Guid == release.Guid);
                    if (existingItem == null)
                    {
                        existingItem = new CachedResult
                        {
                            Created = DateTime.Now
                        };
                        trackerCache.Results.Add(existingItem);
                    }

                    existingItem.Result = release;
                }

                // Prune cache
                foreach(var tracker in _cache)
                {
                    tracker.Results = tracker.Results.OrderByDescending(i => i.Created).Take(MaxResultsPerTracker).ToList();
                }
            }
        }

        public int GetNewItemCount(IIndexer indexer, IEnumerable<ReleaseInfo> releases)
        {
            lock (_cache)
            {
                var newItemCount = 0;
                var trackerCache = _cache.FirstOrDefault(c => c.TrackerId == indexer.ID);
                if (trackerCache != null)
                {
                    newItemCount += releases.Count(release => trackerCache.Results.All(i => i.Result.Guid != release.Guid));
                }
                else {
                    newItemCount++;
                }

                return newItemCount;
            }
        }

        public List<TrackerCacheResult> GetCachedResults()
        {
            lock (_cache)
            {
                var results = new List<TrackerCacheResult>();

                foreach(var tracker in _cache)
                {
                    foreach(var release in tracker.Results.OrderByDescending(i => i.Result.PublishDate).Take(300))
                    {
                        var item = Mapper.Map<TrackerCacheResult>(release.Result);
                        item.FirstSeen = release.Created;
                        item.Tracker = tracker.TrackerName;
                        item.TrackerId = tracker.TrackerId;
                        item.Peers = item.Peers - item.Seeders; // Use peers as leechers
                        results.Add(item);
                    }
                }

                return results.Take(3000).OrderByDescending(i=>i.PublishDate).ToList();
            }
        }
    }
}
