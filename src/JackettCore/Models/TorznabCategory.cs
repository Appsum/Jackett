using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JackettCore.Models
{
    public class TorznabCategory
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public List<TorznabCategory> SubCategories { get; private set; }

        public TorznabCategory()
        {
            SubCategories = new List<TorznabCategory>();
        }

        public TorznabCategory(int id, string name)
        {
            ID = id;
            Name = name;
            SubCategories = new List<TorznabCategory>();
        }

        public JToken ToJson()
        {
            var t = new JObject();
            t["ID"] = ID;
            t["Name"] = Name;
            return t;
        }
    }
}
