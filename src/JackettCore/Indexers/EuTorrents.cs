using JackettCore.Indexers.Abstract;
using JackettCore.Services;
using JackettCore.Utils.Clients;

namespace JackettCore.Indexers
{
    public class EuTorrents : AvistazTracker, IIndexer
    {
        public EuTorrents(IIndexerManagerService indexerManager, IWebClient webClient, Logger logger, IProtectionService protectionService)
            : base(name: "EuTorrents",
                desc: "Part of the Avistaz network.",
                link: "https://eutorrents.to/",
                indexerManager: indexerManager,
                logger: logger,
                protectionService: protectionService,
                webClient: webClient
                )
        {
        }
    }
}