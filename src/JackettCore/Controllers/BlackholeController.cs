using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JackettCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace JackettCore.Controllers
{
    [AllowAnonymous]
    [ResponseCache(CacheProfileName = "Never")]
    public class BlackholeController : Controller
    {
        private ILogger logger;
        private IIndexerManagerService indexerService;
        IServerService serverService;

        public BlackholeController(IIndexerManagerService i, ILogger l, IServerService s)
        {
            logger = l;
            indexerService = i;
            serverService = s;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Blackhole(string indexerID, string path, string apikey)
        {

            var jsonReply = new JObject();
            try
            {
                var indexer = indexerService.GetIndexer(indexerID);
                if (!indexer.IsConfigured)
                {
                    logger.Warn(string.Format("Rejected a request to {0} which is unconfigured.", indexer.DisplayName));
                    throw new Exception("This indexer is not configured.");
                }

                if (serverService.Config.APIKey != apikey)
                    throw new Exception("Incorrect API key");

                var remoteFile = new Uri(Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(path)), UriKind.RelativeOrAbsolute);
                remoteFile = indexer.UncleanLink(remoteFile);

                var downloadBytes = await indexer.Download(remoteFile);

                if (string.IsNullOrWhiteSpace(Engine.Server.Config.BlackholeDir))
                {
                    throw new Exception("Blackhole directory not set!");
                }

                if (!Directory.Exists(Engine.Server.Config.BlackholeDir))
                {
                    throw new Exception("Blackhole directory does not exist: " + Engine.Server.Config.BlackholeDir);
                }

                var fileName = DateTime.Now.Ticks + ".torrent";
                System.IO.File.WriteAllBytes(Path.Combine(Engine.Server.Config.BlackholeDir, fileName), downloadBytes);
                jsonReply["result"] = "success";
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error downloading to blackhole " + indexerID + " " + path);
                jsonReply["result"] = "error";
                jsonReply["error"] = ex.Message;
            }

            return Json(jsonReply);
        }
    }
}
