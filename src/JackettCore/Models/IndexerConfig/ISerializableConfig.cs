using Newtonsoft.Json.Linq;

namespace Jackett.Models.IndexerConfig
{
    public interface ISerializableConfig
    {
        JObject Serialize();
        ISerializableConfig Deserialize(JObject jobj);
    }
}
