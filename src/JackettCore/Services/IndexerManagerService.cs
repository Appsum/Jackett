using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using JackettCore.Indexers;
using JackettCore.Models;
using JackettCore.Utils.Clients;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace JackettCore.Services
{
    public interface IIndexerManagerService
    {
        Task TestIndexer(string name);
        void DeleteIndexer(string name);
        IIndexer GetIndexer(string name);
        IEnumerable<IIndexer> GetAllIndexers();
        void SaveConfig(IIndexer indexer, JToken obj);
        void InitIndexers();
    }

    public class IndexerManagerService : IIndexerManagerService
    {
        private readonly IContainer _container;
        private readonly IConfigurationService _configurationService;
        private readonly ILogger _logger;
        private readonly ICacheService _cacheService;
        private Dictionary<string, IIndexer> indexers = new Dictionary<string, IIndexer>();

        public IndexerManagerService(IContainer container, IConfigurationService configurationService, ILogger logger, ICacheService cacheService)
        {
            _container = container;
            _configurationService = configurationService;
            _logger = logger;
            _cacheService = cacheService;
        }

        public void InitIndexers()
        {
            _logger.LogInformation("Using HTTP Client: " + _container.Resolve<IWebClient>().GetType().Name);

            foreach (var idx in _container.Resolve<IEnumerable<IIndexer>>().OrderBy(_ => _.DisplayName))
            {
                indexers.Add(idx.ID, idx);
                var configFilePath = GetIndexerConfigFilePath(idx);
                if (File.Exists(configFilePath))
                {
                    var fileStr = File.ReadAllText(configFilePath);
                    var jsonString = JToken.Parse(fileStr);
                    try
                    {
                        idx.LoadFromSavedConfiguration(jsonString);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed loading configuration for {0}, you must reconfigure this indexer", idx.DisplayName);
                    }
                }
            }
        }

        public IIndexer GetIndexer(string name)
        {
            if (indexers.ContainsKey(name))
            {
                return indexers[name];
            }
            else
            {
                _logger.LogError("Request for unknown indexer: " + name);
                throw new Exception("Unknown indexer: " + name);
            }
        }

        public IEnumerable<IIndexer> GetAllIndexers()
        {
            return indexers.Values;
        }

        public async Task TestIndexer(string name)
        {
            var indexer = GetIndexer(name);
            var browseQuery = new TorznabQuery();
            var results = await indexer.PerformQuery(browseQuery);
            results = indexer.CleanLinks(results);
            _logger.LogInformation(string.Format("Found {0} releases from {1}", results.Count(), indexer.DisplayName));
            if (results.Count() == 0)
                throw new Exception("Found no results while trying to browse this tracker");
            _cacheService.CacheRssResults(indexer, results);
        }

        public void DeleteIndexer(string name)
        {
            var indexer = GetIndexer(name);
            var configPath = GetIndexerConfigFilePath(indexer);
            File.Delete(configPath);
            indexers[name] = _container.ResolveNamed<IIndexer>(indexer.ID);
        }

        private string GetIndexerConfigFilePath(IIndexer indexer)
        {
            return Path.Combine(_configurationService.GetIndexerConfigDir(), indexer.ID + ".json");
        }

        public void SaveConfig(IIndexer indexer, JToken obj)
        {
            var configFilePath = GetIndexerConfigFilePath(indexer);
            if (!Directory.Exists(_configurationService.GetIndexerConfigDir()))
                Directory.CreateDirectory(_configurationService.GetIndexerConfigDir());
            File.WriteAllText(configFilePath, obj.ToString());
        }
    }
}
