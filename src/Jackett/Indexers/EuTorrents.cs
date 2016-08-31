using Jackett.Indexers.Abstract;
using NLog;
using Jackett.Services;
using Jackett.Utils.Clients;

namespace Jackett.Indexers
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