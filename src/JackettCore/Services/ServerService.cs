using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using JackettCore.Models.Config;
using JackettCore.Utils;
using JackettCore.Utils.Clients;
using Newtonsoft.Json.Linq;

namespace JackettCore.Services
{
    public interface IServerService
    {
        void Initalize();
        void Start();
        void Stop();
        void ReserveUrls(bool doInstall = true);
        ServerConfig Config { get; }
        void SaveConfig();
        Uri ConvertToProxyLink(Uri link, string serverUrl, string indexerId, string action = "dl", string file = "t.torrent");
        string BasePath();
    }

    public class ServerService : IServerService
    {
        private ServerConfig _config;

        private IDisposable _server;

        private readonly IIndexerManagerService _indexerService;
        private readonly IProcessService _processService;
        private ISerializeService _serializeService;
        private readonly IConfigurationService _configService;
        private readonly Logger _logger;
        private readonly IWebClient _client;
        private readonly IUpdateService _updater;

        public ServerService(IIndexerManagerService i, IProcessService p, ISerializeService s, IConfigurationService c, Logger l, IWebClient w, IUpdateService u)
        {
            _indexerService = i;
            _processService = p;
            _serializeService = s;
            _configService = c;
            _logger = l;
            _client = w;
            _updater = u;

            LoadConfig();
        }

        public ServerConfig Config => _config;

        public Uri ConvertToProxyLink(Uri link, string serverUrl, string indexerId, string action = "dl", string file = "t.torrent")
        {
            if (link == null || (link.IsAbsoluteUri && link.Scheme == "magnet"))
                return link;

            var encodedLink = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(link.ToString()));
            var urlEncodedFile = WebUtility.UrlEncode(file);
            var proxyLink = $"{serverUrl}{action}/{indexerId}/{_config.APIKey}?path={encodedLink}&file={urlEncodedFile}";
            return new Uri(proxyLink);
        }

        public string BasePath()
        {
            if (string.IsNullOrEmpty(_config.BasePathOverride)) {
                return "/";
            }
            var path = _config.BasePathOverride;
            if (!path.EndsWith("/"))
            {
                path = path + "/";
            }
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            return path;
        }

        private void LoadConfig()
        {
            // Load config
            _config = _configService.GetConfig<ServerConfig>() ?? new ServerConfig();

            if (string.IsNullOrWhiteSpace(_config.APIKey))
            {
                // Check for legacy key config
                var apiKeyFile = Path.Combine(_configService.GetAppDataFolder(), "api_key.txt");
                if (File.Exists(apiKeyFile))
                {
                    _config.APIKey = File.ReadAllText(apiKeyFile);
                }

                // Check for legacy settings

                var path = Path.Combine(_configService.GetAppDataFolder(), "config.json");
                if (File.Exists(path))
                {
                    var jsonReply = JObject.Parse(File.ReadAllText(path));
                    _config.Port = (int)jsonReply["port"];
                    _config.AllowExternal = (bool)jsonReply["public"];
                }

                if (string.IsNullOrWhiteSpace(_config.APIKey))
                    _config.APIKey = StringUtil.GenerateRandom(32);

                _configService.SaveConfig(_config);
            }

            if (string.IsNullOrWhiteSpace(_config.InstanceId))
            {
                _config.InstanceId = StringUtil.GenerateRandom(64);
                _configService.SaveConfig(_config);
            }
        }

        public void SaveConfig()
        {
            _configService.SaveConfig(_config);
        }

        public void Initalize()
        {
            _logger.Info("Starting Jackett " + _configService.GetVersion());
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            // Load indexers
            _indexerService.InitIndexers();
            _client.Init();
        }

        public void Start()
        {
            // Start the server
            _logger.Info("Starting web server at " + _config.GetListenAddresses()[0]);
            var startOptions = new StartOptions();
            _config.GetListenAddresses().ToList().ForEach(u => startOptions.Urls.Add(u));
            Startup.BasePath = BasePath();
            _server = WebApp.Start<Startup>(startOptions);
            _logger.Debug("Web server started");
            _updater.StartUpdateChecker();
        }

        public void ReserveUrls(bool doInstall = true)
        {
            _logger.Debug("Unreserving Urls");
            _config.GetListenAddresses(false).ToList().ForEach(u => RunNetSh($"http delete urlacl {u}"));
            _config.GetListenAddresses(true).ToList().ForEach(u => RunNetSh($"http delete urlacl {u}"));
            if (doInstall)
            {
                _logger.Debug("Reserving Urls");
                _config.GetListenAddresses(_config.AllowExternal).ToList().ForEach(u => RunNetSh(
                    $"http add urlacl {u} sddl=D:(A;;GX;;;S-1-1-0)"));
                _logger.Debug("Urls reserved");
            }
        }

        private void RunNetSh(string args)
        {
            _processService.StartProcessAndLog("netsh.exe", args);
        }

        public void Stop()
        {
            _server?.Dispose();
        }
    }
}
