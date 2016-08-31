using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using JackettCore.Models;
using JackettCore.Models.IndexerConfig;
using JackettCore.Services;
using JackettCore.Utils;
using JackettCore.Utils.Clients;
using Newtonsoft.Json.Linq;

namespace JackettCore.Indexers
{
    public class ThePirateBay : BaseIndexer, IIndexer
    {
        private const string DefaultSiteLink = "https://thepiratebay.mn/";

        private Uri BaseUri
        {
            get { return new Uri(ConfigData.Url.Value); }
            set { ConfigData.Url.Value = value.ToString(); }
        }

        private string SearchUrl => BaseUri + "search/{0}/0/99/208,205";
        private string RecentUrl => BaseUri + "recent";

        private ConfigurationDataUrl ConfigData
        {
            get { return (ConfigurationDataUrl)configData; }
            set { configData = value; }
        }

        public ThePirateBay(IIndexerManagerService i, Logger l, IWebClient wc, IProtectionService ps)
            : base("The Pirate Bay",
                description: "The worlds largest bittorrent indexer",
                link: DefaultSiteLink,
                caps: TorznabUtil.CreateDefaultTorznabTVCaps(),
                manager: i,
                client: wc,
                logger: l,
                p: ps,
                configData: new ConfigurationDataUrl(DefaultSiteLink))
        {
        }

        public async Task<IndexerConfigurationStatus> ApplyConfiguration(JToken configJson)
        {
            ConfigData.LoadValuesFromJson(configJson);
            var releases = await PerformQuery(new TorznabQuery());

            await ConfigureIfOK(string.Empty, releases.Any(), () =>
            {
                throw new Exception("Could not find releases from this URL");
            });

            return IndexerConfigurationStatus.Completed;
        }

        // Override to load legacy config format
        public override void LoadFromSavedConfiguration(JToken jsonConfig)
        {
            if (jsonConfig is JObject)
            {
                BaseUri = new Uri(jsonConfig.Value<string>("base_url"));
                SaveConfig();
                IsConfigured = true;
                return;
            }

            base.LoadFromSavedConfiguration(jsonConfig);
        }

        public async Task<IEnumerable<ReleaseInfo>> PerformQuery(TorznabQuery query)
        {
            var releases = new List<ReleaseInfo>();
            var queryStr = HttpUtility.UrlEncode(query.GetQueryString());
            var episodeSearchUrl = string.IsNullOrWhiteSpace(queryStr) ? RecentUrl : string.Format(SearchUrl, queryStr);
            var response = await RequestStringWithCookiesAndRetry(episodeSearchUrl, string.Empty);

            try
            {
                CQ dom = response.Content;

                var rows = dom["#searchResult > tbody > tr"];
                foreach (var row in rows)
                {
                    if (row.ChildElements.Count() < 2)
                        continue;

                    var release = new ReleaseInfo();
                    var qRow = row.Cq();
                    var qLink = qRow.Find(".detName > .detLink").First();

                    release.MinimumRatio = 1;
                    release.MinimumSeedTime = 172800;
                    release.Title = qLink.Text().Trim();
                    release.Description = release.Title;
                    release.Comments = new Uri(BaseUri + qLink.Attr("href").TrimStart('/'));
                    release.Guid = release.Comments;

                    var downloadCol = row.ChildElements.ElementAt(1).Cq().Children("a");
                    release.MagnetUri = new Uri(downloadCol.Attr("href"));
                    release.InfoHash = release.MagnetUri.ToString().Split(':')[3].Split('&')[0];

                    var descString = qRow.Find(".detDesc").Text().Trim();
                    var descParts = descString.Split(',');

                    var timeString = descParts[0].Split(' ')[1];

                    if (timeString.Contains(" ago"))
                    {
                        release.PublishDate = DateTime.Now - TimeSpan.FromMinutes(ParseUtil.CoerceInt(timeString.Split(' ')[0]));
                    }
                    else if (timeString.Contains("Today"))
                    {
                        release.PublishDate = (DateTime.UtcNow - TimeSpan.FromHours(2) - TimeSpan.Parse(timeString.Split(' ')[1])).ToLocalTime();
                    }
                    else if (timeString.Contains("Y-day"))
                    {
                        release.PublishDate = (DateTime.UtcNow - TimeSpan.FromHours(26) - TimeSpan.Parse(timeString.Split(' ')[1])).ToLocalTime();
                    }
                    else if (timeString.Contains(':'))
                    {
                        var utc = DateTime.ParseExact(timeString, "MM-dd HH:mm", CultureInfo.InvariantCulture) - TimeSpan.FromHours(2);
                        release.PublishDate = DateTime.SpecifyKind(utc, DateTimeKind.Utc).ToLocalTime();
                    }
                    else
                    {
                        var utc = DateTime.ParseExact(timeString, "MM-dd yyyy", CultureInfo.InvariantCulture) - TimeSpan.FromHours(2);
                        release.PublishDate = DateTime.SpecifyKind(utc, DateTimeKind.Utc).ToLocalTime();
                    }

                    release.Size = ReleaseInfo.GetBytes(descParts[1]);

                    release.Seeders = ParseUtil.CoerceInt(row.ChildElements.ElementAt(2).Cq().Text());
                    release.Peers = ParseUtil.CoerceInt(row.ChildElements.ElementAt(3).Cq().Text()) + release.Seeders;

                    releases.Add(release);
                }
            }
            catch (Exception ex)
            {
                OnParseError(response.Content, ex);
            }
            return releases.ToArray();
        }

        public override Task<byte[]> Download(Uri link)
        {
            throw new NotImplementedException();
        }
    }
}
