using Jackett.Indexers.Abstract;
using NLog;
using Jackett.Services;
using Jackett.Utils.Clients;

namespace Jackett.Indexers
{
    public class Avistaz : AvistazTracker, IIndexer
    {
        public Avistaz(IIndexerManagerService indexerManager, IWebClient webClient, Logger logger, IProtectionService protectionService)
            : base(name: "Avistaz",
                desc: "Aka AsiaTorrents",
                link: "https://avistaz.to/",
                indexerManager: indexerManager,
                logger: logger,
                protectionService: protectionService,
                webClient: webClient
                )
        {
        }
    }
}