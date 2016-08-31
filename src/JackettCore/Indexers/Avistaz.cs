using JackettCore.Indexers.Abstract;
using JackettCore.Services;
using JackettCore.Utils.Clients;

namespace JackettCore.Indexers
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