using Newtonsoft.Json.Linq;

namespace JackettCore.Models.IndexerConfig
{
    public interface ISerializableConfig
    {
        JObject Serialize();
        ISerializableConfig Deserialize(JObject jobj);
    }
}
